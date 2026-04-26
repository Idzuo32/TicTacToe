using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// ScriptableObject implementation of the four theme sub-interfaces
    /// (<see cref="ITheme"/>, <see cref="IThemeBoard"/>,
    /// <see cref="IThemeUI"/>, <see cref="IThemeHUD"/>,
    /// <see cref="IThemeAudio"/>). One asset per theme (e.g.
    /// <c>Theme_Classic</c>) lives under <c>Assets/_Project/Themes/</c> and
    /// is registered with <c>ThemeManager</c> via its Inspector array.
    /// Adding a new theme requires a new asset only — no code changes.
    /// </summary>
    /// <remarks>
    /// Sprite fields documented as "Leave null to use the Color fallback"
    /// are optional — consumers check for null and fall back to the
    /// matching color so themes can ship without bespoke art for every
    /// surface.
    /// </remarks>
    [CreateAssetMenu(fileName = "Theme_Classic", menuName = "TicTacToe/Theme")]
    public class ThemeSO : ScriptableObject, ITheme, IThemeBoard, IThemeUI, IThemeHUD, IThemeAudio
    {
        [Header("Identity")]
        [Tooltip("Stable id persisted in GameSettings. Must be unique across all themes (e.g. 'Classic').")]
        [SerializeField] private string _themeId;

        [Tooltip("Human-readable name shown in the theme selection popup.")]
        [SerializeField] private string _displayName;

        [Header("Board")]
        [Tooltip("Leave null to use the Color fallback")]
        [SerializeField] private Sprite _boardBackgroundSprite;

        [SerializeField] private Color _boardBackgroundColor = Color.white;

        [Tooltip("Leave null to use the Color fallback")]
        [SerializeField] private Sprite _cellDefaultSprite;

        [SerializeField] private Color _cellDefaultColor = Color.white;

        [Tooltip("Leave null to use the Color fallback")]
        [SerializeField] private Sprite _cellAlternateSprite;

        [SerializeField] private Color _cellAlternateColor = Color.white;

        [SerializeField] private Color _cellPressedTint = Color.grey;

        [Header("Marks")]
        [SerializeField] private Sprite _xSprite;
        [SerializeField] private Sprite _oSprite;
        [SerializeField] private Color _player1Color = Color.white;
        [SerializeField] private Color _player2Color = Color.white;

        [Header("Scene Background")]
        [Tooltip("Leave null to use the Color fallback")]
        [SerializeField] private Sprite _sceneBackgroundSprite;

        [SerializeField] private Color _sceneBackgroundColor = Color.white;

        [Header("Buttons")]
        [SerializeField] private Sprite _buttonNormalSprite;
        [SerializeField] private Sprite _buttonPressedSprite;
        [SerializeField] private Color _buttonTextColor = Color.white;

        [Header("Popups")]
        [SerializeField] private Sprite _popupBackgroundSprite;
        [SerializeField] private Color _popupTitleColor = Color.white;
        [SerializeField] private Color _popupBodyColor = Color.white;

        [Header("HUD")]
        [SerializeField] private Color _hudTextColor = Color.white;
        [SerializeField] private Color _settingsIndicatorColor = Color.white;
        [SerializeField] private Color _strikeColor = Color.red;

        [Header("Audio")]
        [Tooltip("Leave null to use the AudioManager's default BGM clip")]
        [SerializeField] private AudioClip _bgmClip;

        // ───── ITheme ─────────────────────────────────────────────────

        /// <inheritdoc />
        public string ThemeId => _themeId;

        /// <inheritdoc />
        public string DisplayName => _displayName;

        // ───── IThemeBoard ────────────────────────────────────────────

        /// <inheritdoc />
        public Sprite BoardBackgroundSprite => _boardBackgroundSprite;

        /// <inheritdoc />
        public Color BoardBackgroundColor => _boardBackgroundColor;

        /// <inheritdoc />
        public Sprite CellDefaultSprite => _cellDefaultSprite;

        /// <inheritdoc />
        public Color CellDefaultColor => _cellDefaultColor;

        /// <inheritdoc />
        public Sprite CellAlternateSprite => _cellAlternateSprite;

        /// <inheritdoc />
        public Color CellAlternateColor => _cellAlternateColor;

        /// <inheritdoc />
        public Color CellPressedTint => _cellPressedTint;

        /// <inheritdoc />
        public Sprite XSprite => _xSprite;

        /// <inheritdoc />
        public Sprite OSprite => _oSprite;

        /// <inheritdoc />
        public Color Player1Color => _player1Color;

        /// <inheritdoc />
        public Color Player2Color => _player2Color;

        // ───── IThemeUI ───────────────────────────────────────────────

        /// <inheritdoc />
        public Sprite SceneBackgroundSprite => _sceneBackgroundSprite;

        /// <inheritdoc />
        public Color SceneBackgroundColor => _sceneBackgroundColor;

        /// <inheritdoc />
        public Sprite ButtonNormalSprite => _buttonNormalSprite;

        /// <inheritdoc />
        public Sprite ButtonPressedSprite => _buttonPressedSprite;

        /// <inheritdoc />
        public Color ButtonTextColor => _buttonTextColor;

        /// <inheritdoc />
        public Sprite PopupBackgroundSprite => _popupBackgroundSprite;

        /// <inheritdoc />
        public Color PopupTitleColor => _popupTitleColor;

        /// <inheritdoc />
        public Color PopupBodyColor => _popupBodyColor;

        // ───── IThemeHUD ──────────────────────────────────────────────

        /// <inheritdoc />
        public Color HUDTextColor => _hudTextColor;

        /// <inheritdoc />
        public Color SettingsIndicatorColor => _settingsIndicatorColor;

        /// <inheritdoc />
        public Color StrikeColor => _strikeColor;

        // ───── IThemeAudio ────────────────────────────────────────────

        /// <inheritdoc />
        public AudioClip BGMClip => _bgmClip;
    }
}
