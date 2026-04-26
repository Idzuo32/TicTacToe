using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;
using TicTacToe.UI;

namespace TicTacToe
{
    /// <summary>
    /// Game scene HUD. Mirrors <see cref="GameManager"/> and
    /// <see cref="GameTimer"/> state onto the on-screen labels — current
    /// player indicator, per-player move counts, and the match timer — and
    /// coordinates the scene's two popups: <see cref="SettingsPopup"/>
    /// (opened on button tap) and <see cref="GameResultPopup"/>
    /// (opened on <see cref="GameManager.OnGameOver"/>).
    /// </summary>
    /// <remarks>
    /// Strictly a listener for game state — never mutates it. The only
    /// outbound calls are <see cref="PopupManager.OpenPopup"/> for the
    /// settings popup and <see cref="GameResultPopup.Show"/> for the result
    /// popup. The HUD owns the result-popup open because that popup is
    /// inactive by default and cannot self-subscribe to <c>OnGameOver</c>;
    /// routing through the HUD keeps a single always-active listener in
    /// the scene. Move counts are tracked locally from
    /// <see cref="GameManager.OnMarkPlaced"/> so the HUD stays independent
    /// of any board-state API.
    /// </remarks>
    public class GameHUDController : MonoBehaviour
    {
        [Header("Current Turn")]
        [Tooltip("Label that shows whose turn it is. Copy is set from the active PlayerMark.")]
        [SerializeField] private TMP_Text _currentPlayerLabel;

        [Header("Move Counts")]
        [Tooltip("Player 1 (X) running move count for the current match.")]
        [SerializeField] private TMP_Text _player1MoveCountLabel;

        [Tooltip("Player 2 (O) running move count for the current match.")]
        [SerializeField] private TMP_Text _player2MoveCountLabel;

        [Header("Timer")]
        [Tooltip("Match timer display updated from GameTimer.OnTimerUpdated. Pre-formatted MM:SS.")]
        [SerializeField] private TMP_Text _timerLabel;

        [Header("Settings")]
        [Tooltip("Button that opens the shared settings popup via PopupManager.")]
        [SerializeField] private Button _settingsButton;

        [Tooltip("Settings popup instance present in this scene. Pushed onto the popup stack when the settings button is tapped.")]
        [SerializeField] private PopupBase _settingsPopup;

        [Header("Result")]
        [Tooltip("Result popup shown at end of match. Opened from HandleGameOver because the popup GameObject starts inactive in GameScene and cannot self-subscribe to GameManager.OnGameOver.")]
        [SerializeField] private GameResultPopup _gameResultPopup;

        [Tooltip("Seconds to wait after game-over before opening the result popup. Lets the strike-line reveal play out before the popup covers it.")]
        [SerializeField] private float _resultPopupDelay = 0.5f;

        private int _player1MoveCount;
        private int _player2MoveCount;
        private Coroutine _showResultPopupRoutine;

        private void OnEnable()
        {
            GameManager.OnTurnChanged += HandleTurnChanged;
            GameManager.OnMarkPlaced += HandleMarkPlaced;
            GameManager.OnGameOver += HandleGameOver;
            GameManager.OnGameRestarted += HandleGameRestarted;
            GameTimer.OnTimerUpdated += HandleTimerUpdated;

            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(HandleSettingsClicked);
            }

            ResetDisplays();
        }

        private void OnDisable()
        {
            GameManager.OnTurnChanged -= HandleTurnChanged;
            GameManager.OnMarkPlaced -= HandleMarkPlaced;
            GameManager.OnGameOver -= HandleGameOver;
            GameManager.OnGameRestarted -= HandleGameRestarted;
            GameTimer.OnTimerUpdated -= HandleTimerUpdated;

            if (_settingsButton != null)
            {
                _settingsButton.onClick.RemoveListener(HandleSettingsClicked);
            }
        }

        private void HandleTurnChanged(int playerNumber)
        {
            if (_currentPlayerLabel == null)
            {
                return;
            }

            _currentPlayerLabel.text = PlayerLabels.PlayerNameWithMark(playerNumber);
        }

        private void HandleMarkPlaced(PlayerMark mark)
        {
            if (mark == PlayerMark.X)
            {
                _player1MoveCount++;
                if (_player1MoveCountLabel != null)
                {
                    _player1MoveCountLabel.text = PlayerLabels.MoveCountLine(PlayerMark.X, _player1MoveCount);
                }
            }
            else if (mark == PlayerMark.O)
            {
                _player2MoveCount++;
                if (_player2MoveCountLabel != null)
                {
                    _player2MoveCountLabel.text = PlayerLabels.MoveCountLine(PlayerMark.O, _player2MoveCount);
                }
            }
        }

        /// <summary>
        /// Reacts to <see cref="GameManager.OnGameOver"/> by mirroring the
        /// outcome into the current-player label and surfacing the
        /// <see cref="GameResultPopup"/>. The HUD owns the popup open
        /// because the popup GameObject is inactive by default and cannot
        /// subscribe to the event itself.
        /// </summary>
        private void HandleGameOver(WinResult result)
        {
            if (result == null)
            {
                return;
            }

            if (_currentPlayerLabel != null)
            {
                _currentPlayerLabel.text = PlayerLabels.WinHeading(result);
            }

            if (_gameResultPopup != null && result.IsGameOver)
            {
                if (_showResultPopupRoutine != null)
                {
                    StopCoroutine(_showResultPopupRoutine);
                }
                _showResultPopupRoutine = StartCoroutine(ShowResultPopupDelayed(result));
            }
        }

        private IEnumerator ShowResultPopupDelayed(WinResult result)
        {
            if (_resultPopupDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(_resultPopupDelay);
            }

            _showResultPopupRoutine = null;

            if (_gameResultPopup != null)
            {
                _gameResultPopup.Show(result);
            }
        }

        private void HandleGameRestarted()
        {
            if (_showResultPopupRoutine != null)
            {
                StopCoroutine(_showResultPopupRoutine);
                _showResultPopupRoutine = null;
            }

            ResetDisplays();
        }

        private void HandleTimerUpdated(string formattedTime)
        {
            if (_timerLabel != null)
            {
                _timerLabel.text = formattedTime;
            }
        }

        private void HandleSettingsClicked()
        {
            if (PopupManager.Instance == null || _settingsPopup == null)
            {
                return;
            }

            PopupManager.Instance.OpenPopup(_settingsPopup);
        }

        private void ResetDisplays()
        {
            _player1MoveCount = 0;
            _player2MoveCount = 0;

            if (_player1MoveCountLabel != null)
            {
                _player1MoveCountLabel.text = PlayerLabels.MoveCountLine(PlayerMark.X, _player1MoveCount);
            }
            if (_player2MoveCountLabel != null)
            {
                _player2MoveCountLabel.text = PlayerLabels.MoveCountLine(PlayerMark.O, _player2MoveCount);
            }

            if (_timerLabel != null)
            {
                _timerLabel.text = TimeFormatter.FormatMMSS(0f);
            }

            if (_currentPlayerLabel != null)
            {
                _currentPlayerLabel.text = PlayerLabels.PlayerNameWithMark(1);
            }
        }
    }
}
