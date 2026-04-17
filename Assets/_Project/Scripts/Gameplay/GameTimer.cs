using UnityEngine;
using System;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Per-match stopwatch. Accumulates elapsed seconds in
    /// <see cref="Update"/> while running and fires
    /// <see cref="OnTimerUpdated"/> at most once per whole second with a
    /// pre-formatted <c>MM:SS</c> string so UI consumers never have to
    /// format values themselves.
    /// </summary>
    /// <remarks>
    /// The per-second fan-out is deliberate: firing every frame would
    /// push redundant text updates to the HUD and cost allocations on
    /// mobile. A separate second-accumulator avoids comparing truncated
    /// integers, which would mis-fire when a frame straddles a boundary.
    /// Self-registers with <see cref="GameManager"/> so match completion
    /// can read <see cref="ElapsedSeconds"/> for save records, and listens
    /// to <see cref="GameManager.OnGameRestarted"/> / <see cref="GameManager.OnGameOver"/>
    /// to start and stop without a polling scheme.
    /// </remarks>
    public class GameTimer : MonoBehaviour
    {
        private const float ONE_SECOND = 1f;

        /// <summary>Total elapsed seconds since the last reset.</summary>
        public float ElapsedSeconds { get; private set; }

        /// <summary>Current elapsed time formatted as <c>MM:SS</c>.</summary>
        public string FormattedTime => Format(ElapsedSeconds);

        /// <summary>
        /// Fires once per whole second of elapsed time (and once on
        /// <see cref="ResetTimer"/>) with the pre-formatted display string.
        /// </summary>
        public static event Action<string> OnTimerUpdated;

        private bool _isRunning;
        private float _secondAccumulator;

        private void OnEnable()
        {
            GameManager.OnGameRestarted += HandleGameRestarted;
            GameManager.OnGameOver += HandleGameOver;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterGameTimer(this);
            }
        }

        private void OnDisable()
        {
            GameManager.OnGameRestarted -= HandleGameRestarted;
            GameManager.OnGameOver -= HandleGameOver;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnregisterGameTimer(this);
            }
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            float delta = Time.deltaTime;
            ElapsedSeconds += delta;
            _secondAccumulator += delta;

            while (_secondAccumulator >= ONE_SECOND)
            {
                _secondAccumulator -= ONE_SECOND;
                OnTimerUpdated?.Invoke(FormattedTime);
            }
        }

        /// <summary>Begin accumulating elapsed time. Idempotent.</summary>
        public void StartTimer() => _isRunning = true;

        /// <summary>Pause accumulation. Elapsed time is preserved.</summary>
        public void StopTimer() => _isRunning = false;

        /// <summary>
        /// Reset elapsed time to zero and broadcast the cleared display
        /// so HUD text updates to <c>00:00</c>. Does not start or stop
        /// the timer.
        /// </summary>
        public void ResetTimer()
        {
            ElapsedSeconds = 0f;
            _secondAccumulator = 0f;
            OnTimerUpdated?.Invoke(FormattedTime);
        }

        private void HandleGameRestarted()
        {
            ResetTimer();
            StartTimer();
        }

        private void HandleGameOver(WinResult _) => StopTimer();

        private static string Format(float seconds)
        {
            int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
            int minutes = total / 60;
            int remainder = total % 60;
            return $"{minutes:00}:{remainder:00}";
        }
    }
}
