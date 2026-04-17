using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Pure, stateless evaluator for a 3×3 tic-tac-toe board. Given a
    /// flat <see cref="PlayerMark"/> array of length nine (row-major,
    /// index 0 = top-left), returns a <see cref="WinResult"/> describing
    /// whether the game has been won, drawn, or is still in progress.
    /// </summary>
    /// <remarks>
    /// Deliberately static with no Unity dependencies so it can be unit
    /// tested without instantiating a scene. This is the cleanest example
    /// of single-responsibility separation in the project — board state
    /// lives in <c>BoardController</c>, turn rotation in <c>TurnManager</c>,
    /// and outcome evaluation lives here.
    /// </remarks>
    public static class WinConditionChecker
    {
        private const int BOARD_SIZE = 9;
        private const int LINE_LENGTH = 3;

        /// <summary>
        /// The eight lines that constitute a win: three rows, three
        /// columns, and two diagonals. Each sub-array holds three cell
        /// indices in [0..8].
        /// </summary>
        private static readonly int[][] WIN_LINES =
        {
            new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, // rows
            new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, // columns
            new[] { 0, 4, 8 }, new[] { 2, 4, 6 }                    // diagonals
        };

        /// <summary>
        /// Evaluate a board snapshot and describe its outcome.
        /// </summary>
        /// <param name="board">Length-nine board in row-major order.</param>
        /// <returns>
        /// <see cref="WinResult.Win"/> if any line is completed by a single
        /// mark; <see cref="WinResult.Draw"/> if every cell is filled with
        /// no winner; <c>null</c> if the match is still in progress.
        /// </returns>
        public static WinResult Check(PlayerMark[] board)
        {
            if (board == null || board.Length != BOARD_SIZE)
            {
                return null;
            }

            for (int i = 0; i < WIN_LINES.Length; i++)
            {
                int[] line = WIN_LINES[i];
                PlayerMark mark = board[line[0]];

                if (mark == PlayerMark.None)
                {
                    continue;
                }

                if (board[line[1]] == mark && board[line[2]] == mark)
                {
                    return WinResult.Win(mark, line);
                }
            }

            if (IsBoardFull(board))
            {
                return WinResult.Draw();
            }

            return null;
        }

        private static bool IsBoardFull(PlayerMark[] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == PlayerMark.None)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
