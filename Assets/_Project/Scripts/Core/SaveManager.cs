using UnityEngine;
using System;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that owns the live <see cref="StatsData"/> and
    /// <see cref="GameSettings"/> instances. Wraps <see cref="SaveSystem"/>
    /// so callers never deal with JSON directly and broadcasts change events
    /// so UI and audio systems can react without polling.
    /// </summary>
    /// <remarks>
    /// Loads on <c>Awake</c>. Saves automatically on every mutation and
    /// again on <c>OnApplicationPause(true)</c> as a belt-and-braces guard
    /// against Android process termination.
    /// </remarks>
    public class SaveManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static SaveManager Instance { get; private set; }

        /// <summary>Fires after stats change — typically after a match ends.</summary>
        public static event Action<StatsData> OnStatsUpdated;

        /// <summary>Fires once after the initial load — lets systems apply persisted settings.</summary>
        public static event Action<GameSettings> OnSettingsLoaded;

        /// <summary>Fires after any settings mutation (music, SFX, theme).</summary>
        public static event Action<GameSettings> OnSettingsChanged;

        /// <summary>Live stats object; mutate only through <see cref="RecordGameResult"/>.</summary>
        public StatsData Stats { get; private set; }

        /// <summary>Live settings object; mutate only through the Update* methods.</summary>
        public GameSettings Settings { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAll();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                SaveAll();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveAll();
            }
        }

        /// <summary>
        /// Read both saves from disk into <see cref="Stats"/> and
        /// <see cref="Settings"/>. Fires <see cref="OnSettingsLoaded"/>
        /// so dependent systems (audio, theme) can sync to persisted values.
        /// </summary>
        public void LoadAll()
        {
            Stats = SaveSystem.Load<StatsData>(StatsData.SAVE_KEY);
            Settings = SaveSystem.Load<GameSettings>(GameSettings.SAVE_KEY);

            OnSettingsLoaded?.Invoke(Settings);
        }

        /// <summary>
        /// Persist both saves to disk. Called automatically on pause / focus
        /// loss and after every mutation — rarely needs to be called manually.
        /// </summary>
        public void SaveAll()
        {
            if (Stats != null)
            {
                SaveSystem.Save(Stats, Stats.SaveKey);
            }

            if (Settings != null)
            {
                SaveSystem.Save(Settings, Settings.SaveKey);
            }
        }

        /// <summary>
        /// Update cumulative stats with the outcome of a completed match
        /// and persist the change. Fires <see cref="OnStatsUpdated"/> so
        /// the stats popup can refresh if it happens to be open.
        /// </summary>
        /// <param name="result">Outcome from <c>WinConditionChecker.Check</c>. Must be game-over.</param>
        /// <param name="durationSeconds">Match duration measured by <c>GameTimer</c>.</param>
        public void RecordGameResult(WinResult result, float durationSeconds)
        {
            if (result == null || !result.IsGameOver)
            {
                Debug.LogWarning("[SaveManager] RecordGameResult called without a finished result — ignored.");
                return;
            }

            Stats.TotalGamesPlayed++;
            Stats.TotalDurationSeconds += Mathf.Max(0f, durationSeconds);

            if (result.IsDraw)
            {
                Stats.Draws++;
            }
            else if (result.Winner == PlayerMark.X)
            {
                Stats.Player1Wins++;
            }
            else if (result.Winner == PlayerMark.O)
            {
                Stats.Player2Wins++;
            }

            SaveSystem.Save(Stats, Stats.SaveKey);
            OnStatsUpdated?.Invoke(Stats);
        }

        /// <summary>Toggle background music persistence. Notifies audio via <see cref="OnSettingsChanged"/>.</summary>
        public void UpdateMusicEnabled(bool enabled)
        {
            if (Settings.MusicEnabled == enabled)
            {
                return;
            }

            Settings.MusicEnabled = enabled;
            PersistSettings();
        }

        /// <summary>Toggle sound effects persistence. Notifies audio via <see cref="OnSettingsChanged"/>.</summary>
        public void UpdateSFXEnabled(bool enabled)
        {
            if (Settings.SFXEnabled == enabled)
            {
                return;
            }

            Settings.SFXEnabled = enabled;
            PersistSettings();
        }

        /// <summary>Persist the selected theme id. Notifies the theme manager via <see cref="OnSettingsChanged"/>.</summary>
        /// <param name="themeId">Must match a <c>ThemeSO.ThemeId</c> registered with <c>ThemeManager</c>.</param>
        public void UpdateSelectedTheme(string themeId)
        {
            if (string.IsNullOrEmpty(themeId) || Settings.SelectedThemeId == themeId)
            {
                return;
            }

            Settings.SelectedThemeId = themeId;
            PersistSettings();
        }

        private void PersistSettings()
        {
            SaveSystem.Save(Settings, Settings.SaveKey);
            OnSettingsChanged?.Invoke(Settings);
        }
    }
}
