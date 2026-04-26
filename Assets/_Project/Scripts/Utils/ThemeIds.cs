namespace TicTacToe
{
    /// <summary>
    /// Canonical theme identifiers. Centralised so a renamed asset id
    /// surfaces as a compile error rather than as a silent fallback to
    /// the default theme at runtime.
    /// </summary>
    public static class ThemeIds
    {
        public const string DEFAULT = "Classic";

        public const string CLASSIC = "Classic";

        public const string NEON = "Neon";

        public const string NATURE = "Nature";
    }
}
