namespace TicTacToe.Data
{
    /// <summary>
    /// Marker interface for any data type that can be persisted to disk
    /// by <c>SaveSystem</c>. The <see cref="SaveKey"/> is used as the
    /// JSON file name on <c>Application.persistentDataPath</c>.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Stable identifier used as the on-disk filename (without extension).
        /// Must be unique across all saveable types and stable across versions.
        /// </summary>
        string SaveKey { get; }
    }
}
