using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    /// <summary>
    /// Abstract base class for every popup. Centralises the open/close
    /// animation, the popup SFX, the deactivation handoff, and the
    /// <see cref="IThemeUI"/> background skin so individual popups only
    /// describe their own content — never the lifecycle.
    /// </summary>
    /// <remarks>
    /// Popups never close themselves directly; <see cref="PopupManager"/>
    /// owns the stack and calls <see cref="Close"/>. Subclasses implement
    /// <see cref="OnOpened"/> / <see cref="OnClosed"/> for content
    /// population and teardown, and may override <see cref="ApplyTheme"/>
    /// to also update title / body text colors from the same
    /// <see cref="IThemeUI"/> the base uses for the background sprite.
    /// </remarks>
    /// <remarks>
    /// Subclasses that need their own <c>OnEnable</c> / <c>OnDisable</c>
    /// must override the protected virtual versions on this class and
    /// call <c>base.OnEnable()</c> / <c>base.OnDisable()</c> first —
    /// otherwise the theme subscription is silently dropped.
    /// </remarks>
    public abstract class PopupBase : MonoBehaviour, IPopup
    {
        /// <summary>Default close-animation duration in seconds; override per popup via <see cref="_closeAnimationSeconds"/>.</summary>
        protected const float DEFAULT_CLOSE_ANIMATION_SECONDS = 0.3f;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Tooltip("Seconds to wait after triggering PopupClose before deactivating the GameObject. Must match the close clip length.")]
        [SerializeField] private float _closeAnimationSeconds = DEFAULT_CLOSE_ANIMATION_SECONDS;

        [Header("Theming")]
        [Tooltip("Image rendering the popup panel background. Skinned from IThemeUI.PopupBackgroundSprite when present.")]
        [SerializeField] private Image _backgroundImage;

        private Coroutine _closeRoutine;

        /// <inheritdoc />
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Subscribes to theme changes and applies the current UI theme
        /// to this popup's background. Subclasses that override this
        /// method MUST call <c>base.OnEnable()</c> first.
        /// </summary>
        protected virtual void OnEnable()
        {
            ThemeManager.OnThemeChanged += HandleThemeChanged;

            if (ThemeManager.Instance != null)
            {
                ApplyTheme(ThemeManager.Instance.ActiveThemeUI);
            }
        }

        /// <summary>
        /// Releases the theme-change subscription. Subclasses that override
        /// this method MUST call <c>base.OnDisable()</c> first.
        /// </summary>
        protected virtual void OnDisable()
        {
            ThemeManager.OnThemeChanged -= HandleThemeChanged;
        }

        /// <summary>
        /// Activate the GameObject, play the open animation, emit the popup
        /// SFX, mark the popup open, and dispatch to <see cref="OnOpened"/>
        /// for subclass content setup.
        /// </summary>
        public virtual void Open()
        {
            if (_closeRoutine != null)
            {
                StopCoroutine(_closeRoutine);
                _closeRoutine = null;
            }

            gameObject.SetActive(true);

            if (_animator != null)
            {
                _animator.ResetTrigger(AnimatorParams.POPUP_CLOSE);
                _animator.SetTrigger(AnimatorParams.POPUP_OPEN);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPopup();
            }

            IsOpen = true;
            OnOpened();
        }

        /// <summary>
        /// Play the close animation, dispatch to <see cref="OnClosed"/> for
        /// subclass teardown, and deactivate the GameObject after the
        /// configured animation duration. Safe to call when already closed.
        /// </summary>
        public virtual void Close()
        {
            if (!IsOpen)
            {
                return;
            }

            if (_animator != null)
            {
                _animator.ResetTrigger(AnimatorParams.POPUP_OPEN);
                _animator.SetTrigger(AnimatorParams.POPUP_CLOSE);
            }

            IsOpen = false;
            OnClosed();

            if (isActiveAndEnabled)
            {
                _closeRoutine = StartCoroutine(DeactivateAfterAnimation());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called by an <c>AnimationEvent</c> on the final frame of the
        /// close animation to deactivate the popup GameObject. Public so
        /// the Animator can bind to it — <c>AnimationEvent</c> callbacks
        /// require a public method signature. Safe to call at any time;
        /// if a subsequent <see cref="Open"/> has already reactivated the
        /// popup, the coroutine fallback in <see cref="Close"/> suppresses
        /// redundant deactivation on its own path.
        /// </summary>
        public void DeactivateSelf()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Apply the active <see cref="IThemeUI"/> to this popup's
        /// background. When the theme provides
        /// <see cref="IThemeUI.PopupBackgroundSprite"/> the sprite is
        /// applied with a white tint; otherwise the background falls back
        /// to flat <see cref="Color.white"/>. Subclasses override to also
        /// update title and body text colors from the same
        /// <see cref="IThemeUI"/>.
        /// </summary>
        /// <param name="theme">Active UI theme. May be null during very early init — call is a no-op in that case.</param>
        public virtual void ApplyTheme(IThemeUI theme)
        {
            if (theme == null || _backgroundImage == null)
            {
                return;
            }

            if (theme.PopupBackgroundSprite != null)
            {
                _backgroundImage.sprite = theme.PopupBackgroundSprite;
                _backgroundImage.color = Color.white;
            }
            else
            {
                _backgroundImage.sprite = null;
                _backgroundImage.color = Color.white;
            }
        }

        /// <summary>
        /// Subclass hook invoked inside <see cref="Open"/> after the popup
        /// is visible. Use this to populate content (stats values, theme
        /// list, confirm text) — never to trigger its own animations.
        /// Default implementation is a no-op so popups with purely static
        /// content (e.g. <c>ExitConfirmPopup</c>) don't carry empty overrides.
        /// </summary>
        protected virtual void OnOpened() { }

        /// <summary>
        /// Subclass hook invoked inside <see cref="Close"/> before the
        /// deactivation delay. Use this to stop timers, unsubscribe from
        /// live events, or commit pending edits. Default implementation is
        /// a no-op for the same reason as <see cref="OnOpened"/>.
        /// </summary>
        protected virtual void OnClosed() { }

        private void HandleThemeChanged(ITheme _)
        {
            if (ThemeManager.Instance != null)
            {
                ApplyTheme(ThemeManager.Instance.ActiveThemeUI);
            }
        }

        private IEnumerator DeactivateAfterAnimation()
        {
            yield return new WaitForSeconds(_closeAnimationSeconds);
            _closeRoutine = null;
            if (!IsOpen)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
