using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// ScriptableObject implementation of <see cref="ITheme"/>. One asset
    /// per theme (e.g. <c>Theme_Classic</c>) lives under
    /// <c>Assets/_Project/Themes/</c> and is registered with
    /// <c>ThemeManager</c> via its Inspector array. Adding a new theme
    /// requires a new asset only — no code changes.
    /// </summary>
    [CreateAssetMenu(fileName = "Theme_Classic", menuName = "TicTacToe/Theme")]
    public class ThemeSO : ScriptableObject, ITheme
    {
        [Header("Identity")]
        [Tooltip("Stable id persisted in GameSettings. Must be unique across all themes (e.g. 'Classic').")]
        [SerializeField] private string _themeId;

        [Tooltip("Human-readable name shown in the theme selection popup.")]
        [SerializeField] private string _displayName;

        [Header("Sprites")]
        [SerializeField] private Sprite _xSprite;
        [SerializeField] private Sprite _oSprite;

        [Header("Player Colors")]
        [SerializeField] private Color _player1Color = Color.white;
        [SerializeField] private Color _player2Color = Color.white;

        /// <inheritdoc />
        public string ThemeId => _themeId;

        /// <inheritdoc />
        public string DisplayName => _displayName;

        /// <inheritdoc />
        public Sprite XSprite => _xSprite;

        /// <inheritdoc />
        public Sprite OSprite => _oSprite;

        /// <inheritdoc />
        public Color Player1Color => _player1Color;

        /// <inheritdoc />
        public Color Player2Color => _player2Color;
    }
}
