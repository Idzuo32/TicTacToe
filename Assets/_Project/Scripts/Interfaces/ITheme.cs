namespace TicTacToe.Data
{
    /// <summary>
    /// Identity of a visual theme. Deliberately limited to id and display
    /// name so consumers that only need to identify or label a theme don't
    /// have to depend on board, UI, HUD, or audio data they never use —
    /// see <see cref="IThemeBoard"/>, <see cref="IThemeUI"/>,
    /// <see cref="IThemeHUD"/>, and <see cref="IThemeAudio"/> for the
    /// segregated visual surfaces a theme can supply.
    /// </summary>
    /// <remarks>
    /// <c>ThemeSO</c> implements all five theme interfaces. Systems should
    /// depend on the narrowest interface that satisfies their need
    /// (Interface Segregation Principle): the audio system depends on
    /// <see cref="IThemeAudio"/> only, the HUD on <see cref="IThemeHUD"/>
    /// only, and so on.
    /// </remarks>
    public interface ITheme
    {
        /// <summary>Stable identifier persisted in <c>GameSettings</c>.</summary>
        string ThemeId { get; }

        /// <summary>Human-readable name shown in the theme selection popup.</summary>
        string DisplayName { get; }
    }
}
