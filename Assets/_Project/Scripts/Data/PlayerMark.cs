namespace TicTacToe.Data
{
    /// <summary>
    /// The contents of a single board cell. <see cref="None"/> represents
    /// an empty cell — using a dedicated enum value rather than nullable
    /// keeps the board array allocation-free and switch-exhaustive.
    /// </summary>
    public enum PlayerMark
    {
        /// <summary>Empty cell, no mark placed.</summary>
        None,

        /// <summary>Player 1's mark.</summary>
        X,

        /// <summary>Player 2's mark.</summary>
        O
    }
}
