using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
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
    /// changes. The selected option is visualised by toggling
    /// <c>Button.interactable</c> off on the active choice, which leverages
    /// the built-in Button color block without extra GameObjects. Navigation
    /// to the game scene goes through <see cref="GameManager.RequestStartGame"/>
    /// because the popup runs in <c>PlayScene</c> and the match begin must
    /// wait for <c>GameScene</c> systems to register.
    /// </remarks>
    public class ThemeSelectionPopup : PopupBase
    {
        [Header("Grid")]
        [Tooltip("Prefab for a single theme option. Must contain a Button, an Image for the theme icon, and a TMP_Text for the display name.")]
        [SerializeField] private GameObject _themeOptionPrefab;

        [Tooltip("Layout group that hosts the instantiated theme option buttons.")]
        [SerializeField] private Transform _themeGridContainer;

        [Header("Actions")]
        [Tooltip("Confirms the current selection, applies the theme, saves it, closes the popup, and requests a new match.")]
        [SerializeField] private Button _startButton;

        [Tooltip("Optional close button that dismisses the popup without starting a match.")]
        [SerializeField] private Button _closeButton;

        private readonly List<ThemeOption> _options = new();
        private string _selectedThemeId;

        private void OnEnable()
        {
            if (_startButton != null)
            {
                _startButton.onClick.AddListener(HandleStartClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(HandleCloseClicked);
            }
        }

        private void OnDisable()
        {
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
        /// from a clean grid and spawned buttons don't leak between sessions.
        /// </summary>
        protected override void OnClosed() => ClearOptions();

        private void SpawnOption(ThemeSO theme)
        {
            GameObject instance = Instantiate(_themeOptionPrefab, _themeGridContainer);
            Button button = instance.GetComponent<Button>();
            Image[] images = instance.GetComponentsInChildren<Image>(includeInactive: true);
            TMP_Text label = instance.GetComponentInChildren<TMP_Text>(includeInactive: true);

            if (button == null)
            {
                Debug.LogError($"[ThemeSelectionPopup] Theme option prefab is missing a Button component.");
                Destroy(instance);
                return;
            }

            Image icon = FindIconImage(images, button);
            if (icon != null)
            {
                icon.sprite = theme.XSprite;
                icon.color = theme.Player1Color;
            }

            if (label != null)
            {
                label.text = theme.DisplayName;
            }

            string themeId = theme.ThemeId;
            button.onClick.AddListener(() => SelectTheme(themeId));

            _options.Add(new ThemeOption(themeId, button));
        }

        private void SelectTheme(string themeId)
        {
            _selectedThemeId = themeId;

            for (int i = 0; i < _options.Count; i++)
            {
                ThemeOption option = _options[i];
                bool isSelected = option.ThemeId == themeId;
                if (option.Button != null)
                {
                    option.Button.interactable = !isSelected;
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
                ThemeOption option = _options[i];
                if (option.Button != null)
                {
                    option.Button.onClick.RemoveAllListeners();
                    Destroy(option.Button.gameObject);
                }
            }

            _options.Clear();
        }

        private static Image FindIconImage(Image[] images, Button button)
        {
            if (images == null)
            {
                return null;
            }

            Image buttonImage = button != null ? button.GetComponent<Image>() : null;

            for (int i = 0; i < images.Length; i++)
            {
                Image candidate = images[i];
                if (candidate != null && candidate != buttonImage)
                {
                    return candidate;
                }
            }

            return buttonImage;
        }

        private readonly struct ThemeOption
        {
            public string ThemeId { get; }
            public Button Button { get; }

            public ThemeOption(string themeId, Button button)
            {
                ThemeId = themeId;
                Button = button;
            }
        }
    }
}
