using UnityEngine;
using Cyber.Util;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// Helper component for hologram meshes.
    /// </summary>
    public class Hologram : Interactable {

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

        public override void Interact(SyncBase Trigger) {
            Visible = !Visible;
        }

        public override void Deserialize(UnityEngine.Networking.NetworkReader reader) {
        }

        public override void Serialize(UnityEngine.Networking.NetworkWriter writer) {
        }

        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(false, 10);
        }

        public override InteractableSyncdata GetInteractableSyncdata() {
            return new InteractableSyncdata(false, false);
        }

        private void Start() {
            CurrentScale = Visible ? 1 : 0;
            UpdateScale();
        }

        private void Update() {
            float Scale = Visible ? 1 : 0;
            if (Scale != CurrentScale) {
                UpdateScale();
            }
        }

        private void UpdateScale() {
            float Scale = Visible ? 1 : 0;
            CurrentScale = Mathf.Lerp(CurrentScale, Scale, 8f * Time.deltaTime);
            if (Mathf.Abs(Scale - CurrentScale) < 0.05) {
                CurrentScale = Scale;
            }
            transform.localScale = 
                new Vector3(CurrentScale, CurrentScale, CurrentScale);
        }
    }
}