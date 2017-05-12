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
        private bool Requested = false;

        public void RequestLockState(bool locked) {
            RequestedLockState = locked;
            Requested = true;
        }

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
            } else if (Requested) {
                CursorLocked = RequestedLockState;
                UpdateCursor();
                Requested = false;
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