using System;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Shared settings popup shown from both the main menu and the game HUD.
    /// Binds two persistent audio toggles (<c>MusicEnabled</c> and
    /// <c>SFXEnabled</c>) to <see cref="SaveManager"/>; changes round-trip
    /// to disk immediately so the setting survives app suspension.
    /// </summary>
    /// <remarks>
    /// Listeners on the toggles are wired in <see cref="OnOpened"/> and
    /// detached in <see cref="OnClosed"/> so a closed popup never forwards
    /// stray UI events to <see cref="SaveManager"/> — matters on mobile
    /// where the OS can flip toggle state during activation transitions.
    /// Audio is not touched here; <see cref="AudioManager"/> is the sole
    /// consumer of <c>OnSettingsChanged</c>.
    /// </remarks>
    public class SettingsPopup : PopupBase
    {
        [Header("Audio Toggles")]
        [Tooltip("Background music toggle. Initial value is populated from GameSettings on open.")]
        [SerializeField] private Toggle _musicToggle;

        [Tooltip("Sound effects toggle. Initial value is populated from GameSettings on open.")]
        [SerializeField] private Toggle _sfxToggle;

        [Header("Navigation")]
        [Tooltip("Optional close button that dismisses the popup via PopupManager. Leave null to rely on an external close path.")]
        [SerializeField] private Button _closeButton;

        [Tooltip("Optional exit button. Leave null in scenes where exiting is not applicable (e.g. main menu). Click is forwarded via OnExitClicked so the host scene decides what 'exit' means.")]
        [SerializeField] private Button _exitButton;

        /// <summary>
        /// Raised when the optional exit button is clicked. The popup itself
        /// does not act on this — host controllers (e.g. the game HUD) decide
        /// what exiting means in their scene.
        /// </summary>
        public event Action OnExitClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(HandleCloseClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.AddListener(HandleExitClicked);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(HandleCloseClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.RemoveListener(HandleExitClicked);
            }
        }

        /// <summary>
        /// Populate both toggles from the persisted <see cref="GameSettings"/>
        /// and subscribe to change events. Values are set with notification
        /// suppressed so populating the UI does not immediately trigger a
        /// disk write.
        /// </summary>
        protected override void OnOpened()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.Settings == null)
            {
                return;
            }

            GameSettings settings = SaveManager.Instance.Settings;

            if (_musicToggle != null)
            {
                _musicToggle.SetIsOnWithoutNotify(settings.MusicEnabled);
                _musicToggle.onValueChanged.AddListener(HandleMusicToggleChanged);
            }

            if (_sfxToggle != null)
            {
                _sfxToggle.SetIsOnWithoutNotify(settings.SFXEnabled);
                _sfxToggle.onValueChanged.AddListener(HandleSFXToggleChanged);
            }
        }

        /// <summary>
        /// Detach toggle change listeners so the popup is inert once closed.
        /// </summary>
        protected override void OnClosed()
        {
            if (_musicToggle != null)
            {
                _musicToggle.onValueChanged.RemoveListener(HandleMusicToggleChanged);
            }

            if (_sfxToggle != null)
            {
                _sfxToggle.onValueChanged.RemoveListener(HandleSFXToggleChanged);
            }
        }

        private void HandleMusicToggleChanged(bool isOn)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateMusicEnabled(isOn);
            }
        }

        private void HandleSFXToggleChanged(bool isOn)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateSFXEnabled(isOn);
            }
        }

        private void HandleCloseClicked()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CloseTopPopup();
            }
        }

        private void HandleExitClicked()
        {
            OnExitClicked?.Invoke();
        }
    }
}
