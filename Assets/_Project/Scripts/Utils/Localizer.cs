using System.Collections.Generic;

namespace TicTacToe
{
    /// <summary>
    /// Single seam between user-facing strings and the (eventually swappable)
    /// localisation backend. Every string the player sees should be requested
    /// through <see cref="Get"/> or <see cref="Format"/> using a key from
    /// <see cref="LocalisationKeys"/>; the current implementation returns
    /// English defaults from an in-memory table, but the call sites are now
    /// ready for a real translation provider to be dropped in without touching
    /// gameplay or UI code.
    /// </summary>
    /// <remarks>
    /// Values that contain <c>{0}</c>, <c>{1}</c> etc. are format templates
    /// consumed by <see cref="Format"/>. Plain strings should be requested
    /// through <see cref="Get"/>. Missing keys return <c>[key]</c> so the
    /// missing translation is loud on screen rather than silent.
    /// </remarks>
    public static class Localizer
    {
        private static readonly Dictionary<string, string> _english = new()
        {
            // Main menu
            { LocalisationKeys.MENU_PLAY,          "Play" },
            { LocalisationKeys.MENU_STATS,         "Stats" },
            { LocalisationKeys.MENU_SETTINGS,      "Settings" },
            { LocalisationKeys.MENU_EXIT,          "Exit" },

            // Theme selection
            { LocalisationKeys.THEME_TITLE,        "Choose a Theme" },
            { LocalisationKeys.THEME_START,        "Start Game" },

            // Stats popup (format templates)
            { LocalisationKeys.STATS_TITLE,        "Statistics" },
            { LocalisationKeys.STATS_TOTAL_GAMES,  "Total Games: {0}" },
            { LocalisationKeys.STATS_P1_WINS,      "Player 1 Wins: {0}" },
            { LocalisationKeys.STATS_P2_WINS,      "Player 2 Wins: {0}" },
            { LocalisationKeys.STATS_DRAWS,        "Draws: {0}" },
            { LocalisationKeys.STATS_AVG_DURATION, "Average Duration: {0}" },

            // Settings popup
            { LocalisationKeys.SETTINGS_TITLE,     "Settings" },
            { LocalisationKeys.SETTINGS_MUSIC,     "Music" },
            { LocalisationKeys.SETTINGS_SFX,       "Sound Effects" },

            // Exit confirm
            { LocalisationKeys.EXIT_CONFIRM_MESSAGE, "Are you sure you want to exit?" },
            { LocalisationKeys.EXIT_CONFIRM_YES,   "Yes" },
            { LocalisationKeys.EXIT_CONFIRM_NO,    "No" },

            // Game HUD
            { LocalisationKeys.HUD_PLAYER_1,       "Player 1" },
            { LocalisationKeys.HUD_PLAYER_2,       "Player 2" },
            { LocalisationKeys.HUD_TURN,           "Turn" },
            { LocalisationKeys.HUD_TIME,           "Time" },

            // Game result popup
            { LocalisationKeys.RESULT_P1_WINS,     "Player 1 Wins" },
            { LocalisationKeys.RESULT_P2_WINS,     "Player 2 Wins" },
            { LocalisationKeys.RESULT_DRAW,        "Draw" },
            { LocalisationKeys.RESULT_DURATION,    "Duration: {0}" },
            { LocalisationKeys.RESULT_RETRY,       "Retry" },
            { LocalisationKeys.RESULT_EXIT,        "Exit" },
        };

        /// <summary>
        /// Resolve a plain (non-format) string from the active table. Returns
        /// <c>[key]</c> when no entry exists so missing translations are
        /// visible at a glance instead of falling back to silent emptiness.
        /// </summary>
        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            return _english.TryGetValue(key, out string value) ? value : $"[{key}]";
        }

        /// <summary>
        /// Resolve a format template and substitute <paramref name="args"/>
        /// into it. Mirrors <see cref="string.Format(string, object[])"/>'s
        /// placeholder syntax. Returns <c>[key]</c> on a missing entry.
        /// </summary>
        public static string Format(string key, params object[] args)
        {
            string template = Get(key);
            return args == null || args.Length == 0 ? template : string.Format(template, args);
        }
    }
}
