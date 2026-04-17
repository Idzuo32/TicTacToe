using UnityEngine;
using System;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that owns the <see cref="GameState"/> state
    /// machine and coordinates the match lifecycle. Every game-wide event
    /// is broadcast from here so UI, audio, and save systems subscribe to
    /// one source of truth.
    /// </summary>
    /// <remarks>
    /// Holds scene-scoped references to <see cref="TurnManager"/> and
    /// <see cref="GameTimer"/> registered by those systems in their
    /// <c>OnEnable</c>. This keeps game-flow orchestration centralised
    /// without resorting to <c>FindObjectOfType</c> at runtime. Command
    /// methods (<see cref="StartGame"/>, <see cref="RestartGame"/>,
    /// <see cref="ExitToMenu"/>, <see cref="PauseGame"/>,
    /// <see cref="ResumeGame"/>) are the only allowed external mutation
    /// path.
    /// </remarks>
    public class GameManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static GameManager Instance { get; private set; }

        /// <summary>Fires on every state transition; carries the new state.</summary>
        public static event Action<GameState> OnGameStateChanged;

        /// <summary>Fires after a mark is successfully placed on the board.</summary>
        public static event Action<PlayerMark> OnMarkPlaced;

        /// <summary>Fires once per match when a winner or draw is determined.</summary>
        public static event Action<WinResult> OnGameOver;

        /// <summary>Fires when the active player changes; carries 1 or 2.</summary>
        public static event Action<int> OnTurnChanged;

        /// <summary>Fires when the board is reset for a rematch on the same scene.</summary>
        public static event Action OnGameRestarted;

        /// <summary>Current high-level state; mutated only via <see cref="SetState"/>.</summary>
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        private TurnManager _turnManager;
        private GameTimer _gameTimer;
        private bool _matchPending;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneLoader.OnSceneLoadCompleted += HandleSceneLoadCompleted;
        }

        private void OnDisable()
        {
            SceneLoader.OnSceneLoadCompleted -= HandleSceneLoadCompleted;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Called by <see cref="TurnManager"/> in its <c>OnEnable</c> so
        /// GameManager holds a live reference across scene loads without
        /// using <c>FindObjectOfType</c>.
        /// </summary>
        public void RegisterTurnManager(TurnManager turnManager)
        {
            _turnManager = turnManager;
            TryBeginPendingMatch();
        }

        /// <summary>Release the registered <see cref="TurnManager"/> on scene unload.</summary>
        public void UnregisterTurnManager(TurnManager turnManager)
        {
            if (_turnManager == turnManager)
            {
                _turnManager = null;
            }
        }

        /// <summary>
        /// Called by <see cref="GameTimer"/> in its <c>OnEnable</c> so
        /// GameManager can read <c>ElapsedSeconds</c> for save records and
        /// drive start/stop without a static accessor.
        /// </summary>
        public void RegisterGameTimer(GameTimer gameTimer)
        {
            _gameTimer = gameTimer;
            TryBeginPendingMatch();
        }

        /// <summary>Release the registered <see cref="GameTimer"/> on scene unload.</summary>
        public void UnregisterGameTimer(GameTimer gameTimer)
        {
            if (_gameTimer == gameTimer)
            {
                _gameTimer = null;
            }
        }

        /// <summary>
        /// Entry point used by <c>ThemeSelectionPopup</c> to navigate from
        /// the menu into a fresh match. Loads <c>GameScene</c> and defers
        /// the actual match begin to <see cref="StartGame"/>, which is
        /// triggered automatically once the scene load completes and the
        /// match systems have registered.
        /// </summary>
        public void RequestStartGame()
        {
            _matchPending = true;
            SceneLoader.LoadGameScene();
        }

        /// <summary>
        /// Begin a match in the already-loaded <c>GameScene</c>. Sets state
        /// to <see cref="GameState.Playing"/>, resets the turn rotation,
        /// and starts the timer from zero. Invoked automatically by the
        /// scene-load completion handler after <see cref="RequestStartGame"/>
        /// or manually once <see cref="TurnManager"/> and
        /// <see cref="GameTimer"/> have registered.
        /// </summary>
        public void StartGame()
        {
            SetState(GameState.Playing);

            if (_turnManager != null)
            {
                _turnManager.ResetTurns();
            }

            if (_gameTimer != null)
            {
                _gameTimer.ResetTimer();
                _gameTimer.StartTimer();
            }
        }

        /// <summary>
        /// Reset the in-scene match state without reloading the scene.
        /// Fires <see cref="OnGameRestarted"/> so the board, strike
        /// animator, and timer all clear themselves in place, then resets
        /// the turn rotation so <see cref="OnTurnChanged"/> fires for the
        /// fresh match.
        /// </summary>
        public void RestartGame()
        {
            SetState(GameState.Playing);
            OnGameRestarted?.Invoke();

            if (_turnManager != null)
            {
                _turnManager.ResetTurns();
            }
        }

        /// <summary>
        /// Abandon the current match and return to the main menu. Loads
        /// <c>PlayScene</c> and drops state to <see cref="GameState.MainMenu"/>.
        /// </summary>
        public void ExitToMenu()
        {
            _matchPending = false;
            SetState(GameState.MainMenu);
            SceneLoader.LoadPlayScene();
        }

        /// <summary>
        /// Suspend the active match. Idempotent — only transitions from
        /// <see cref="GameState.Playing"/>.
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            SetState(GameState.Paused);
        }

        /// <summary>
        /// Resume a paused match. Idempotent — only transitions from
        /// <see cref="GameState.Paused"/>.
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused)
            {
                return;
            }

            SetState(GameState.Playing);
        }

        /// <summary>
        /// Entry point for board logic to announce a placed mark. Kept as
        /// a method on GameManager so invocation stays centralised while
        /// the event itself remains a static broadcast.
        /// </summary>
        public void ReportMarkPlaced(PlayerMark mark) => OnMarkPlaced?.Invoke(mark);

        /// <summary>Entry point for turn rotation announcements.</summary>
        public void ReportTurnChanged(int playerNumber) => OnTurnChanged?.Invoke(playerNumber);

        /// <summary>
        /// Entry point for match completion. Transitions to
        /// <see cref="GameState.GameOver"/>, stops the timer, fires
        /// <see cref="OnGameOver"/> so popups and the strike animator can
        /// react, then hands the outcome to <see cref="SaveManager"/> for
        /// stats persistence.
        /// </summary>
        public void ReportGameOver(WinResult result)
        {
            if (result == null || !result.IsGameOver)
            {
                return;
            }

            SetState(GameState.GameOver);

            float elapsed = _gameTimer != null ? _gameTimer.ElapsedSeconds : 0f;

            if (_gameTimer != null)
            {
                _gameTimer.StopTimer();
            }

            OnGameOver?.Invoke(result);

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.RecordGameResult(result, elapsed);
            }
        }

        private void HandleSceneLoadCompleted(string sceneName)
        {
            if (sceneName != SceneNames.GameScene || !_matchPending)
            {
                return;
            }

            TryBeginPendingMatch();
        }

        private void TryBeginPendingMatch()
        {
            if (!_matchPending || _turnManager == null || _gameTimer == null)
            {
                return;
            }

            _matchPending = false;
            StartGame();
        }

        private void SetState(GameState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }
}
