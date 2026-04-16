namespace TicTacToe.UI
{
    /// <summary>
    /// Common contract for every popup in the project. <c>PopupBase</c>
    /// implements this interface; <c>PopupManager</c> drives popups through
    /// it so it never depends on concrete popup types.
    /// </summary>
    public interface IPopup
    {
        /// <summary>True while the popup is visible (post-open animation).</summary>
        bool IsOpen { get; }

        /// <summary>Activates the popup and plays its open animation.</summary>
        void Open();

        /// <summary>Plays the close animation; the popup deactivates when it ends.</summary>
        void Close();
    }
}
