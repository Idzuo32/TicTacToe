namespace TicTacToe
{
    /// <summary>
    /// Canonical scene name constants. All <c>SceneManager.LoadScene</c>
    /// calls must reference these — never a raw string literal — so a
    /// scene rename surfaces as a compile error rather than a runtime crash.
    /// </summary>
    public static class SceneNames
    {
        /// <summary>The main menu scene; entry point and home of persistent managers.</summary>
        public const string PlayScene = "PlayScene";

        /// <summary>The active match scene loaded when the player starts a game.</summary>
        public const string GameScene = "GameScene";
    }
}
