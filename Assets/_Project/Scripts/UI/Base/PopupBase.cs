using UnityEngine;
using System.Collections;

namespace TicTacToe.UI
{
    /// <summary>
    /// Abstract base class for every popup. Centralises the open/close
    /// animation, the popup SFX, and the deactivation handoff so individual
    /// popups only describe their own content — never the lifecycle.
    /// </summary>
    /// <remarks>
    /// Popups never close themselves directly; <see cref="PopupManager"/>
    /// owns the stack and calls <see cref="Close"/>. Subclasses implement
    /// <see cref="OnOpened"/> / <see cref="OnClosed"/> for content
    /// population and teardown.
    /// </remarks>
    public abstract class PopupBase : MonoBehaviour, IPopup
    {
        /// <summary>Default close-animation duration in seconds; override per popup via <see cref="_closeAnimationSeconds"/>.</summary>
        protected const float DEFAULT_CLOSE_ANIMATION_SECONDS = 0.3f;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Tooltip("Seconds to wait after triggering PopupClose before deactivating the GameObject. Must match the close clip length.")]
        [SerializeField] private float _closeAnimationSeconds = DEFAULT_CLOSE_ANIMATION_SECONDS;

        private Coroutine _closeRoutine;

        /// <inheritdoc />
        public bool IsOpen { get; private set; }

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
        /// Subclass hook invoked inside <see cref="Open"/> after the popup
        /// is visible. Use this to populate content (stats values, theme
        /// list, confirm text) — never to trigger its own animations.
        /// </summary>
        protected abstract void OnOpened();

        /// <summary>
        /// Subclass hook invoked inside <see cref="Close"/> before the
        /// deactivation delay. Use this to stop timers, unsubscribe from
        /// live events, or commit pending edits.
        /// </summary>
        protected abstract void OnClosed();

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
