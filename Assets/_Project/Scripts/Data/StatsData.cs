using System;

namespace TicTacToe.Data
{
    /// <summary>
    /// Aggregate match statistics persisted across sessions. Serialised by
    /// <c>SaveSystem</c> as <c>stats.json</c>. Public fields (rather than
    /// properties) are required for <c>JsonUtility</c> compatibility.
    /// </summary>
    [Serializable]
    public class StatsData : ISaveable
    {
        /// <summary>Total number of completed matches (wins + draws).</summary>
        public int TotalGamesPlayed;

        /// <summary>Total wins recorded for player 1 (X).</summary>
        public int Player1Wins;

        /// <summary>Total wins recorded for player 2 (O).</summary>
        public int Player2Wins;

        public int Draws;

        /// <summary>Cumulative match duration in seconds across all matches.</summary>
        public float TotalDurationSeconds;

        /// <summary>On-disk save key for this data class.</summary>
        public const string SAVE_KEY = "stats";

        /// <inheritdoc />
        public string SaveKey => SAVE_KEY;

        /// <summary>
        /// Average match duration in seconds, or zero when no matches have
        /// been played yet. Computed on demand to avoid stale derived state.
        /// </summary>
        public float AverageDurationSeconds =>
            TotalGamesPlayed > 0 ? TotalDurationSeconds / TotalGamesPlayed : 0f;
    }
}
