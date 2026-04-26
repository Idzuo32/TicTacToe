using UnityEngine;
using System;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that holds the active visual theme and
    /// broadcasts theme changes. Exposes the theme through four
    /// segregated interfaces (<see cref="ITheme"/>, <see cref="IThemeBoard"/>,
    /// <see cref="IThemeUI"/>, <see cref="IThemeHUD"/>,
    /// <see cref="IThemeAudio"/>) so consumers depend only on the data
    /// surface they actually need.
    /// </summary>
    /// <remarks>
    /// <see cref="OnThemeChanged"/> intentionally carries <see cref="ITheme"/>
    /// only — listeners that need board/UI/HUD/audio data cast through the
    /// appropriate <c>ActiveTheme*</c> property on this manager. That keeps
    /// the broadcast payload narrow while still letting subscribers reach
    /// the segregated surfaces they need.
    /// </remarks>
    public class ThemeManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static ThemeManager Instance { get; private set; }

        /// <summary>
        /// Fires whenever the active theme changes; carries the identity
        /// view of the new theme. Listeners that need richer surfaces read
        /// the <c>ActiveTheme*</c> properties on the singleton instead.
        /// </summary>
        public static event Action<ITheme> OnThemeChanged;

        [Header("Themes")]
        [Tooltip("All selectable themes. Assigned in the Inspector. Must contain at least one entry with id matching ThemeIds.DEFAULT.")]
        [SerializeField] private ThemeSO[] _availableThemes;

        /// <summary>The currently applied theme's identity view; non-null after <c>Awake</c>.</summary>
        public ITheme ActiveTheme { get; private set; }

        /// <summary>The currently applied theme's board surface; non-null after <c>Awake</c>.</summary>
        public IThemeBoard ActiveThemeBoard { get; private set; }

        /// <summary>The currently applied theme's UI surface; non-null after <c>Awake</c>.</summary>
        public IThemeUI ActiveThemeUI { get; private set; }

        /// <summary>The currently applied theme's HUD surface; non-null after <c>Awake</c>.</summary>
        public IThemeHUD ActiveThemeHUD { get; private set; }

        /// <summary>The currently applied theme's audio surface; non-null after <c>Awake</c>.</summary>
        public IThemeAudio ActiveThemeAudio { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_availableThemes == null || _availableThemes.Length == 0)
            {
                Debug.LogError("[ThemeManager] No themes assigned. Populate _availableThemes in the Inspector.");
                return;
            }

            // Seed every active-theme surface from the first available
            // ThemeSO so they're never null between Awake and the first
            // OnSettingsLoaded callback. Done without persisting so we
            // can't overwrite a previously-saved selection if
            // SaveManager.Awake runs before ours.
            BindActiveTheme(_availableThemes[0]);
        }

        private void OnEnable()
        {
            SaveManager.OnSettingsLoaded += HandleSettingsLoaded;

            // SaveManager may have already loaded by the time this OnEnable
            // runs — pick up its current settings so the saved theme applies
            // even if we missed the broadcast.
            if (SaveManager.Instance != null && SaveManager.Instance.Settings != null)
            {
                ApplyTheme(SaveManager.Instance.Settings.SelectedThemeId);
            }
        }

        private void OnDisable()
        {
            SaveManager.OnSettingsLoaded -= HandleSettingsLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void HandleSettingsLoaded(GameSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            ApplyTheme(settings.SelectedThemeId);
        }

        /// <summary>
        /// Activate the theme whose id matches <paramref name="themeId"/>.
        /// Falls back to the first available theme when no match is found,
        /// so a stale id in the save file can never leave the game themeless.
        /// Persists the choice through <see cref="SaveManager"/> and fires
        /// <see cref="OnThemeChanged"/>.
        /// </summary>
        /// <param name="themeId">The <c>ThemeId</c> of a theme in <c>_availableThemes</c>.</param>
        public void ApplyTheme(string themeId)
        {
            if (string.IsNullOrEmpty(themeId))
            {
                themeId = ThemeIds.DEFAULT;
            }

            if (_availableThemes == null || _availableThemes.Length == 0)
            {
                return;
            }

            ThemeSO resolved = FindThemeById(themeId);
            if (resolved == null)
            {
                Debug.LogWarning($"[ThemeManager] Theme id '{themeId}' not found. Falling back to '{_availableThemes[0].ThemeId}'.");
                resolved = _availableThemes[0];
            }

            if (ActiveTheme != null && ActiveTheme.ThemeId == resolved.ThemeId)
            {
                return;
            }

            BindActiveTheme(resolved);

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateSelectedTheme(resolved.ThemeId);
            }

            OnThemeChanged?.Invoke(ActiveTheme);
        }

        private void BindActiveTheme(ThemeSO theme)
        {
            ActiveTheme = theme;
            ActiveThemeBoard = theme;
            ActiveThemeUI = theme;
            ActiveThemeHUD = theme;
            ActiveThemeAudio = theme;
        }

        /// <summary>
        /// Returns the full list of selectable themes — used by
        /// <c>ThemeSelectionPopup</c> to populate its grid.
        /// </summary>
        public ThemeSO[] GetAvailableThemes() => _availableThemes;

        private ThemeSO FindThemeById(string themeId)
        {
            if (string.IsNullOrEmpty(themeId))
            {
                return null;
            }

            for (int i = 0; i < _availableThemes.Length; i++)
            {
                ThemeSO theme = _availableThemes[i];
                if (theme != null && theme.ThemeId == themeId)
                {
                    return theme;
                }
            }

            return null;
        }
    }
}
