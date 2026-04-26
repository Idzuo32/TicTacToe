using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Composes the on-screen player labels (name, name with mark glyph,
    /// move-count line, win heading) so the same mapping isn't reimplemented
    /// across the HUD, the result popup, and the stats popup. Routes the
    /// translatable parts through <see cref="Localizer"/>.
    /// </summary>
    public static class PlayerLabels
    {
        private const string MARK_X = "X";

        private const string MARK_O = "O";

        /// <summary>Returns "Player 1" or "Player 2" for the given mark.</summary>
        public static string PlayerName(PlayerMark mark)
        {
            return mark == PlayerMark.X
                ? Localizer.Get(LocalisationKeys.HUD_PLAYER_1)
                : Localizer.Get(LocalisationKeys.HUD_PLAYER_2);
        }

        /// <summary>Returns "Player 1" or "Player 2" for player number 1 or 2.</summary>
        public static string PlayerName(int playerNumber)
        {
            return playerNumber == 1
                ? Localizer.Get(LocalisationKeys.HUD_PLAYER_1)
                : Localizer.Get(LocalisationKeys.HUD_PLAYER_2);
        }

        /// <summary>
        /// Returns the HUD-style "Player N (X/O)" label combining the
        /// player name and their mark glyph.
        /// </summary>
        public static string PlayerNameWithMark(int playerNumber)
        {
            string mark = playerNumber == 1 ? MARK_X : MARK_O;
            return $"{PlayerName(playerNumber)} ({mark})";
        }

        /// <summary>
        /// Returns the HUD move-count line — e.g. "Player 1 — 3 moves" — for
        /// the given mark and count.
        /// </summary>
        public static string MoveCountLine(PlayerMark mark, int moveCount)
        {
            return $"{PlayerName(mark)} {moveCount} moves";
        }

        /// <summary>
        /// Returns the result-popup heading for a finished match — "Player 1
        /// Wins", "Player 2 Wins", or "Draw". Returns an empty string for
        /// in-progress or null results.
        /// </summary>
        public static string WinHeading(WinResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            if (result.IsDraw)
            {
                return Localizer.Get(LocalisationKeys.RESULT_DRAW);
            }

            if (result.Winner == PlayerMark.X)
            {
                return Localizer.Get(LocalisationKeys.RESULT_P1_WINS);
            }

            if (result.Winner == PlayerMark.O)
            {
                return Localizer.Get(LocalisationKeys.RESULT_P2_WINS);
            }

            return string.Empty;
        }
    }
}
