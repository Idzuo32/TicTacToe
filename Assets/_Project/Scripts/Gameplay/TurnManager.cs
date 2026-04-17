using UnityEngine;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Tracks whose turn it is and rotates between Player 1 (X) and
    /// Player 2 (O). Announces every rotation through
    /// <see cref="GameManager.OnTurnChanged"/> via <c>GameManager</c>'s
    /// report method so the event bus stays centralised.
    /// </summary>
    /// <remarks>
    /// Knows nothing about the board, win conditions, or UI — its entire
    /// responsibility is answering "who plays next?" in response to
    /// <see cref="NextTurn"/> calls issued by <c>BoardController</c>.
    /// </remarks>
    public class TurnManager : MonoBehaviour
    {
        private const int PLAYER_ONE = 1;
        private const int PLAYER_TWO = 2;

        /// <summary>The active player number — 1 or 2.</summary>
        public int CurrentPlayer { get; private set; } = PLAYER_ONE;

        /// <summary>The mark the active player will place on their next move.</summary>
        public PlayerMark CurrentMark { get; private set; } = PlayerMark.X;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterTurnManager(this);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnregisterTurnManager(this);
            }
        }

        /// <summary>
        /// Rotate to the other player and broadcast the change through
        /// <c>GameManager</c>. Call after a mark has been successfully
        /// placed and the board was not won or drawn.
        /// </summary>
        public void NextTurn()
        {
            if (CurrentPlayer == PLAYER_ONE)
            {
                CurrentPlayer = PLAYER_TWO;
                CurrentMark = PlayerMark.O;
            }
            else
            {
                CurrentPlayer = PLAYER_ONE;
                CurrentMark = PlayerMark.X;
            }

            BroadcastTurnChanged();
        }

        /// <summary>
        /// Reset to the starting state — Player 1, mark X — and broadcast
        /// the change so HUD elements clear and realign. Called on scene
        /// load and on <see cref="GameManager.OnGameRestarted"/>.
        /// </summary>
        public void ResetTurns()
        {
            CurrentPlayer = PLAYER_ONE;
            CurrentMark = PlayerMark.X;
            BroadcastTurnChanged();
        }

        private void BroadcastTurnChanged()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.ReportTurnChanged(CurrentPlayer);
        }
    }
}
