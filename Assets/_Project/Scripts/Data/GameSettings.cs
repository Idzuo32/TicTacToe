using System;

namespace TicTacToe.Data
{
    /// <summary>
    /// User-configurable settings persisted across sessions. Serialised by
    /// <c>SaveSystem</c> as <c>settings.json</c>. Defaults are applied for
    /// first-run users who have no save file on disk.
    /// </summary>
    [Serializable]
    public class GameSettings : ISaveable
    {
        public bool MusicEnabled = true;

        public bool SFXEnabled = true;

        /// <summary>
        /// Identifier of the currently selected theme. Must match a
        /// <c>ThemeSO.ThemeId</c> registered with <c>ThemeManager</c>.
        /// </summary>
        public string SelectedThemeId = ThemeIds.DEFAULT;

        /// <summary>On-disk save key for this data class.</summary>
        public const string SAVE_KEY = "settings";

        /// <inheritdoc />
        public string SaveKey => SAVE_KEY;
    }
}
