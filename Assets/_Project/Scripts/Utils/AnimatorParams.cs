namespace TicTacToe
{
    /// <summary>
    /// Canonical Animator parameter and trigger names. Centralised here so
    /// renaming a parameter in the Animator surfaces in exactly one place
    /// rather than scattered <c>SetTrigger("...")</c> calls across the UI.
    /// </summary>
    public static class AnimatorParams
    {
        /// <summary>Trigger fired by <c>PopupBase.Open</c> to play the open animation.</summary>
        public const string POPUP_OPEN = "PopupOpen";

        /// <summary>Trigger fired by <c>PopupBase.Close</c> to play the close animation.</summary>
        public const string POPUP_CLOSE = "PopupClose";

        /// <summary>Trigger fired by <c>StrikeAnimator</c> to play the winning-line strike.</summary>
        public const string STRIKE_PLAY = "StrikePlay";
    }
}
