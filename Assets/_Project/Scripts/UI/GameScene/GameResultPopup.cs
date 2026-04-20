using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Game-scene popup shown at the end of every match. Displays the
    /// outcome (Player 1 Wins, Player 2 Wins, or Draw) and the final match
    /// duration sourced from <see cref="GameTimer"/>, then offers Retry
    /// (rematch in place) or Exit (return to the main menu).
    /// </summary>
    /// <remarks>
    /// The popup self-opens on <see cref="GameManager.OnGameOver"/>: the
    /// event carries the <see cref="WinResult"/>, which is cached for
    /// <see cref="OnOpened"/> to consume. Self-opening keeps the
    /// end-of-match wiring inside a single class and avoids spreading
    /// "open the result popup" responsibility across the HUD or the
    /// board controller. Retry dismisses the popup before calling
    /// <see cref="GameManager.RestartGame"/> so the rematch begins with a
    /// clean stack.
    /// </remarks>
    public class GameResultPopup : PopupBase
    {
        [Header("Display")]
        [Tooltip("Primary heading set from the WinResult — 'Player 1 Wins', 'Player 2 Wins', or 'Draw'.")]
        [SerializeField] private TMP_Text _resultLabel;

        [Tooltip("Match duration label populated from GameTimer.FormattedTime on open.")]
        [SerializeField] private TMP_Text _durationLabel;

        [Header("Actions")]
        [Tooltip("Starts a rematch in place via GameManager.RestartGame and closes the popup.")]
        [SerializeField] private Button _retryButton;

        [Tooltip("Returns to the main menu via GameManager.ExitToMenu.")]
        [SerializeField] private Button _exitButton;

        [Header("Dependencies")]
        [Tooltip("Scene-local timer used to read ElapsedSeconds for the final duration display.")]
        [SerializeField] private GameTimer _gameTimer;

        private WinResult _pendingResult;

        private void OnEnable()
        {
            GameManager.OnGameOver += HandleGameOver;

            if (_retryButton != null)
            {
                _retryButton.onClick.AddListener(HandleRetryClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.AddListener(HandleExitClicked);
            }
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= HandleGameOver;

            if (_retryButton != null)
            {
                _retryButton.onClick.RemoveListener(HandleRetryClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.RemoveListener(HandleExitClicked);
            }
        }

        /// <summary>
        /// Write the cached <see cref="WinResult"/> into the heading and
        /// the current <see cref="GameTimer.FormattedTime"/> into the
        /// duration label. Falls back to <c>00:00</c> when the timer
        /// reference is missing so the UI never shows stale text.
        /// </summary>
        protected override void OnOpened()
        {
            if (_resultLabel != null)
            {
                _resultLabel.text = BuildResultText(_pendingResult);
            }

            if (_durationLabel != null)
            {
                _durationLabel.text = _gameTimer != null ? _gameTimer.FormattedTime : "00:00";
            }
        }

        /// <summary>
        /// Clear the cached result so a future open without a fresh
        /// OnGameOver cannot display a stale outcome.
        /// </summary>
        protected override void OnClosed() => _pendingResult = null;

        private void HandleGameOver(WinResult result)
        {
            if (result == null || !result.IsGameOver)
            {
                return;
            }

            _pendingResult = result;

            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.OpenPopup(this);
            }
        }

        private void HandleRetryClicked()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CloseTopPopup();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        private void HandleExitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ExitToMenu();
            }
        }

        private static string BuildResultText(WinResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            if (result.IsDraw)
            {
                return "Draw";
            }

            if (result.Winner == PlayerMark.X)
            {
                return "Player 1 Wins";
            }

            if (result.Winner == PlayerMark.O)
            {
                return "Player 2 Wins";
            }

            return string.Empty;
        }
    }
}
