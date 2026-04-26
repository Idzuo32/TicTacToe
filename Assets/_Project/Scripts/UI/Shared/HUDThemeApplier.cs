using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Auto-applies <see cref="IThemeHUD"/> colors to a fixed set of HUD
    /// labels and the settings indicator graphic. Place one instance
    /// on the GameScene HUD root and assign the labels and indicator in
    /// the Inspector.
    /// </summary>
    /// <remarks>
    /// Listens to <see cref="ThemeManager.OnThemeChanged"/> and re-applies
    /// on every change, so a mid-match theme swap repaints the HUD without
    /// touching the gameplay or popup paths.
    /// </remarks>
    public class HUDThemeApplier : MonoBehaviour
    {
        [Tooltip("Every TMP label whose color should follow IThemeHUD.HUDTextColor — e.g. timer, current-player label, move counts.")]
        [SerializeField] private TMP_Text[] _hudTexts;

        [Tooltip("Optional graphic that highlights the settings bar. Tinted from IThemeHUD.SettingsIndicatorColor.")]
        [SerializeField] private Image _settingsIndicator;

        private void OnEnable()  => ThemeManager.OnThemeChanged += HandleThemeChanged;

        private void OnDisable() => ThemeManager.OnThemeChanged -= HandleThemeChanged;

        private void Start()
        {
            if (ThemeManager.Instance != null)
            {
                ApplyTheme(ThemeManager.Instance.ActiveThemeHUD);
            }
        }

        private void HandleThemeChanged(ITheme _)
        {
            if (ThemeManager.Instance != null)
            {
                ApplyTheme(ThemeManager.Instance.ActiveThemeHUD);
            }
        }

        private void ApplyTheme(IThemeHUD theme)
        {
            if (theme == null)
            {
                return;
            }

            if (_hudTexts != null)
            {
                for (int i = 0; i < _hudTexts.Length; i++)
                {
                    if (_hudTexts[i] != null)
                    {
                        _hudTexts[i].color = theme.HUDTextColor;
                    }
                }
            }

            if (_settingsIndicator != null)
            {
                _settingsIndicator.color = theme.SettingsIndicatorColor;
            }
        }
    }
}
