using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Visual representation of a single board cell. Detects taps through
    /// its <see cref="Button"/>, renders the current mark via an
    /// <see cref="Image"/>, and reports input up to
    /// <see cref="BoardController"/> using the <see cref="_cellIndex"/>
    /// assigned in the Inspector.
    /// </summary>
    /// <remarks>
    /// The view holds no game logic of its own — it knows nothing about
    /// turns, win conditions, or game state. All validation happens in
    /// <see cref="BoardController.OnCellClicked"/>. The owning controller
    /// is injected through <see cref="Bind"/> in its <c>Awake</c> so this
    /// view never reaches out with <c>FindObjectOfType</c>.
    /// </remarks>
    public class CellView : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Zero-based cell index in [0..8], row-major (0 = top-left, 8 = bottom-right). Must be unique across the grid.")]
        [SerializeField] private int _cellIndex;

        [Header("Wiring")]
        [Tooltip("Image that displays the X or O sprite. Disabled until a mark is placed.")]
        [SerializeField] private Image _markImage;

        [Tooltip("Tap target for this cell. onClick is wired programmatically when the BoardController injects itself.")]
        [SerializeField] private Button _button;

        private BoardController _board;

        /// <summary>Zero-based board index assigned in the Inspector.</summary>
        public int CellIndex => _cellIndex;

        private void Awake()
        {
            if (_markImage != null)
            {
                _markImage.enabled = false;
            }
        }

        /// <summary>
        /// Injection entry point. Called by
        /// <see cref="BoardController.Awake"/> so cells never reach out
        /// with <c>FindObjectOfType</c>. Also wires the button's onClick
        /// handler once the owning controller is known.
        /// </summary>
        /// <param name="board">The controller that owns this cell.</param>
        public void Bind(BoardController board)
        {
            _board = board;

            if (_button == null)
            {
                return;
            }

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(HandleClicked);
        }

        /// <summary>
        /// Display the given mark using the supplied theme and lock the
        /// cell against further input. Safe to call repeatedly — also
        /// used by <see cref="BoardController"/> to reapply sprites after
        /// the active theme changes mid-match.
        /// </summary>
        /// <param name="mark">
        /// The mark to display. <see cref="PlayerMark.None"/> routes to
        /// <see cref="ResetCell"/> so callers don't have to branch.
        /// </param>
        /// <param name="theme">Active theme providing the X / O sprites and player colors.</param>
        public void SetMark(PlayerMark mark, ITheme theme)
        {
            if (mark == PlayerMark.None)
            {
                ResetCell();
                return;
            }

            if (_markImage != null && theme != null)
            {
                _markImage.sprite = mark == PlayerMark.X ? theme.XSprite : theme.OSprite;
                _markImage.color = mark == PlayerMark.X ? theme.Player1Color : theme.Player2Color;
                _markImage.enabled = true;
            }

            if (_button != null)
            {
                _button.interactable = false;
            }
        }

        /// <summary>
        /// Restore the cell to its empty, interactable state. Called by
        /// <see cref="BoardController.ResetBoard"/> on rematch.
        /// </summary>
        public void ResetCell()
        {
            if (_markImage != null)
            {
                _markImage.enabled = false;
                _markImage.sprite = null;
            }

            if (_button != null)
            {
                _button.interactable = true;
            }
        }

        /// <summary>
        /// Enable or disable the tap target without clearing the displayed
        /// mark. Used by <see cref="BoardController"/> to freeze the board
        /// on game over and unfreeze on rematch.
        /// </summary>
        /// <param name="interactable">True to accept taps; false to ignore them.</param>
        public void SetInteractable(bool interactable)
        {
            if (_button != null)
            {
                _button.interactable = interactable;
            }
        }

        private void HandleClicked()
        {
            if (_board == null)
            {
                return;
            }

            _board.OnCellClicked(_cellIndex);
        }
    }
}
