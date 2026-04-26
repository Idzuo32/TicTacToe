using UnityEngine;

namespace TicTacToe.Data
{
    /// <summary>
    /// Scene-chrome visuals supplied by a theme: the scene background,
    /// every Button's normal/pressed sprite and text color, and every
    /// popup's background sprite and text colors.
    /// </summary>
    /// <remarks>
    /// Consumed by <c>SceneBackgroundController</c>, <c>ButtonThemeApplier</c>,
    /// and <c>PopupBase</c>. Sprite fields that can be omitted in a theme
    /// asset are documented inline so authors know which slots are
    /// optional. Listeners depend on this interface only and have no
    /// access to board, HUD, or audio data.
    /// </remarks>
    public interface IThemeUI
    {
        /// <summary>Full-screen scene background sprite, or null to use <see cref="SceneBackgroundColor"/>.</summary>
        Sprite SceneBackgroundSprite { get; }

        /// <summary>Flat color fallback for the scene background when no sprite is provided.</summary>
        Color SceneBackgroundColor { get; }

        Sprite ButtonNormalSprite { get; }

        /// <summary>Pressed / highlighted sprite applied to every themed Button.</summary>
        Sprite ButtonPressedSprite { get; }

        Color ButtonTextColor { get; }

        Sprite PopupBackgroundSprite { get; }

        Color PopupTitleColor { get; }

        Color PopupBodyColor { get; }
    }
}
