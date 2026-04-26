using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// Board-surface visuals supplied by a theme: the board background,
    /// the nine cells (with a checkerboard alternate slot for visual
    /// distinction), the cell pressed tint, and the X / O mark sprites
    /// and per-player tint colors.
    /// </summary>
    /// <remarks>
    /// <see cref="BoardController"/> and <see cref="CellView"/> depend on
    /// this interface only — they don't need anything from
    /// <see cref="IThemeUI"/>, <see cref="IThemeHUD"/>, or
    /// <see cref="IThemeAudio"/>. Sprite fields may be left null in the
    /// theme asset; in that case the matching <c>...Color</c> field is
    /// used as a flat fallback so themes can ship without bespoke art.
    /// </remarks>
    public interface IThemeBoard
    {
        /// <summary>Sprite for the board panel background, or null to use <see cref="BoardBackgroundColor"/>.</summary>
        Sprite BoardBackgroundSprite { get; }

        /// <summary>Flat color fallback for the board panel when no sprite is provided.</summary>
        Color BoardBackgroundColor { get; }

        /// <summary>Sprite for the default (non-alternate) cells, or null to use <see cref="CellDefaultColor"/>.</summary>
        Sprite CellDefaultSprite { get; }

        /// <summary>Flat color fallback for the default cells when no sprite is provided.</summary>
        Color CellDefaultColor { get; }

        /// <summary>Sprite for the alternate (checkerboard, indices 1/3/5/7) cells, or null to use <see cref="CellAlternateColor"/>.</summary>
        Sprite CellAlternateSprite { get; }

        /// <summary>Flat color fallback for the alternate cells when no sprite is provided.</summary>
        Color CellAlternateColor { get; }

        /// <summary>Pressed tint applied to every cell button's color block.</summary>
        Color CellPressedTint { get; }

        Sprite XSprite { get; }

        Sprite OSprite { get; }

        Color Player1Color { get; }

        Color Player2Color { get; }
    }
}
