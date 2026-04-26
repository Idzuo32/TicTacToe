using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Play-scene popup that surfaces the cumulative <see cref="StatsData"/>
    /// persisted across sessions — games played, per-player wins, draws, and
    /// average match duration. Refreshes live when a match finishes while
    /// the popup happens to be open.
    /// </summary>
    /// <remarks>
    /// The popup subscribes to <see cref="SaveManager.OnStatsUpdated"/> on
    /// enable and uses the <see cref="PopupBase.IsOpen"/> flag to ignore
    /// updates that arrive while the popup is hidden — avoiding
    /// stale-refresh costs. All duration formatting mirrors
    /// <c>GameTimer.FormattedTime</c> so the player sees a single consistent
    /// time format everywhere.
    /// </remarks>
    public class StatsPopup : PopupBase
    {
        [Header("Stats Fields")]
        [Tooltip("Total games played across all sessions.")]
        [SerializeField] private TMP_Text _totalGamesLabel;

        [Tooltip("Total wins recorded for player 1 (X).")]
        [SerializeField] private TMP_Text _player1WinsLabel;

        [Tooltip("Total wins recorded for player 2 (O).")]
        [SerializeField] private TMP_Text _player2WinsLabel;

        [Tooltip("Total draws recorded.")]
        [SerializeField] private TMP_Text _drawsLabel;

        [Tooltip("Average match duration formatted as MM:SS.")]
        [SerializeField] private TMP_Text _averageDurationLabel;

        [Header("Navigation")]
        [Tooltip("Optional close button that dismisses the popup via PopupManager.")]
        [SerializeField] private Button _closeButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            SaveManager.OnStatsUpdated += HandleStatsUpdated;

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(HandleCloseClicked);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SaveManager.OnStatsUpdated -= HandleStatsUpdated;

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        /// <summary>
        /// Refresh every label from the currently-persisted stats snapshot.
        /// </summary>
        protected override void OnOpened() => Refresh(SaveManager.Instance?.Stats);

        private void HandleStatsUpdated(StatsData stats)
        {
            if (!IsOpen)
            {
                return;
            }

            Refresh(stats);
        }

        private void Refresh(StatsData stats)
        {
            if (stats == null)
            {
                return;
            }

            if (_totalGamesLabel != null)
            {
                _totalGamesLabel.text = Localizer.Format(LocalisationKeys.STATS_TOTAL_GAMES, stats.TotalGamesPlayed);
            }

            if (_player1WinsLabel != null)
            {
                _player1WinsLabel.text = Localizer.Format(LocalisationKeys.STATS_P1_WINS, stats.Player1Wins);
            }

            if (_player2WinsLabel != null)
            {
                _player2WinsLabel.text = Localizer.Format(LocalisationKeys.STATS_P2_WINS, stats.Player2Wins);
            }

            if (_drawsLabel != null)
            {
                _drawsLabel.text = Localizer.Format(LocalisationKeys.STATS_DRAWS, stats.Draws);
            }

            if (_averageDurationLabel != null)
            {
                _averageDurationLabel.text = Localizer.Format(LocalisationKeys.STATS_AVG_DURATION, TimeFormatter.FormatMMSS(stats.AverageDurationSeconds));
            }
        }

        private void HandleCloseClicked()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CloseTopPopup();
            }
        }
    }
}
