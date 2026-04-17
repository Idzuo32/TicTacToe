using UnityEngine;
using System;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that holds the active visual theme and
    /// broadcasts theme changes. Consumers depend on <see cref="ITheme"/>
    /// rather than <c>ThemeSO</c> so new themes can be added as assets
    /// without recompiling any code.
    /// </summary>
    public class ThemeManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static ThemeManager Instance { get; private set; }

        /// <summary>Fires whenever the active theme changes; carries the new theme.</summary>
        public static event Action<ITheme> OnThemeChanged;

        [Header("Themes")]
        [Tooltip("All selectable themes. Assigned in the Inspector. Must contain at least one entry with id 'Classic'.")]
        [SerializeField] private ThemeSO[] _availableThemes;

        /// <summary>The currently applied theme; non-null after <c>Awake</c>.</summary>
        public ITheme ActiveTheme { get; private set; }

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

            string savedThemeId = SaveManager.Instance != null && SaveManager.Instance.Settings != null
                ? SaveManager.Instance.Settings.SelectedThemeId
                : null;

            ApplyTheme(savedThemeId);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
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
                themeId = "Classic";
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

            ActiveTheme = resolved;

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateSelectedTheme(resolved.ThemeId);
            }

            OnThemeChanged?.Invoke(ActiveTheme);
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
