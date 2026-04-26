using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Lightweight per-Button component that auto-applies theme sprites,
    /// pressed/highlighted swap, and label color from the active
    /// <see cref="IThemeUI"/>. Add to every themed Button GameObject so
    /// theme switches restyle the whole UI without any central registry.
    /// </summary>
    /// <remarks>
    /// The component takes over the Button's transition mode by setting
    /// <see cref="Selectable.Transition.SpriteSwap"/> with the theme's
    /// pressed sprite mirrored on the highlighted slot — themes therefore
    /// only need to provide a normal and pressed sprite, not a full state
    /// matrix.
    /// </remarks>
    public class ButtonThemeApplier : MonoBehaviour
    {
        [Tooltip("The Button's primary background Image — receives the theme's normal sprite.")]
        [SerializeField] private Image _buttonImage;

        [Tooltip("Optional label text — receives the theme's button text color when present.")]
        [SerializeField] private TMP_Text _buttonText;

        private Button _button;

        private void Awake() => _button = GetComponent<Button>();

        private void OnEnable()  => ThemeManager.OnThemeChanged += HandleThemeChanged;

        private void OnDisable() => ThemeManager.OnThemeChanged -= HandleThemeChanged;

        private void Start()
        {
            if (ThemeManager.Instance != null)
            {
                ApplyTheme(ThemeManager.Instance.ActiveThemeUI);
            }
        }

        private void HandleThemeChanged(ITheme _)
        {
            if (ThemeManager.Instance != null)
            {
                ApplyTheme(ThemeManager.Instance.ActiveThemeUI);
            }
        }

        private void ApplyTheme(IThemeUI theme)
        {
            if (theme == null || _button == null)
            {
                return;
            }

            ColorBlock colors = _button.colors;
            colors.pressedColor = Color.grey;
            _button.colors = colors;

            if (_buttonImage != null)
            {
                _buttonImage.sprite = theme.ButtonNormalSprite;
            }

            SpriteState spriteState = _button.spriteState;
            spriteState.pressedSprite = theme.ButtonPressedSprite;
            spriteState.highlightedSprite = theme.ButtonPressedSprite;
            _button.spriteState = spriteState;
            _button.transition = Selectable.Transition.SpriteSwap;

            if (_buttonText != null)
            {
                _buttonText.color = theme.ButtonTextColor;
            }
        }
    }
}
