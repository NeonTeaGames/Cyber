using UnityEngine;
using Cyber.Util;

namespace Cyber.Util {

    /// <summary>
    /// Helper component for hologram meshes.
    /// </summary>
    public class Hologram : MonoBehaviour {

        /// <summary>
        /// Whether the hologram is visible or not.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// The texture that will handle the text which is displayed on this
        /// hologram.
        /// </summary>
        public TextTextureApplier Text;

        private float CurrentScale;

        private void Start() {
            CurrentScale = GetTargetScale();
            UpdateScale();
        }

        private void Update() {
            if (GetTargetScale() != CurrentScale) {
                UpdateScale();
            }
        }

        private void UpdateScale() {
            float Scale = GetTargetScale();
            CurrentScale = Mathf.Lerp(CurrentScale, Scale, 8f * Time.deltaTime);
            if (Mathf.Abs(Scale - CurrentScale) < 0.05) {
                CurrentScale = Scale;
            }
            transform.localScale = 
                new Vector3(CurrentScale, CurrentScale, CurrentScale);
        }

        private float GetTargetScale() {
            return Visible ? 1 : 0;
        }
    }
}