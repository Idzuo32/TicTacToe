using UnityEngine;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that owns the two audio channels (BGM and SFX)
    /// and plays every clip the game exposes. Syncs itself to
    /// <see cref="GameSettings"/> by listening to <see cref="SaveManager"/>
    /// events — it never polls or reads settings directly.
    /// </summary>
    /// <remarks>
    /// Two dedicated <see cref="AudioSource"/> components keep BGM and SFX
    /// independently toggleable; muting music must never silence a
    /// placement sound that is playing on the SFX channel.
    /// </remarks>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static AudioManager Instance { get; private set; }

        [Header("Channels")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Music")]
        [SerializeField] private AudioClip _bgmClip;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip _sfxButtonClick;
        [SerializeField] private AudioClip _sfxPlacement;
        [SerializeField] private AudioClip _sfxWin;
        [SerializeField] private AudioClip _sfxPopup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_bgmSource != null)
            {
                _bgmSource.loop = true;
                _bgmSource.playOnAwake = false;
                _bgmSource.clip = _bgmClip;
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
        }

        private void OnDisable()
        {
            SaveManager.OnSettingsLoaded -= HandleSettingsLoaded;
            SaveManager.OnSettingsChanged -= HandleSettingsChanged;
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
            if (_bgmSource == null || _bgmClip == null)
            {
                return;
            }

            if (_bgmSource.isPlaying)
            {
                return;
            }

            _bgmSource.clip = _bgmClip;
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

        /// <summary>Play the UI button click SFX.</summary>
        public void PlayButtonClick() => PlaySfx(_sfxButtonClick);

        /// <summary>Play the mark-placement SFX.</summary>
        public void PlayPlacement() => PlaySfx(_sfxPlacement);

        /// <summary>Play the match-win / strike SFX.</summary>
        public void PlayWin() => PlaySfx(_sfxWin);

        /// <summary>Play the popup open/close SFX.</summary>
        public void PlayPopup() => PlaySfx(_sfxPopup);

        /// <summary>
        /// Enable or disable the BGM channel and persist the change through
        /// <see cref="SaveManager"/>. The authoritative state lives in
        /// <see cref="GameSettings"/> — this method is the one place allowed
        /// to mutate the BGM source directly.
        /// </summary>
        public void SetMusicEnabled(bool enabled)
        {
            if (_bgmSource != null)
            {
                _bgmSource.enabled = enabled;
            }

            if (enabled)
            {
                PlayBGM();
            }
            else
            {
                StopBGM();
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateMusicEnabled(enabled);
            }
        }

        /// <summary>
        /// Enable or disable the SFX channel and persist the change through
        /// <see cref="SaveManager"/>. When disabled, any clip currently
        /// playing on the channel is allowed to finish — only new calls are
        /// suppressed.
        /// </summary>
        public void SetSFXEnabled(bool enabled)
        {
            if (_sfxSource != null)
            {
                _sfxSource.enabled = enabled;
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UpdateSFXEnabled(enabled);
            }
        }

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
    }
}
