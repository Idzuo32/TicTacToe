using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Play-scene root controller for the four main-menu buttons. Routes
    /// each click through <see cref="AudioManager"/> for the button SFX and
    /// then pushes the corresponding popup onto <see cref="PopupManager"/>.
    /// </summary>
    /// <remarks>
    /// Strictly a UI router — no game logic, no state mutation. The only
    /// outbound calls are <see cref="PopupManager.OpenPopup"/> and the
    /// button-click SFX. Listens to
    /// <see cref="GameManager.OnGameStateChanged"/> so any popups left open
    /// when returning to <see cref="GameState.MainMenu"/> (e.g. after a
    /// match exit) are dismissed cleanly.
    /// </remarks>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [Tooltip("Opens the ThemeSelectionPopup so the player can pick a theme and start a match.")]
        [SerializeField] private Button _playButton;

        [Tooltip("Opens the StatsPopup showing lifetime match statistics.")]
        [SerializeField] private Button _statsButton;

        [Tooltip("Opens the shared SettingsPopup for BGM/SFX toggles.")]
        [SerializeField] private Button _settingsButton;

        [Tooltip("Opens the ExitConfirmPopup to confirm application quit.")]
        [SerializeField] private Button _exitButton;

        [Header("Popups")]
        [Tooltip("ThemeSelectionPopup instance present in the Play Scene. Pushed onto the popup stack when Play is tapped.")]
        [SerializeField] private PopupBase _themeSelectionPopup;

        [Tooltip("StatsPopup instance present in the Play Scene. Pushed onto the popup stack when Stats is tapped.")]
        [SerializeField] private PopupBase _statsPopup;

        [Tooltip("Shared SettingsPopup instance present in the Play Scene. Pushed onto the popup stack when Settings is tapped.")]
        [SerializeField] private PopupBase _settingsPopup;

        [Tooltip("ExitConfirmPopup instance present in the Play Scene. Pushed onto the popup stack when Exit is tapped.")]
        [SerializeField] private PopupBase _exitConfirmPopup;

        private void OnEnable()
        {
            if (_playButton != null)
            {
                _playButton.onClick.AddListener(OnPlayClicked);
            }

            if (_statsButton != null)
            {
                _statsButton.onClick.AddListener(OnStatsClicked);
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.AddListener(OnExitClicked);
            }

            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            if (_playButton != null)
            {
                _playButton.onClick.RemoveListener(OnPlayClicked);
            }

            if (_statsButton != null)
            {
                _statsButton.onClick.RemoveListener(OnStatsClicked);
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.RemoveListener(OnExitClicked);
            }

            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        // Click handlers are private — wired exclusively from OnEnable so
        // an Inspector-side UnityEvent binding cannot accidentally double-fire.
        private void OnPlayClicked() => OpenPopup(_themeSelectionPopup);

        private void OnStatsClicked() => OpenPopup(_statsPopup);

        private void OnSettingsClicked() => OpenPopup(_settingsPopup);

        private void OnExitClicked() => OpenPopup(_exitConfirmPopup);

        private void OpenPopup(PopupBase popup)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            if (PopupManager.Instance == null || popup == null)
            {
                return;
            }

            PopupManager.Instance.OpenPopup(popup);
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState != GameState.MainMenu)
            {
                return;
            }

            if (PopupManager.Instance != null && PopupManager.Instance.HasOpenPopup)
            {
                PopupManager.Instance.CloseAllPopups();
            }
        }
    }
}
