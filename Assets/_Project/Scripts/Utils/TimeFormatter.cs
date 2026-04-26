using UnityEngine;

namespace TicTacToe
{
    /// <summary>
    /// Stateless time-formatting helpers shared by the gameplay HUD, the
    /// stats popup, and the result popup so a single format owns every
    /// duration the player sees on screen.
    /// </summary>
    public static class TimeFormatter
    {
        private const int SECONDS_PER_MINUTE = 60;

        /// <summary>
        /// Format <paramref name="seconds"/> as a zero-padded <c>MM:SS</c>
        /// string. Negative inputs clamp to zero so the display never shows
        /// odd values when a timer races past a stop.
        /// </summary>
        public static string FormatMMSS(float seconds)
        {
            int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
            int minutes = total / SECONDS_PER_MINUTE;
            int remainder = total % SECONDS_PER_MINUTE;
            return $"{minutes:00}:{remainder:00}";
        }
    }
}
