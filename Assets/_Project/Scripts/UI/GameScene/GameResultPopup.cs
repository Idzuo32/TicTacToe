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
    /// The popup cannot self-subscribe to <see cref="GameManager.OnGameOver"/>
    /// because its GameObject is inactive by default — <c>OnEnable</c>
    /// would never run. Instead, <see cref="GameHUDController"/> owns the
    /// <c>OnGameOver</c> subscription and calls <see cref="Show"/>, which
    /// caches the <see cref="WinResult"/> for <see cref="OnOpened"/> to
    /// consume and pushes the popup onto <see cref="PopupManager"/>. Retry
    /// dismisses the popup before calling <see cref="GameManager.RestartGame"/>
    /// so the rematch begins with a clean stack.
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

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_retryButton != null)
            {
                _retryButton.onClick.AddListener(HandleRetryClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.AddListener(HandleExitClicked);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

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
        /// Single entry point for opening this popup. Caches
        /// <paramref name="result"/> for <see cref="OnOpened"/> to read and
        /// pushes the popup onto <see cref="PopupManager"/>. Called by
        /// <see cref="GameHUDController"/> when <see cref="GameManager.OnGameOver"/>
        /// fires; the popup cannot subscribe to that event itself because it
        /// starts inactive in the scene and <c>OnEnable</c> never runs.
        /// </summary>
        public void Show(WinResult result)
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
                _resultLabel.text = PlayerLabels.WinHeading(_pendingResult);
            }

            if (_durationLabel != null)
            {
                _durationLabel.text = _gameTimer != null ? _gameTimer.FormattedTime : TimeFormatter.FormatMMSS(0f);
            }
        }

        /// <summary>
        /// Clear the cached result so a future open without a fresh
        /// OnGameOver cannot display a stale outcome.
        /// </summary>
        protected override void OnClosed() => _pendingResult = null;

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
    }
}
