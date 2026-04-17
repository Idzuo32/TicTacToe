using UnityEngine;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Activates the pre-positioned strike line that matches the winning
    /// line reported by <see cref="WinConditionChecker"/>. Eight
    /// <see cref="GameObject"/> slots mirror the eight possible win lines
    /// (three rows, three columns, two diagonals); exactly one is activated
    /// per win and optionally drives an <see cref="Animator"/> trigger for
    /// the reveal animation.
    /// </summary>
    /// <remarks>
    /// Holds no logic of its own — matches the incoming cell indices to the
    /// canonical win-line table and flips the matching slot's
    /// <see cref="GameObject.activeSelf"/>. Pre-positioning lives in the
    /// editor: each line is a child GameObject placed over the correct row,
    /// column, or diagonal and left inactive at authoring time.
    /// </remarks>
    public class StrikeAnimator : MonoBehaviour
    {
        /// <summary>
        /// Canonical win-line table in the same order as
        /// <c>WinConditionChecker.WIN_LINES</c>. The index into this array
        /// is the index into <see cref="_strikeLines"/>.
        /// </summary>
        private static readonly int[][] WIN_LINES =
        {
            new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, // rows
            new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, // columns
            new[] { 0, 4, 8 }, new[] { 2, 4, 6 }                    // diagonals
        };

        [Header("Wiring")]
        [Tooltip("Eight pre-positioned strike line GameObjects — rows 0-2, columns 0-2, diagonals 0-1. Assigned in the Inspector, all inactive by default.")]
        [SerializeField] private GameObject[] _strikeLines;

        private void OnEnable()
        {
            GameManager.OnGameOver += HandleGameOver;
            GameManager.OnGameRestarted += HandleGameRestarted;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= HandleGameOver;
            GameManager.OnGameRestarted -= HandleGameRestarted;
        }

        /// <summary>
        /// Reveal the strike line that matches <paramref name="winLine"/>.
        /// Resets any previously active line first so back-to-back wins on
        /// the same scene show only the current result.
        /// </summary>
        /// <param name="winLine">The three cell indices from <c>WinResult.WinLine</c>.</param>
        public void PlayStrike(int[] winLine)
        {
            if (winLine == null || winLine.Length != 3)
            {
                return;
            }

            if (_strikeLines == null || _strikeLines.Length != WIN_LINES.Length)
            {
                Debug.LogError($"[StrikeAnimator] _strikeLines must contain exactly {WIN_LINES.Length} GameObjects. Populate in the Inspector.");
                return;
            }

            ResetStrike();

            int lineIndex = FindLineIndex(winLine);
            if (lineIndex < 0)
            {
                Debug.LogWarning("[StrikeAnimator] Received a win line that does not match any canonical line.");
                return;
            }

            GameObject strike = _strikeLines[lineIndex];
            if (strike == null)
            {
                return;
            }

            strike.SetActive(true);

            Animator animator = strike.GetComponent<Animator>();
            if (animator != null)
            {
                animator.ResetTrigger(AnimatorParams.STRIKE_PLAY);
                animator.SetTrigger(AnimatorParams.STRIKE_PLAY);
            }
        }

        /// <summary>
        /// Deactivate every strike line so the next match starts clean.
        /// Called on restart and before activating a new line.
        /// </summary>
        public void ResetStrike()
        {
            if (_strikeLines == null)
            {
                return;
            }

            for (int i = 0; i < _strikeLines.Length; i++)
            {
                if (_strikeLines[i] != null)
                {
                    _strikeLines[i].SetActive(false);
                }
            }
        }

        private void HandleGameOver(WinResult result)
        {
            if (result == null || !result.HasWinner || result.WinLine == null)
            {
                return;
            }

            PlayStrike(result.WinLine);
        }

        private void HandleGameRestarted() => ResetStrike();

        private static int FindLineIndex(int[] winLine)
        {
            for (int i = 0; i < WIN_LINES.Length; i++)
            {
                int[] candidate = WIN_LINES[i];
                if (candidate[0] == winLine[0] && candidate[1] == winLine[1] && candidate[2] == winLine[2])
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
