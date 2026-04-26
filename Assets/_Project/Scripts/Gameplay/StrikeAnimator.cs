using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Data;

namespace TicTacToe
{
    /// <summary>
    /// Reveals the winning line by positioning and rotating a single
    /// <see cref="RectTransform"/> at runtime so it spans the three cells
    /// reported by <see cref="WinConditionChecker"/>. Listens to
    /// <see cref="GameManager.OnGameOver"/> to show the strike and to
    /// <see cref="GameManager.OnGameRestarted"/> to clear it.
    /// </summary>
    /// <remarks>
    /// One reusable line replaces the earlier eight pre-positioned variants.
    /// The first and last cell in the win line define the segment; the
    /// middle cell sits on that segment by construction. The line is
    /// parented to the cells' container so world-space math survives
    /// Canvas scaling across portrait and landscape.
    ///
    /// The reveal is driven from script rather than from the StrikeLine
    /// Animator. The Animator approach failed on retry because the Idle
    /// state has WriteDefaults enabled: when the popup's Retry button
    /// disabled the line mid-animation, scale.x was captured as the new
    /// "default" and written back over every subsequent strike, leaving
    /// the line at scale 0. A coroutine-driven lerp avoids that pitfall
    /// entirely and is also easier to defend in a code walkthrough.
    /// </remarks>
    public class StrikeAnimator : MonoBehaviour
    {
        private const int EXPECTED_CELL_COUNT = 9;
        private const int WIN_LINE_LENGTH = 3;

        [Header("Wiring")]
        [Tooltip("The reusable strike-line RectTransform. Authored horizontal and centered with pivot (0.5, 0.5); moved, rotated around Z, and optionally resized at runtime.")]
        [SerializeField] private RectTransform _strikeLine;

        [Tooltip("The nine cell anchors in row-major order (0 = top-left, 8 = bottom-right). Must match BoardController._cells index-for-index.")]
        [SerializeField] private RectTransform[] _cellAnchors;

        [Header("Sizing")]
        [Tooltip("When true, sizeDelta.x is set to the measured distance plus padding. When false, only position and rotation are written and the authored width is preserved.")]
        [SerializeField] private bool _resizeToFit = true;

        [Tooltip("Extra length added to the measured cell-to-cell distance so the line visibly overshoots the end cells.")]
        [SerializeField] private float _lengthPadding = 40f;

        [Header("Reveal")]
        [Tooltip("Seconds taken for the strike line to grow from zero to full width.")]
        [SerializeField] private float _revealDuration = 0.25f;

        private Image _strikeLineImage;
        private Coroutine _revealRoutine;

        private void Awake()
        {
            if (_strikeLine == null)
            {
                return;
            }

            _strikeLineImage = _strikeLine.GetComponent<Image>();

            // Disable the prefab's Animator so it cannot fight the script.
            // The Idle state runs with WriteDefaults on, which corrupts the
            // captured scale.x default whenever the line is hidden mid-play
            // (the Retry button's exact code path). Driving the reveal from
            // a coroutine sidesteps that class of bug entirely.
            Animator strikeLineAnimator = _strikeLine.GetComponent<Animator>();
            if (strikeLineAnimator != null)
            {
                strikeLineAnimator.enabled = false;
            }
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += HandleGameOver;
            GameManager.OnGameRestarted += HandleGameRestarted;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= HandleGameOver;
            GameManager.OnGameRestarted -= HandleGameRestarted;
        }

        /// <summary>
        /// Position and rotate the strike line so it spans the supplied
        /// winning cell indices, optionally stretching its width to fit,
        /// then activate it and run the reveal lerp.
        /// </summary>
        /// <param name="winLine">Three cell indices from <see cref="WinResult.WinLine"/>. Endpoints at [0] and [2] define the segment.</param>
        public void PlayStrike(int[] winLine)
        {
            if (!ValidateInput(winLine))
            {
                return;
            }

            RectTransform start = _cellAnchors[winLine[0]];
            RectTransform end = _cellAnchors[winLine[WIN_LINE_LENGTH - 1]];
            if (start == null || end == null)
            {
                Debug.LogError("[StrikeAnimator] A referenced cell anchor is null. Check the Inspector wiring.");
                return;
            }

            Vector3 startWorld = start.position;
            Vector3 endWorld = end.position;

            _strikeLine.position = (startWorld + endWorld) * 0.5f;

            Vector2 delta = endWorld - startWorld;
            float angleDegrees = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            _strikeLine.localRotation = Quaternion.Euler(0f, 0f, angleDegrees);

            if (_resizeToFit)
            {
                Vector2 size = _strikeLine.sizeDelta;
                size.x = delta.magnitude + _lengthPadding;
                _strikeLine.sizeDelta = size;
            }

            // Tint from the active HUD theme so the strike matches the
            // current palette. Pulled here rather than cached because the
            // theme can change between matches and the active surface is
            // always the source of truth.
            if (_strikeLineImage != null && ThemeManager.Instance != null && ThemeManager.Instance.ActiveThemeHUD != null)
            {
                _strikeLineImage.color = ThemeManager.Instance.ActiveThemeHUD.StrikeColor;
            }

            _strikeLine.gameObject.SetActive(true);

            if (_revealRoutine != null)
            {
                StopCoroutine(_revealRoutine);
            }
            _revealRoutine = StartCoroutine(RevealRoutine());
        }

        /// <summary>
        /// Hide the strike line so the next match starts clean. Called on
        /// restart and before a fresh strike is positioned.
        /// </summary>
        public void ResetStrike()
        {
            if (_revealRoutine != null)
            {
                StopCoroutine(_revealRoutine);
                _revealRoutine = null;
            }

            if (_strikeLine != null)
            {
                _strikeLine.gameObject.SetActive(false);
            }
        }

        private IEnumerator RevealRoutine()
        {
            // Start from zero width and grow to full so the line "draws"
            // along the win axis. localScale.y/z stay at 1 so the stroke
            // thickness is unaffected.
            SetRevealProgress(0f);

            float elapsed = 0f;
            float duration = Mathf.Max(_revealDuration, 0.0001f);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetRevealProgress(Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            SetRevealProgress(1f);
            _revealRoutine = null;
        }

        private void SetRevealProgress(float t)
        {
            Vector3 scale = _strikeLine.localScale;
            scale.x = t;
            scale.y = 1f;
            scale.z = 1f;
            _strikeLine.localScale = scale;
        }

        private void HandleGameOver(WinResult result)
        {
            if (result == null || !result.HasWinner || result.WinLine == null)
            {
                return;
            }

            PlayStrike(result.WinLine);
        }

        private void HandleGameRestarted() => ResetStrike();

        private bool ValidateInput(int[] winLine)
        {
            if (winLine == null || winLine.Length != WIN_LINE_LENGTH)
            {
                return false;
            }

            if (_strikeLine == null)
            {
                Debug.LogError("[StrikeAnimator] _strikeLine is not assigned. Wire it in the Inspector.");
                return false;
            }

            if (_cellAnchors == null || _cellAnchors.Length != EXPECTED_CELL_COUNT)
            {
                Debug.LogError($"[StrikeAnimator] _cellAnchors must contain exactly {EXPECTED_CELL_COUNT} RectTransforms in row-major order.");
                return false;
            }

            return true;
        }
    }
}
