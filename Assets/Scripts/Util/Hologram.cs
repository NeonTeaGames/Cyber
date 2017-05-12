using System.Collections.Generic;
using UnityEngine;
using Cyber.Util;

namespace Cyber.Util {

    /// <summary>
    /// Helper component for hologram meshes.
    /// </summary>
    public class Hologram : MonoBehaviour {

        /// <summary>
        /// The Hologram mesh.
        /// </summary>
        public MeshRenderer HologramMesh;

        /// <summary>
        /// Whether the hologram is visible or not.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// The texture that will handle the text which is displayed on this
        /// hologram.
        /// </summary>
        public TextTextureApplier Text;

        /// <summary>
        /// How fast the scanlines scroll. 1 = the scanlines will scroll the
        /// height of a line per second.
        /// </summary>
        public float HologramScanlineScrollingSpeed = 1f;

        private float CurrentScale;
        private List<Material> ScanlineMaterials = new List<Material>();

        private void Start() {
            CurrentScale = GetTargetScale();
            UpdateScale();
            foreach (Material Mat in HologramMesh.materials) {
                if (!Mat.name.Contains("Screen")) {
                    ScanlineMaterials.Add(Mat);
                }
            }
        }

        private void Update() {
            if (GetTargetScale() != CurrentScale) {
                UpdateScale();
            }
            float ScaledTime = Time.time * HologramScanlineScrollingSpeed;
            float UVScroll = (ScaledTime - (int) ScaledTime);
            foreach (Material Mat in ScanlineMaterials) {
                Mat.SetTextureOffset("_DetailAlbedoMap", new Vector2(0, UVScroll));
            }
        }

        private void UpdateScale() {
            float Scale = GetTargetScale();
            CurrentScale = Mathf.Lerp(CurrentScale, Scale, 8f * Time.deltaTime);
            if (Mathf.Abs(Scale - CurrentScale) < 0.001) {
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