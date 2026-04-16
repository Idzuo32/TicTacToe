namespace TicTacToe.Data
{
    /// <summary>
    /// Immutable outcome of a board evaluation produced by
    /// <c>WinConditionChecker.Check</c>. Three shapes are possible:
    /// no result yet (game still in progress), a draw, or a win with a
    /// specific winning line.
    /// </summary>
    public class WinResult
    {
        /// <summary>True when the board is full with no winner.</summary>
        public bool IsDraw { get; }

        /// <summary>True when a player has completed a winning line.</summary>
        public bool HasWinner => Winner != PlayerMark.None;

        /// <summary>True when the match is over (win or draw).</summary>
        public bool IsGameOver => HasWinner || IsDraw;

        /// <summary>The winning mark, or <see cref="PlayerMark.None"/> on draw / in-progress.</summary>
        public PlayerMark Winner { get; }

        /// <summary>
        /// Cell indices [0..8] forming the winning line, or <c>null</c>
        /// for draws and in-progress games. Always length 3 when present.
        /// </summary>
        public int[] WinLine { get; }

        private WinResult(bool isDraw, PlayerMark winner, int[] winLine)
        {
            IsDraw = isDraw;
            Winner = winner;
            WinLine = winLine;
        }

        /// <summary>Result indicating the game has not yet ended.</summary>
        public static WinResult InProgress() => new(false, PlayerMark.None, null);

        /// <summary>Result indicating a draw — board is full with no winner.</summary>
        public static WinResult Draw() => new(true, PlayerMark.None, null);

        /// <summary>Result indicating a win for the given mark on the given line.</summary>
        /// <param name="winner">The mark that completed the line; must not be <see cref="PlayerMark.None"/>.</param>
        /// <param name="winLine">The three cell indices forming the winning line.</param>
        public static WinResult Win(PlayerMark winner, int[] winLine) => new(false, winner, winLine);
    }
}
