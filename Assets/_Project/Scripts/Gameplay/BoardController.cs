using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Owns the nine-cell board. Receives taps from <see cref="CellView"/>,
    /// validates them against <see cref="GameManager.CurrentState"/>,
    /// writes the current player's mark into the backing array, asks
    /// <see cref="WinConditionChecker"/> for the outcome, and routes
    /// results back through <see cref="GameManager"/>'s event bus.
    /// </summary>
    /// <remarks>
    /// Does not decide whose turn it is — that belongs to
    /// <see cref="TurnManager"/>. Does not evaluate win lines — that
    /// belongs to <see cref="WinConditionChecker"/>. Does not render cells
    /// — that belongs to <see cref="CellView"/>. The controller's sole
    /// responsibility is coordinating those three collaborators around
    /// the canonical board state held in <see cref="_board"/>.
    /// </remarks>
    public class BoardController : MonoBehaviour
    {
        private const int BOARD_SIZE = 9;

        [Header("Wiring")]
        [Tooltip("Exactly nine cells in row-major order (0 = top-left, 8 = bottom-right). Assigned in the Inspector.")]
        [SerializeField] private CellView[] _cells;

        [Tooltip("Turn rotator for this match. Assigned in the Inspector on the GameSystems GameObject.")]
        [SerializeField] private TurnManager _turnManager;

        [Header("Theming")]
        [Tooltip("Image rendering the board panel background. Tinted/sprited from the active IThemeBoard.")]
        [SerializeField] private Image _boardBackgroundImage;

        private readonly PlayerMark[] _board = new PlayerMark[BOARD_SIZE];

        private void Awake()
        {
            if (_cells == null || _cells.Length != BOARD_SIZE)
            {
                Debug.LogError($"[BoardController] _cells must contain exactly {BOARD_SIZE} CellViews. Populate in the Inspector.");
                return;
            }

            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i] != null)
                {
                    _cells[i].Bind(this);
                }
            }
        }

        private void OnEnable()
        {
            GameManager.OnGameRestarted += HandleGameRestarted;
            ThemeManager.OnThemeChanged += HandleThemeChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameRestarted -= HandleGameRestarted;
            ThemeManager.OnThemeChanged -= HandleThemeChanged;
        }

        private void Start()
        {
            // Apply the current theme as soon as ThemeManager is reachable
            // — Awake/OnEnable fire before scene-singleton wiring is
            // guaranteed, but Start runs after every Awake/OnEnable so
            // ThemeManager.Instance and ActiveThemeBoard are reliably
            // populated by the time we get here.
            ApplyCurrentTheme();
        }

        /// <summary>
        /// Entry point for cell taps. Validates that the match is live
        /// and the cell is empty, commits the active player's mark,
        /// reports the placement, then either announces the game-over
        /// outcome or hands control to the next turn.
        /// </summary>
        /// <param name="cellIndex">Zero-based cell index in [0..8].</param>
        public void OnCellClicked(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= BOARD_SIZE)
            {
                return;
            }

            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            {
                return;
            }

            if (!IsCellEmpty(cellIndex))
            {
                return;
            }

            if (_turnManager == null)
            {
                Debug.LogError("[BoardController] _turnManager not assigned. Wire it in the Inspector.");
                return;
            }

            PlayerMark mark = _turnManager.CurrentMark;
            _board[cellIndex] = mark;
            _cells[cellIndex].SetMark(mark, GetActiveThemeBoard());

            GameManager.Instance.ReportMarkPlaced(mark);
            AudioManager.Instance?.PlayPlacement();

            WinResult result = WinConditionChecker.Check(_board);
            if (result != null && result.IsGameOver)
            {
                SetAllInteractable(false);
                GameManager.Instance.ReportGameOver(result);
                if (result.HasWinner)
                {
                    AudioManager.Instance?.PlayWin();
                }
                return;
            }

            _turnManager.NextTurn();
        }

        /// <summary>
        /// Clear the board state and restore every cell to its empty,
        /// interactable appearance. Invoked when
        /// <see cref="GameManager.OnGameRestarted"/> fires so the same
        /// scene can host back-to-back matches without a reload.
        /// </summary>
        public void ResetBoard()
        {
            for (int i = 0; i < _board.Length; i++)
            {
                _board[i] = PlayerMark.None;
            }

            if (_cells == null)
            {
                return;
            }

            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i] == null)
                {
                    continue;
                }

                _cells[i].ResetCell();
                _cells[i].SetInteractable(true);
            }
        }

        /// <summary>Returns the mark currently occupying the given cell.</summary>
        /// <param name="index">Zero-based cell index in [0..8].</param>
        /// <returns>The mark at <paramref name="index"/>, or <see cref="PlayerMark.None"/> when out of range.</returns>
        public PlayerMark GetCell(int index)
        {
            if (index < 0 || index >= _board.Length)
            {
                return PlayerMark.None;
            }

            return _board[index];
        }

        /// <summary>True when no mark has been placed on the given cell.</summary>
        /// <param name="index">Zero-based cell index in [0..8].</param>
        public bool IsCellEmpty(int index) => GetCell(index) == PlayerMark.None;

        private void HandleGameRestarted() => ResetBoard();

        /// <summary>
        /// Re-skin the entire board on a theme switch. The event payload
        /// is intentionally ignored — board visuals come from the
        /// segregated <see cref="IThemeBoard"/> surface, not from
        /// <see cref="ITheme"/>.
        /// </summary>
        private void HandleThemeChanged(ITheme _) => ApplyCurrentTheme();

        private void ApplyCurrentTheme()
        {
            IThemeBoard themeBoard = GetActiveThemeBoard();
            if (themeBoard == null || _cells == null)
            {
                return;
            }

            ApplyBoardBackground(themeBoard);

            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i] == null)
                {
                    continue;
                }

                _cells[i].ApplyTheme(themeBoard, IsAlternateCellIndex(i));

                if (_board[i] != PlayerMark.None)
                {
                    _cells[i].SetMark(_board[i], themeBoard);
                }
            }
        }

        private void ApplyBoardBackground(IThemeBoard themeBoard)
        {
            if (_boardBackgroundImage == null)
            {
                return;
            }

            if (themeBoard.BoardBackgroundSprite != null)
            {
                _boardBackgroundImage.sprite = themeBoard.BoardBackgroundSprite;
                _boardBackgroundImage.color = Color.white;
            }
            else
            {
                _boardBackgroundImage.sprite = null;
                _boardBackgroundImage.color = themeBoard.BoardBackgroundColor;
            }
        }

        private void SetAllInteractable(bool interactable)
        {
            if (_cells == null)
            {
                return;
            }

            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i] != null)
                {
                    _cells[i].SetInteractable(interactable);
                }
            }
        }

        private static bool IsAlternateCellIndex(int index) => index % 2 != 0;

        private IThemeBoard GetActiveThemeBoard()
        {
            return ThemeManager.Instance != null ? ThemeManager.Instance.ActiveThemeBoard : null;
        }
    }
}
