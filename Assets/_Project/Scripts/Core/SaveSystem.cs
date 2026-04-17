using UnityEngine;
using System;
using System.IO;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Low-level JSON persistence helper. Reads and writes
    /// <see cref="ISaveable"/> instances as <c>{key}.json</c> files on
    /// <c>Application.persistentDataPath</c>. Has no knowledge of the
    /// domain objects themselves — callers are responsible for owning
    /// the data and deciding when to persist it.
    /// </summary>
    public static class SaveSystem
    {
        private const string FILE_EXTENSION = ".json";

        /// <summary>
        /// Serialise <paramref name="data"/> to JSON and write it atomically
        /// to disk using the provided <paramref name="key"/> as the filename.
        /// Writes to a temp file first, then replaces the target — a crash
        /// mid-write cannot leave a half-written save on disk.
        /// </summary>
        /// <typeparam name="T">Concrete saveable type; must be <c>[Serializable]</c>.</typeparam>
        /// <param name="data">Instance to persist. Must not be null.</param>
        /// <param name="key">Filename (without extension). Typically <c>data.SaveKey</c>.</param>
        public static void Save<T>(T data, string key) where T : ISaveable
        {
            if (data == null)
            {
                Debug.LogError($"[SaveSystem] Refusing to save null data for key '{key}'.");
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[SaveSystem] Refusing to save with empty key.");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                string targetPath = GetPath(key);
                string tempPath = targetPath + ".tmp";

                File.WriteAllText(tempPath, json);

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Move(tempPath, targetPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to save '{key}': {e.Message}");
            }
        }

        /// <summary>
        /// Read the JSON file for <paramref name="key"/> and deserialise it
        /// into a new <typeparamref name="T"/>. If the file does not exist
        /// or cannot be read, a default-constructed <typeparamref name="T"/>
        /// is returned — callers never receive null.
        /// </summary>
        /// <typeparam name="T">Concrete saveable type with a parameterless constructor.</typeparam>
        /// <param name="key">Filename (without extension) to load.</param>
        /// <returns>Loaded instance, or a fresh default instance on miss or error.</returns>
        public static T Load<T>(string key) where T : ISaveable, new()
        {
            string path = GetPath(key);

            if (!File.Exists(path))
            {
                return new T();
            }

            try
            {
                string json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new T();
                }

                T data = JsonUtility.FromJson<T>(json);
                return data ?? new T();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to load '{key}': {e.Message}. Returning defaults.");
                return new T();
            }
        }

        /// <summary>
        /// Returns true when a save file exists on disk for the given key.
        /// Does not validate the contents — only that the file is present.
        /// </summary>
        public static bool Exists(string key) => File.Exists(GetPath(key));

        /// <summary>
        /// Delete the save file for the given key if it exists. Silent no-op
        /// when the file is already absent.
        /// </summary>
        public static void Delete(string key)
        {
            string path = GetPath(key);
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to delete '{key}': {e.Message}");
            }
        }

        private static string GetPath(string key) =>
            Path.Combine(Application.persistentDataPath, key + FILE_EXTENSION);
    }
}
