using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// HUD visuals supplied by a theme: the in-game text color, the
    /// Settings indicator color, and the strike-line color drawn
    /// on a winning row, column, or diagonal.
    /// </summary>
    /// <remarks>
    /// Consumed by <c>HUDThemeApplier</c> and <c>StrikeAnimator</c>. The
    /// HUD never reads board, audio, or popup data — keeping the surface
    /// this small lets HUD widgets be moved or restyled without dragging
    /// in unrelated theme concerns.
    /// </remarks>
    public interface IThemeHUD
    {
        /// <summary>Tint applied to HUD text labels (timer, move counts, settings).</summary>
        Color HUDTextColor { get; }

        Color SettingsIndicatorColor { get; }

        /// <summary>Tint applied to the strike line drawn through a winning row/column/diagonal.</summary>
        Color StrikeColor { get; }
    }
}
