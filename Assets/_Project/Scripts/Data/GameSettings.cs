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
        /// <summary>Whether background music should play.</summary>
        public bool MusicEnabled = true;

        /// <summary>Whether sound effects should play.</summary>
        public bool SFXEnabled = true;

        /// <summary>
        /// Identifier of the currently selected theme. Must match a
        /// <c>ThemeSO.ThemeId</c> registered with <c>ThemeManager</c>.
        /// </summary>
        public string SelectedThemeId = "Classic";

        /// <inheritdoc />
        public string SaveKey => "settings";
    }
}
