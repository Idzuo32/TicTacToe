using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that owns the two audio channels (BGM and SFX)
    /// and plays every clip the game exposes. Syncs itself to
    /// <see cref="GameSettings"/> by listening to <see cref="SaveManager"/>
    /// events, to <see cref="IThemeAudio"/> via
    /// <see cref="ThemeManager.OnThemeChanged"/>, and to scene transitions
    /// via <see cref="SceneLoader.OnSceneLoadCompleted"/> — it never polls
    /// or reads settings directly.
    /// </summary>
    /// <remarks>
    /// Two dedicated <see cref="AudioSource"/> components keep BGM and SFX
    /// independently toggleable; muting music must never silence a
    /// placement sound that is playing on the SFX channel. BGM resolution
    /// is scene-aware: while <see cref="SceneNames.PlayScene"/> is active
    /// the BGM channel plays <see cref="_menuBgmClip"/>; in any other
    /// scene it plays the active theme's <see cref="IThemeAudio.BGMClip"/>.
    /// Both paths fall back to <see cref="_defaultBgmClip"/> when the
    /// preferred clip is null.
    /// </remarks>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static AudioManager Instance { get; private set; }

        [Header("Channels")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Music")]
        [Tooltip("Fallback BGM clip used when the active theme doesn't supply one. The 'music' clip lives here.")]
        [FormerlySerializedAs("_bgmClip")]
        [SerializeField] private AudioClip _defaultBgmClip;

        [Tooltip("BGM clip used while the main menu (PlayScene) is active. Falls back to _defaultBgmClip when null.")]
        [SerializeField] private AudioClip _menuBgmClip;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip _sfxButtonClick;
        [SerializeField] private AudioClip _sfxPlacement;
        [SerializeField] private AudioClip _sfxWin;
        [SerializeField] private AudioClip _sfxPopup;

        private string _currentSceneName;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _currentSceneName = SceneManager.GetActiveScene().name;

            if (_bgmSource != null)
            {
                _bgmSource.loop = true;
                _bgmSource.playOnAwake = false;
                _bgmSource.clip = ResolveBgmClip();
            }

            if (_sfxSource != null)
            {
                _sfxSource.loop = false;
                _sfxSource.playOnAwake = false;
            }
        }

        private void OnEnable()
        {
            SaveManager.OnSettingsLoaded += HandleSettingsLoaded;
            SaveManager.OnSettingsChanged += HandleSettingsChanged;
            ThemeManager.OnThemeChanged += HandleThemeChanged;
            SceneLoader.OnSceneLoadCompleted += HandleSceneLoadCompleted;

            // SaveManager fires OnSettingsLoaded inside its own Awake,
            // before any OnEnable runs across the scene — so the initial
            // broadcast is missed by every listener that subscribes here.
            // Re-apply manually for cold-boot correctness. Without this,
            // BGM never starts on Play even when MusicEnabled is true.
            // Mirrors the ThemeManager.OnEnable defensive pattern.
            if (SaveManager.Instance != null && SaveManager.Instance.Settings != null)
            {
                HandleSettingsLoaded(SaveManager.Instance.Settings);
            }
        }

        private void OnDisable()
        {
            SaveManager.OnSettingsLoaded -= HandleSettingsLoaded;
            SaveManager.OnSettingsChanged -= HandleSettingsChanged;
            ThemeManager.OnThemeChanged -= HandleThemeChanged;
            SceneLoader.OnSceneLoadCompleted -= HandleSceneLoadCompleted;
        }

        private void Start()
        {
            // Re-resolve the BGM clip at boot in case ThemeManager applied
            // its initial theme before our OnEnable subscribed, and to pick
            // up the right scene-specific clip for whichever scene is
            // currently active (PlayScene at cold boot, GameScene if
            // entering directly via the editor for testing).
            RefreshBgm();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>Begin looping the background music clip on the BGM channel.</summary>
        public void PlayBGM()
        {
            if (_bgmSource == null)
            {
                return;
            }

            if (_bgmSource.clip == null)
            {
                _bgmSource.clip = _defaultBgmClip;
            }

            if (_bgmSource.clip == null || _bgmSource.isPlaying)
            {
                return;
            }

            _bgmSource.Play();
        }

        /// <summary>Stop background music immediately — used on app pause or mute.</summary>
        public void StopBGM()
        {
            if (_bgmSource == null)
            {
                return;
            }

            _bgmSource.Stop();
        }

        public void PlayButtonClick() => PlaySfx(_sfxButtonClick);

        public void PlayPlacement() => PlaySfx(_sfxPlacement);

        /// <summary>Play the match-win / strike SFX.</summary>
        public void PlayWin() => PlaySfx(_sfxWin);

        /// <summary>Play the popup open/close SFX.</summary>
        public void PlayPopup() => PlaySfx(_sfxPopup);

        /// <summary>
        /// Re-resolve the active BGM clip. Kept for backward compatibility
        /// with callers that pass a theme directly; the parameter is
        /// ignored because <see cref="ResolveBgmClip"/> reads the active
        /// theme from <see cref="ThemeManager"/> together with the current
        /// scene name, ensuring scene-specific overrides (e.g. menu music
        /// in <see cref="SceneNames.PlayScene"/>) win when applicable.
        /// </summary>
        /// <param name="theme">Ignored. Present for API stability.</param>
        public void ApplyThemeAudio(IThemeAudio theme) => RefreshBgm();

        private void PlaySfx(AudioClip clip)
        {
            if (_sfxSource == null || !_sfxSource.enabled || clip == null)
            {
                return;
            }

            _sfxSource.PlayOneShot(clip);
        }

        private void HandleSettingsLoaded(GameSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            if (_bgmSource != null)
            {
                _bgmSource.enabled = settings.MusicEnabled;
            }

            if (_sfxSource != null)
            {
                _sfxSource.enabled = settings.SFXEnabled;
            }

            if (settings.MusicEnabled)
            {
                PlayBGM();
            }
            else
            {
                StopBGM();
            }
        }

        private void HandleSettingsChanged(GameSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            if (_bgmSource != null && _bgmSource.enabled != settings.MusicEnabled)
            {
                _bgmSource.enabled = settings.MusicEnabled;
                if (settings.MusicEnabled)
                {
                    PlayBGM();
                }
                else
                {
                    StopBGM();
                }
            }

            if (_sfxSource != null && _sfxSource.enabled != settings.SFXEnabled)
            {
                _sfxSource.enabled = settings.SFXEnabled;
            }
        }

        private void HandleThemeChanged(ITheme _) => RefreshBgm();

        private void HandleSceneLoadCompleted(string sceneName)
        {
            _currentSceneName = sceneName;
            RefreshBgm();
        }

        /// <summary>
        /// Pick the BGM clip that should be playing right now. PlayScene
        /// uses <see cref="_menuBgmClip"/> regardless of theme; every other
        /// scene reads from the active <see cref="IThemeAudio"/>. Either
        /// path falls back to <see cref="_defaultBgmClip"/> when the
        /// preferred clip is null.
        /// </summary>
        private AudioClip ResolveBgmClip()
        {
            if (_currentSceneName == SceneNames.PlayScene)
            {
                return _menuBgmClip != null ? _menuBgmClip : _defaultBgmClip;
            }

            IThemeAudio theme = ThemeManager.Instance != null ? ThemeManager.Instance.ActiveThemeAudio : null;
            AudioClip themeClip = theme != null ? theme.BGMClip : null;
            return themeClip != null ? themeClip : _defaultBgmClip;
        }

        private void RefreshBgm() => SetBgmClip(ResolveBgmClip());

        private void SetBgmClip(AudioClip clip)
        {
            if (_bgmSource == null || _bgmSource.clip == clip)
            {
                return;
            }

            bool wasPlaying = _bgmSource.isPlaying;
            if (wasPlaying)
            {
                _bgmSource.Stop();
            }

            _bgmSource.clip = clip;

            if (wasPlaying && clip != null)
            {
                _bgmSource.Play();
            }
        }
    }
}
