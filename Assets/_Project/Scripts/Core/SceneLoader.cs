using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace TicTacToe
{
    /// <summary>
    /// Thin static wrapper over <see cref="SceneManager"/> that routes every
    /// scene transition through <see cref="SceneNames"/> constants. Exposes
    /// lifecycle events so a loading screen can fade in and out without this
    /// class depending on any UI — the loading screen itself is a stub until
    /// the UI pass wires one up.
    /// </summary>
    public static class SceneLoader
    {
        /// <summary>True while an async scene load is in flight.</summary>
        public static bool IsLoading { get; private set; }

        /// <summary>Fires immediately before the target scene begins loading; carries the scene name.</summary>
        public static event Action<string> OnSceneLoadStarted;

        /// <summary>Fires when the target scene finishes loading and is active.</summary>
        public static event Action<string> OnSceneLoadCompleted;

        /// <summary>Fires on every frame during load; carries progress in [0, 1].</summary>
        public static event Action<float> OnSceneLoadProgress;

        /// <summary>Load the main menu scene asynchronously.</summary>
        /// <returns>The underlying <see cref="AsyncOperation"/> for callers that want to await it.</returns>
        public static AsyncOperation LoadPlayScene() => LoadSceneAsync(SceneNames.PlayScene);

        /// <summary>Load the match scene asynchronously.</summary>
        /// <returns>The underlying <see cref="AsyncOperation"/> for callers that want to await it.</returns>
        public static AsyncOperation LoadGameScene() => LoadSceneAsync(SceneNames.GameScene);

        private static AsyncOperation LoadSceneAsync(string sceneName)
        {
            // Reject concurrent loads outright. Spawning a second runner
            // while the first is still in flight would orphan the first
            // [SceneLoaderRunner] GameObject under DontDestroyOnLoad.
            if (IsLoading)
            {
                Debug.LogWarning($"[SceneLoader] Ignoring load request for '{sceneName}' — another load is already in flight.");
                return null;
            }

            IsLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                Debug.LogError($"[SceneLoader] LoadSceneAsync returned null for '{sceneName}'. Is it in Build Settings?");
                IsLoading = false;
                return null;
            }

            LoadingScreenStubRunner.Run(operation, sceneName);
            return operation;
        }

        internal static void ReportProgress(float progress) => OnSceneLoadProgress?.Invoke(progress);

        internal static void ReportCompleted(string sceneName)
        {
            IsLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
        }

        /// <summary>
        /// Hidden MonoBehaviour that polls the <see cref="AsyncOperation"/>
        /// each frame so <see cref="SceneLoader"/> can stay a pure static
        /// class. Replace with a real loading-screen controller once the
        /// UI for it exists.
        /// </summary>
        private class LoadingScreenStubRunner : MonoBehaviour
        {
            private AsyncOperation _operation;
            private string _sceneName;

            public static void Run(AsyncOperation operation, string sceneName)
            {
                var host = new GameObject("[SceneLoaderRunner]");
                DontDestroyOnLoad(host);
                var runner = host.AddComponent<LoadingScreenStubRunner>();
                runner._operation = operation;
                runner._sceneName = sceneName;
            }

            private void Update()
            {
                if (_operation == null)
                {
                    Destroy(gameObject);
                    return;
                }

                ReportProgress(_operation.progress);

                if (_operation.isDone)
                {
                    ReportCompleted(_sceneName);
                    Destroy(gameObject);
                }
            }
        }
    }
}
