using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// View component on the theme-option prefab. Owns the explicit
    /// references to its icon, label, and button so
    /// <see cref="ThemeSelectionPopup"/> never has to discover them via
    /// <c>GetComponentsInChildren</c> at runtime — adding a decorative
    /// Image to the prefab no longer risks the wrong sprite being picked
    /// as the icon.
    /// </summary>
    public class ThemeOptionView : MonoBehaviour
    {
        [Header("Wiring")]
        [Tooltip("Image that displays the theme's representative sprite (typically the X mark).")]
        [SerializeField] private Image _icon;

        [Tooltip("Label that shows the theme's display name.")]
        [SerializeField] private TMP_Text _label;

        [Tooltip("The selectable button — its onClick is wired by Bind to forward this option's theme id.")]
        [SerializeField] private Button _button;

        private string _themeId;
        private Action<string> _onSelected;

        /// <summary>The theme id this option represents; empty until <see cref="Bind"/> runs.</summary>
        public string ThemeId => _themeId;

        /// <summary>The button that toggles selection — exposed so the popup can disable the active option.</summary>
        public Button Button => _button;

        /// <summary>
        /// Populate the icon and label from <paramref name="theme"/> and
        /// route taps to <paramref name="onSelected"/> with this option's
        /// theme id. Idempotent — calling Bind again replaces the previous
        /// listener.
        /// </summary>
        public void Bind(ThemeSO theme, Action<string> onSelected)
        {
            if (theme == null)
            {
                return;
            }

            _themeId = theme.ThemeId;
            _onSelected = onSelected;

            if (_icon != null)
            {
                _icon.sprite = theme.XSprite;
                _icon.color = theme.Player1Color;
            }

            if (_label != null)
            {
                _label.text = theme.DisplayName;
            }

            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(HandleClicked);
            }
        }

        /// <summary>
        /// Visually mark this option as selected by disabling its button.
        /// The popup uses the built-in Button color block to render the
        /// disabled state — no extra GameObjects required.
        /// </summary>
        public void SetSelected(bool isSelected)
        {
            if (_button != null)
            {
                _button.interactable = !isSelected;
            }
        }

        private void HandleClicked()
        {
            if (_onSelected != null && !string.IsNullOrEmpty(_themeId))
            {
                _onSelected(_themeId);
            }
        }
    }
}
