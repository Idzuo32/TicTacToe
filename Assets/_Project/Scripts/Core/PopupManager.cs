using UnityEngine;
using System.Collections.Generic;
using TicTacToe.UI;

namespace TicTacToe
{
    /// <summary>
    /// Persistent singleton that drives popup open/close transitions through
    /// a LIFO stack so closing the top popup always returns focus to the
    /// previous one. Depends only on <see cref="IPopup"/> via
    /// <see cref="PopupBase"/> — never on specific popup types.
    /// </summary>
    public class PopupManager : MonoBehaviour
    {
        /// <summary>Global access point; non-null after <c>Awake</c>.</summary>
        public static PopupManager Instance { get; private set; }

        private readonly Stack<PopupBase> _popupStack = new();

        /// <summary>True when at least one popup is currently open.</summary>
        public bool HasOpenPopup => _popupStack.Count > 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Push <paramref name="popup"/> onto the stack and open it.
        /// Idempotent — opening a popup that is already at the top is a no-op.
        /// Opening an already-stacked popup that is not on top is rejected
        /// so the stack stays a simple LIFO.
        /// </summary>
        public void OpenPopup(PopupBase popup)
        {
            if (popup == null)
            {
                Debug.LogError("[PopupManager] OpenPopup called with null.");
                return;
            }

            if (_popupStack.Count > 0 && _popupStack.Peek() == popup)
            {
                return;
            }

            if (_popupStack.Contains(popup))
            {
                Debug.LogWarning($"[PopupManager] '{popup.name}' is already on the stack but not on top. Ignoring open.");
                return;
            }

            _popupStack.Push(popup);
            popup.gameObject.SetActive(true);
            popup.Open();
        }

        /// <summary>
        /// Close the popup currently on top of the stack. Safe to call when
        /// the stack is empty — no-op in that case.
        /// </summary>
        public void CloseTopPopup()
        {
            if (_popupStack.Count == 0)
            {
                return;
            }

            PopupBase top = _popupStack.Pop();
            if (top != null)
            {
                top.Close();
            }
        }

        /// <summary>
        /// Close every popup on the stack, top-first, so animations and
        /// dependencies unwind in the reverse of their open order. Used on
        /// scene transitions and hard resets.
        /// </summary>
        public void CloseAllPopups()
        {
            while (_popupStack.Count > 0)
            {
                PopupBase popup = _popupStack.Pop();
                if (popup != null)
                {
                    popup.Close();
                }
            }
        }
    }
}
