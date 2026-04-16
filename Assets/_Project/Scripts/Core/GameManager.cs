using UnityEngine;
using System;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that owns the <see cref="GameState"/> state
    /// machine and broadcasts every game-wide event. Has no knowledge of UI,
    /// audio, save, or board logic — those systems subscribe to the events
    /// below and react in isolation.
    /// </summary>
    /// <remarks>
    /// Command methods (<see cref="StartGame"/>, <see cref="RestartGame"/>,
    /// <see cref="ExitToMenu"/>, <see cref="PauseGame"/>,
    /// <see cref="ResumeGame"/>) are the only allowed external mutation
    /// path. Everything else observes.
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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Transition to <see cref="GameState.Playing"/> and load the game
        /// scene. Called by <c>ThemeSelectionPopup</c> after the player
        /// confirms a theme.
        /// </summary>
        public void StartGame()
        {
            SetState(GameState.Playing);
            SceneLoader.LoadGameScene();
        }

        /// <summary>
        /// Reset the in-scene match state without reloading the scene.
        /// Fires <see cref="OnGameRestarted"/> so the board, turn manager,
        /// timer, and HUD all clear themselves in place.
        /// </summary>
        public void RestartGame()
        {
            SetState(GameState.Playing);
            OnGameRestarted?.Invoke();
        }

        /// <summary>
        /// Abandon the current match and return to the main menu. Loads
        /// <c>PlayScene</c> and drops state to <see cref="GameState.MainMenu"/>.
        /// </summary>
        public void ExitToMenu()
        {
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
        /// <see cref="GameState.GameOver"/> and fires <see cref="OnGameOver"/>
        /// so the result popup, strike animator, and save manager can react.
        /// </summary>
        public void ReportGameOver(WinResult result)
        {
            if (result == null || !result.IsGameOver)
            {
                return;
            }

            SetState(GameState.GameOver);
            OnGameOver?.Invoke(result);
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
