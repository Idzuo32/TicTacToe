using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// Visual theme contract. Implemented by <c>ThemeSO</c> so the rest of
    /// the project can depend on the abstraction rather than a concrete
    /// ScriptableObject. Adding a new theme requires zero code changes —
    /// only a new asset that satisfies this interface.
    /// </summary>
    public interface ITheme
    {
        /// <summary>Stable identifier persisted in <c>GameSettings</c>.</summary>
        string ThemeId { get; }

        /// <summary>Human-readable name shown in the theme selection popup.</summary>
        string DisplayName { get; }

        /// <summary>Sprite used to render the X mark on the board.</summary>
        Sprite XSprite { get; }

        /// <summary>Sprite used to render the O mark on the board.</summary>
        Sprite OSprite { get; }

        /// <summary>Sprite used as the backdrop behind the 3x3 grid.</summary>
        Sprite BoardBackground { get; }

        /// <summary>Tint applied to player 1's UI elements (HUD, score, mark).</summary>
        Color Player1Color { get; }

        /// <summary>Tint applied to player 2's UI elements (HUD, score, mark).</summary>
        Color Player2Color { get; }
    }
}
