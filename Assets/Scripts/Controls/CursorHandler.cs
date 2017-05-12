using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Console;

namespace Cyber.Controls {

    /// <summary>
    /// Handles grabbing the cursor when needed and provides info about the mouse.
    /// </summary>
    public class CursorHandler : MonoBehaviour {
    
        /// <summary>
        /// The mouse sensitivity on the screen's x-axis.
        /// </summary>
        [Range(1f, 5.0f)]
        public float MouseSensitivityX = 2.5f;

        /// <summary>
        /// The mouse sensitivity on the screen's y-axis.
        /// </summary>
        [Range(1f, 5.0f)]
        public float MouseSensitivityY = 2.5f;

        private bool CursorLocked = true;
        private bool RequestedLockState = true;

        /// <summary>
        /// Request a new lock state. The cursor will be locked in case there
        /// isn't another reason to have it in a different state (like the
        /// <see cref="DebugConsole"/> being up). 
        /// </summary>
        /// <param name="locked">If set to <c>true</c>, cursor might bse 
        /// locked.</param>
        public void RequestLockState(bool locked) {
            RequestedLockState = locked;
        }

        /// <summary>
        /// Is the cursor currently locked?
        /// </summary>
        public bool Locked() {
            return Term.IsVisible() || RequestedLockState;
        }

        private void Start() {
            UpdateCursor();
        }

        private void Update() {
            if (Term.IsVisible()) {
                CursorLocked = false;
                UpdateCursor();
            } else {
                CursorLocked = RequestedLockState;
                UpdateCursor();
            }
        }

        private void UpdateCursor() {
            if (CursorLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}