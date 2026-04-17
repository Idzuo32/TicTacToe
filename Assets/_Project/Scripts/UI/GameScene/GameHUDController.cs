using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;
using TicTacToe.UI;

namespace TicTacToe
{
    /// <summary>
    /// Game scene HUD. Mirrors <see cref="GameManager"/> and
    /// <see cref="GameTimer"/> state onto the on-screen labels — current
    /// player indicator, per-player move counts, and the match timer — and
    /// exposes the settings button that pushes the shared settings popup
    /// onto <see cref="PopupManager"/>.
    /// </summary>
    /// <remarks>
    /// Strictly a listener. Does not mutate game state. The only outbound
    /// call is <see cref="PopupManager.OpenPopup"/> from the settings button,
    /// which is a UI-side concern. Move counts are tracked locally from
    /// <see cref="GameManager.OnMarkPlaced"/> so the HUD stays independent
    /// of any board-state API.
    /// </remarks>
    public class GameHUDController : MonoBehaviour
    {
        [Header("Current Turn")]
        [Tooltip("Label that shows whose turn it is. Copy is set from the active PlayerMark.")]
        [SerializeField] private TMP_Text _currentPlayerLabel;

        [Header("Move Counts")]
        [Tooltip("Player 1 (X) running move count for the current match.")]
        [SerializeField] private TMP_Text _player1MoveCountLabel;

        [Tooltip("Player 2 (O) running move count for the current match.")]
        [SerializeField] private TMP_Text _player2MoveCountLabel;

        [Header("Timer")]
        [Tooltip("Match timer display updated from GameTimer.OnTimerUpdated. Pre-formatted MM:SS.")]
        [SerializeField] private TMP_Text _timerLabel;

        [Header("Settings")]
        [Tooltip("Button that opens the shared settings popup via PopupManager.")]
        [SerializeField] private Button _settingsButton;

        [Tooltip("Settings popup instance present in this scene. Pushed onto the popup stack when the settings button is tapped.")]
        [SerializeField] private PopupBase _settingsPopup;

        private int _player1MoveCount;
        private int _player2MoveCount;

        private void OnEnable()
        {
            GameManager.OnTurnChanged += HandleTurnChanged;
            GameManager.OnMarkPlaced += HandleMarkPlaced;
            GameManager.OnGameOver += HandleGameOver;
            GameManager.OnGameRestarted += HandleGameRestarted;
            GameTimer.OnTimerUpdated += HandleTimerUpdated;

            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(HandleSettingsClicked);
            }

            ResetDisplays();
        }

        private void OnDisable()
        {
            GameManager.OnTurnChanged -= HandleTurnChanged;
            GameManager.OnMarkPlaced -= HandleMarkPlaced;
            GameManager.OnGameOver -= HandleGameOver;
            GameManager.OnGameRestarted -= HandleGameRestarted;
            GameTimer.OnTimerUpdated -= HandleTimerUpdated;

            if (_settingsButton != null)
            {
                _settingsButton.onClick.RemoveListener(HandleSettingsClicked);
            }
        }

        private void HandleTurnChanged(int playerNumber)
        {
            if (_currentPlayerLabel == null)
            {
                return;
            }

            _currentPlayerLabel.text = playerNumber == 1 ? "Player 1 (X)" : "Player 2 (O)";
        }

        private void HandleMarkPlaced(PlayerMark mark)
        {
            if (mark == PlayerMark.X)
            {
                _player1MoveCount++;
                UpdateMoveLabel(_player1MoveCountLabel, _player1MoveCount);
            }
            else if (mark == PlayerMark.O)
            {
                _player2MoveCount++;
                UpdateMoveLabel(_player2MoveCountLabel, _player2MoveCount);
            }
        }

        private void HandleGameOver(WinResult result)
        {
            if (_currentPlayerLabel == null || result == null)
            {
                return;
            }

            if (result.IsDraw)
            {
                _currentPlayerLabel.text = "Draw";
            }
            else if (result.Winner == PlayerMark.X)
            {
                _currentPlayerLabel.text = "Player 1 Wins";
            }
            else if (result.Winner == PlayerMark.O)
            {
                _currentPlayerLabel.text = "Player 2 Wins";
            }
        }

        private void HandleGameRestarted() => ResetDisplays();

        private void HandleTimerUpdated(string formattedTime)
        {
            if (_timerLabel != null)
            {
                _timerLabel.text = formattedTime;
            }
        }

        private void HandleSettingsClicked()
        {
            if (PopupManager.Instance == null || _settingsPopup == null)
            {
                return;
            }

            PopupManager.Instance.OpenPopup(_settingsPopup);
        }

        private void ResetDisplays()
        {
            _player1MoveCount = 0;
            _player2MoveCount = 0;

            UpdateMoveLabel(_player1MoveCountLabel, 0);
            UpdateMoveLabel(_player2MoveCountLabel, 0);

            if (_timerLabel != null)
            {
                _timerLabel.text = "00:00";
            }

            if (_currentPlayerLabel != null)
            {
                _currentPlayerLabel.text = "Player 1 (X)";
            }
        }

        private static void UpdateMoveLabel(TMP_Text label, int count)
        {
            if (label != null)
            {
                label.text = count.ToString();
            }
        }
    }
}
