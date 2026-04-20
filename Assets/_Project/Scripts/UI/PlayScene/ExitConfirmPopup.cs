using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI
{
    /// <summary>
    /// Play-scene confirmation popup for the main menu Exit button.
    /// Confirms terminate the application; cancels pop the popup off the
    /// stack and return the user to the menu.
    /// </summary>
    /// <remarks>
    /// No content population is required on open — the popup is purely a
    /// two-button confirmation dialog. <see cref="Application.Quit"/> is a
    /// no-op in the editor, which is the expected behaviour during
    /// development testing; the APK build is the acceptance target.
    /// </remarks>
    public class ExitConfirmPopup : PopupBase
    {
        [Header("Actions")]
        [Tooltip("Quits the application. No-op in the editor by design.")]
        [SerializeField] private Button _confirmButton;

        [Tooltip("Dismisses the popup and returns to the main menu without quitting.")]
        [SerializeField] private Button _cancelButton;

        private void OnEnable()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(HandleConfirmClicked);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(HandleCancelClicked);
            }
        }

        private void OnDisable()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveListener(HandleConfirmClicked);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.RemoveListener(HandleCancelClicked);
            }
        }

        /// <summary>
        /// No content to populate — the popup message is static copy set in
        /// the prefab. Hook retained for PopupBase contract compliance.
        /// </summary>
        protected override void OnOpened() { }

        /// <summary>
        /// No teardown required.
        /// </summary>
        protected override void OnClosed() { }

        private void HandleConfirmClicked() => Application.Quit();

        private void HandleCancelClicked()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CloseTopPopup();
            }
        }
    }
}
