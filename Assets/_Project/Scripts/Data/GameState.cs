namespace TicTacToe.Data
{
    /// <summary>
    /// High-level game state owned by <c>GameManager</c>. State transitions
    /// are broadcast via <c>GameManager.OnGameStateChanged</c>. UI listeners
    /// react to this enum rather than scene names or boolean flags.
    /// </summary>
    public enum GameState
    {
        /// <summary>Main menu is showing; no match in progress.</summary>
        MainMenu,

        /// <summary>Theme selection popup is open prior to starting a match.</summary>
        ThemeSelection,

        /// <summary>An active match is in progress; input is accepted on the board.</summary>
        Playing,

        /// <summary>Match is in progress but suspended (settings popup, app pause).</summary>
        Paused,

        /// <summary>Match has ended via win or draw; result popup is shown.</summary>
        GameOver
    }
}
