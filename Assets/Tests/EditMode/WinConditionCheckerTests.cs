using NUnit.Framework;
using TicTacToe;
using TicTacToe.Data;

namespace TicTacToe.Tests
{
    /// <summary>
    /// Edit-mode tests for <see cref="WinConditionChecker"/>. The checker is
    /// pure C# with zero Unity dependencies, so the entire matrix of board
    /// outcomes can be exercised without any scene or play-mode harness.
    /// This is the marquee SOLID demonstration in the project — these tests
    /// are the proof.
    /// </summary>
    [TestFixture]
    public class WinConditionCheckerTests
    {
        // ───── Wins: rows ──────────────────────────────────────────────

        [Test]
        public void Check_TopRowFilledByX_ReturnsXWin()
        {
            PlayerMark[] board = MakeBoard(
                X, X, X,
                _, _, _,
                _, _, _);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.X, new[] { 0, 1, 2 });
        }

        [Test]
        public void Check_MiddleRowFilledByO_ReturnsOWin()
        {
            PlayerMark[] board = MakeBoard(
                _, _, _,
                O, O, O,
                _, _, _);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.O, new[] { 3, 4, 5 });
        }

        [Test]
        public void Check_BottomRowFilledByX_ReturnsXWin()
        {
            PlayerMark[] board = MakeBoard(
                _, _, _,
                _, _, _,
                X, X, X);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.X, new[] { 6, 7, 8 });
        }

        // ───── Wins: columns ──────────────────────────────────────────

        [Test]
        public void Check_LeftColumnFilledByO_ReturnsOWin()
        {
            PlayerMark[] board = MakeBoard(
                O, _, _,
                O, _, _,
                O, _, _);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.O, new[] { 0, 3, 6 });
        }

        [Test]
        public void Check_MiddleColumnFilledByX_ReturnsXWin()
        {
            PlayerMark[] board = MakeBoard(
                _, X, _,
                _, X, _,
                _, X, _);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.X, new[] { 1, 4, 7 });
        }

        [Test]
        public void Check_RightColumnFilledByO_ReturnsOWin()
        {
            PlayerMark[] board = MakeBoard(
                _, _, O,
                _, _, O,
                _, _, O);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.O, new[] { 2, 5, 8 });
        }

        // ───── Wins: diagonals ────────────────────────────────────────

        [Test]
        public void Check_MainDiagonalFilledByX_ReturnsXWin()
        {
            PlayerMark[] board = MakeBoard(
                X, _, _,
                _, X, _,
                _, _, X);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.X, new[] { 0, 4, 8 });
        }

        [Test]
        public void Check_AntiDiagonalFilledByO_ReturnsOWin()
        {
            PlayerMark[] board = MakeBoard(
                _, _, O,
                _, O, _,
                O, _, _);

            WinResult result = WinConditionChecker.Check(board);

            AssertWin(result, PlayerMark.O, new[] { 2, 4, 6 });
        }

        // ───── Draws and in-progress ─────────────────────────────────

        [Test]
        public void Check_FullBoardWithNoLine_ReturnsDraw()
        {
            // Cat's game — every cell filled, no three-in-a-row anywhere.
            PlayerMark[] board = MakeBoard(
                X, O, X,
                X, O, O,
                O, X, X);

            WinResult result = WinConditionChecker.Check(board);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsDraw, Is.True);
            Assert.That(result.HasWinner, Is.False);
            Assert.That(result.IsGameOver, Is.True);
            Assert.That(result.WinLine, Is.Null);
        }

        [Test]
        public void Check_EmptyBoard_ReturnsNullForInProgress()
        {
            PlayerMark[] board = MakeBoard(
                _, _, _,
                _, _, _,
                _, _, _);

            WinResult result = WinConditionChecker.Check(board);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Check_PartialBoardWithNoLine_ReturnsNullForInProgress()
        {
            PlayerMark[] board = MakeBoard(
                X, O, _,
                _, X, _,
                _, _, _);

            WinResult result = WinConditionChecker.Check(board);

            Assert.That(result, Is.Null);
        }

        // ───── Defensive input handling ──────────────────────────────

        [Test]
        public void Check_NullBoard_ReturnsNull()
        {
            WinResult result = WinConditionChecker.Check(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Check_WrongLengthBoard_ReturnsNull()
        {
            PlayerMark[] board = new PlayerMark[5];

            WinResult result = WinConditionChecker.Check(board);

            Assert.That(result, Is.Null);
        }

        // ───── Helpers ───────────────────────────────────────────────

        private const PlayerMark _ = PlayerMark.None;
        private const PlayerMark X = PlayerMark.X;
        private const PlayerMark O = PlayerMark.O;

        private static PlayerMark[] MakeBoard(
            PlayerMark c0, PlayerMark c1, PlayerMark c2,
            PlayerMark c3, PlayerMark c4, PlayerMark c5,
            PlayerMark c6, PlayerMark c7, PlayerMark c8)
        {
            return new[] { c0, c1, c2, c3, c4, c5, c6, c7, c8 };
        }

        private static void AssertWin(WinResult result, PlayerMark expectedWinner, int[] expectedLine)
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasWinner, Is.True);
            Assert.That(result.IsDraw, Is.False);
            Assert.That(result.IsGameOver, Is.True);
            Assert.That(result.Winner, Is.EqualTo(expectedWinner));
            Assert.That(result.WinLine, Is.EqualTo(expectedLine));
        }
    }
}
