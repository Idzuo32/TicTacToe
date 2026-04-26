using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Drives a single full-screen background <see cref="Image"/> from the
    /// active <see cref="IThemeUI"/>. Place one instance on the background
    /// GameObject in <c>PlayScene</c> and another in <c>GameScene</c>; each
    /// keeps itself in sync with the theme without any manager wiring.
    /// </summary>
    /// <remarks>
    /// When <see cref="IThemeUI.SceneBackgroundSprite"/> is non-null the
    /// sprite is applied with a white tint; otherwise the image falls back
    /// to <see cref="IThemeUI.SceneBackgroundColor"/> as a flat color.
    /// </remarks>
    public class SceneBackgroundController : MonoBehaviour
    {
        [Tooltip("The full-screen background Image this controller skins from the active IThemeUI.")]
        [SerializeField] private Image _backgroundImage;

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
            if (theme == null || _backgroundImage == null)
            {
                return;
            }

            if (theme.SceneBackgroundSprite != null)
            {
                _backgroundImage.sprite = theme.SceneBackgroundSprite;
                _backgroundImage.color = Color.white;
            }
            else
            {
                _backgroundImage.sprite = null;
                _backgroundImage.color = theme.SceneBackgroundColor;
            }
        }
    }
}
