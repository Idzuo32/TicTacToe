namespace TicTacToe
{
    /// <summary>
    /// Canonical keys for every user-facing string in the project. Even
    /// though full localisation is out of scope for this build, all UI
    /// text is routed through these keys so a translation layer can be
    /// dropped in later without touching gameplay or UI controllers.
    /// </summary>
    public static class LocalisationKeys
    {
        // ─── Main Menu ──────────────────────────────────────────────────

        /// <summary>Main menu Play button label.</summary>
        public const string MENU_PLAY = "menu.play";

        /// <summary>Main menu Stats button label.</summary>
        public const string MENU_STATS = "menu.stats";

        /// <summary>Main menu Settings button label.</summary>
        public const string MENU_SETTINGS = "menu.settings";

        /// <summary>Main menu Exit button label.</summary>
        public const string MENU_EXIT = "menu.exit";

        // ─── Theme Selection ────────────────────────────────────────────

        /// <summary>Theme selection popup title.</summary>
        public const string THEME_TITLE = "theme.title";

        /// <summary>Theme selection Start Game confirmation button.</summary>
        public const string THEME_START = "theme.start";

        // ─── Stats Popup ────────────────────────────────────────────────

        /// <summary>Stats popup title.</summary>
        public const string STATS_TITLE = "stats.title";

        /// <summary>Stats row: total games played.</summary>
        public const string STATS_TOTAL_GAMES = "stats.total_games";

        /// <summary>Stats row: player 1 wins.</summary>
        public const string STATS_P1_WINS = "stats.p1_wins";

        /// <summary>Stats row: player 2 wins.</summary>
        public const string STATS_P2_WINS = "stats.p2_wins";

        /// <summary>Stats row: draws.</summary>
        public const string STATS_DRAWS = "stats.draws";

        /// <summary>Stats row: average match duration.</summary>
        public const string STATS_AVG_DURATION = "stats.avg_duration";

        // ─── Settings Popup ─────────────────────────────────────────────

        /// <summary>Settings popup title.</summary>
        public const string SETTINGS_TITLE = "settings.title";

        /// <summary>Settings toggle label for background music.</summary>
        public const string SETTINGS_MUSIC = "settings.music";

        /// <summary>Settings toggle label for sound effects.</summary>
        public const string SETTINGS_SFX = "settings.sfx";

        // ─── Exit Confirm Popup ─────────────────────────────────────────

        /// <summary>Exit confirmation question shown to the user.</summary>
        public const string EXIT_CONFIRM_MESSAGE = "exit.confirm_message";

        /// <summary>Exit confirmation positive button.</summary>
        public const string EXIT_CONFIRM_YES = "exit.yes";

        /// <summary>Exit confirmation negative button.</summary>
        public const string EXIT_CONFIRM_NO = "exit.no";

        // ─── Game HUD ───────────────────────────────────────────────────

        /// <summary>Player 1 display name on the HUD.</summary>
        public const string HUD_PLAYER_1 = "hud.player_1";

        /// <summary>Player 2 display name on the HUD.</summary>
        public const string HUD_PLAYER_2 = "hud.player_2";

        /// <summary>Current-turn label prefix on the HUD.</summary>
        public const string HUD_TURN = "hud.turn";

        /// <summary>Match timer label prefix on the HUD.</summary>
        public const string HUD_TIME = "hud.time";

        // ─── Game Result Popup ──────────────────────────────────────────

        /// <summary>Result popup heading when player 1 wins.</summary>
        public const string RESULT_P1_WINS = "result.p1_wins";

        /// <summary>Result popup heading when player 2 wins.</summary>
        public const string RESULT_P2_WINS = "result.p2_wins";

        /// <summary>Result popup heading on a draw.</summary>
        public const string RESULT_DRAW = "result.draw";

        /// <summary>Result popup duration label prefix.</summary>
        public const string RESULT_DURATION = "result.duration";

        /// <summary>Result popup Retry button.</summary>
        public const string RESULT_RETRY = "result.retry";

        /// <summary>Result popup Exit-to-menu button.</summary>
        public const string RESULT_EXIT = "result.exit";
    }
}
