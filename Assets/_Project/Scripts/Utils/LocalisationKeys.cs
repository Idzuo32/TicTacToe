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

        public const string MENU_PLAY = "menu.play";
        public const string MENU_STATS = "menu.stats";
        public const string MENU_SETTINGS = "menu.settings";
        public const string MENU_EXIT = "menu.exit";

        // ─── Theme Selection ────────────────────────────────────────────

        public const string THEME_TITLE = "theme.title";
        public const string THEME_START = "theme.start";

        // ─── Stats Popup ────────────────────────────────────────────────

        public const string STATS_TITLE = "stats.title";
        public const string STATS_TOTAL_GAMES = "stats.total_games";
        public const string STATS_P1_WINS = "stats.p1_wins";
        public const string STATS_P2_WINS = "stats.p2_wins";
        public const string STATS_DRAWS = "stats.draws";
        public const string STATS_AVG_DURATION = "stats.avg_duration";

        // ─── Settings Popup ─────────────────────────────────────────────

        public const string SETTINGS_TITLE = "settings.title";
        public const string SETTINGS_MUSIC = "settings.music";
        public const string SETTINGS_SFX = "settings.sfx";

        // ─── Exit Confirm Popup ─────────────────────────────────────────

        public const string EXIT_CONFIRM_MESSAGE = "exit.confirm_message";
        public const string EXIT_CONFIRM_YES = "exit.yes";
        public const string EXIT_CONFIRM_NO = "exit.no";

        // ─── Game HUD ───────────────────────────────────────────────────

        public const string HUD_PLAYER_1 = "hud.player_1";
        public const string HUD_PLAYER_2 = "hud.player_2";
        public const string HUD_TURN = "hud.turn";
        public const string HUD_TIME = "hud.time";

        // ─── Game Result Popup ──────────────────────────────────────────

        public const string RESULT_P1_WINS = "result.p1_wins";
        public const string RESULT_P2_WINS = "result.p2_wins";
        public const string RESULT_DRAW = "result.draw";
        public const string RESULT_DURATION = "result.duration";
        public const string RESULT_RETRY = "result.retry";
        public const string RESULT_EXIT = "result.exit";
    }
}
