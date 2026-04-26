using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// Audio supplied by a theme. Currently only the background music clip
    /// is theme-driven; sound effects remain owned by <c>AudioManager</c>'s
    /// global pool so theme switches don't reload UI SFX assets.
    /// </summary>
    /// <remarks>
    /// Consumed by <c>AudioManager</c>. <see cref="BGMClip"/> may be null —
    /// in that case <c>AudioManager</c> falls back to its default BGM clip
    /// so themes can opt out of overriding music without silencing the
    /// game.
    /// </remarks>
    public interface IThemeAudio
    {
        /// <summary>Theme-specific background music clip, or null to use the AudioManager's default.</summary>
        AudioClip BGMClip { get; }
    }
}
