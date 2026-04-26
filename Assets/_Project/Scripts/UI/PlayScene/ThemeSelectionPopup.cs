using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Play-scene popup that lists every registered <see cref="ThemeSO"/> as
    /// a selectable option and launches a match with the chosen theme applied.
    /// Selection state is tracked locally until the user confirms with Start,
    /// so backing out of the popup never mutates <see cref="GameSettings"/>.
    /// </summary>
    /// <remarks>
    /// Option buttons are instantiated from a shared prefab so adding a new
    /// theme requires a new <c>ThemeSO</c> asset only — no code or scene
    /// changes. Each instance carries a <see cref="ThemeOptionView"/> with
    /// explicit Inspector references so the popup never has to discover
    /// child Images or labels at runtime. Navigation to the game scene goes
    /// through <see cref="GameManager.RequestStartGame"/> because the popup
    /// runs in <c>PlayScene</c> and the match begin must wait for
    /// <c>GameScene</c> systems to register.
    /// </remarks>
    public class ThemeSelectionPopup : PopupBase
    {
        [Header("Grid")]
        [Tooltip("Prefab for a single theme option. Must carry a ThemeOptionView with its icon, label, and button references wired.")]
        [SerializeField] private ThemeOptionView _themeOptionPrefab;

        [Tooltip("Layout group that hosts the instantiated theme option views.")]
        [SerializeField] private Transform _themeGridContainer;

        [Header("Actions")]
        [Tooltip("Confirms the current selection, applies the theme, saves it, closes the popup, and requests a new match.")]
        [SerializeField] private Button _startButton;

        [Tooltip("Optional close button that dismisses the popup without starting a match.")]
        [SerializeField] private Button _closeButton;

        private readonly List<ThemeOptionView> _options = new();
        private string _selectedThemeId;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_startButton != null)
            {
                _startButton.onClick.AddListener(HandleStartClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(HandleCloseClicked);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(HandleStartClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        /// <summary>
        /// Rebuild the theme grid from <see cref="ThemeManager.GetAvailableThemes"/>
        /// and default the selection to the currently active theme so the
        /// popup reflects persisted state on every open.
        /// </summary>
        protected override void OnOpened()
        {
            ClearOptions();

            if (ThemeManager.Instance == null)
            {
                return;
            }

            ThemeSO[] themes = ThemeManager.Instance.GetAvailableThemes();
            if (themes == null || themes.Length == 0 || _themeOptionPrefab == null || _themeGridContainer == null)
            {
                return;
            }

            string initialSelection = ThemeManager.Instance.ActiveTheme != null
                ? ThemeManager.Instance.ActiveTheme.ThemeId
                : themes[0].ThemeId;

            for (int i = 0; i < themes.Length; i++)
            {
                ThemeSO theme = themes[i];
                if (theme == null)
                {
                    continue;
                }

                SpawnOption(theme);
            }

            SelectTheme(initialSelection);
        }

        /// <summary>
        /// Tear down every instantiated option so reopening the popup starts
        /// from a clean grid and spawned views don't leak between sessions.
        /// </summary>
        protected override void OnClosed() => ClearOptions();

        private void SpawnOption(ThemeSO theme)
        {
            ThemeOptionView option = Instantiate(_themeOptionPrefab, _themeGridContainer);
            option.Bind(theme, SelectTheme);
            _options.Add(option);
        }

        private void SelectTheme(string themeId)
        {
            _selectedThemeId = themeId;

            for (int i = 0; i < _options.Count; i++)
            {
                ThemeOptionView option = _options[i];
                if (option != null)
                {
                    option.SetSelected(option.ThemeId == themeId);
                }
            }
        }

        private void HandleStartClicked()
        {
            if (string.IsNullOrEmpty(_selectedThemeId))
            {
                return;
            }

            if (ThemeManager.Instance != null)
            {
                ThemeManager.Instance.ApplyTheme(_selectedThemeId);
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateSelectedTheme(_selectedThemeId);
            }

            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CloseTopPopup();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RequestStartGame();
            }
        }

        private void HandleCloseClicked()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CloseTopPopup();
            }
        }

        private void ClearOptions()
        {
            for (int i = 0; i < _options.Count; i++)
            {
                ThemeOptionView option = _options[i];
                if (option != null)
                {
                    if (option.Button != null)
                    {
                        option.Button.onClick.RemoveAllListeners();
                    }
                    Destroy(option.gameObject);
                }
            }

            _options.Clear();
        }
    }
}
