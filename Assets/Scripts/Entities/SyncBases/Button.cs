using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cyber.Networking;
using Cyber.Console;
using System;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// Object that blinks when interacted with.
    /// </summary>
    public class Button : Interactable {

        /// <summary>
        /// The interactable which this button will trigger.
        /// </summary>
        public Interactable[] WillTrigger;

        /// <summary>
        /// The button mesh that will blink.
        /// </summary>
        public MeshRenderer Mesh;

        /// <summary>
        /// How long the blink lasts.
        /// </summary>
        public float BlinkLength = 1f;

        /// <summary>
        /// How bright the light is (upwards of 1 for the bloom to kick in)
        /// </summary>
        public float BlinkBrightness = 1.5f;

        /// <summary>
        /// The color of the blink.
        /// </summary>
        public Color BlinkColor = new Color(1f, 0.6f, 0f);

        private float BlinkTime = float.MinValue;
        private Material Material;

        /// <summary>
        /// When the button is interacted with, it blinks once, light lasting
        /// for <see cref="BlinkLength"/> seconds, and calls Interact
        /// on the <see cref="WillTrigger"/>.
        /// </summary>
        public override void Interact(SyncBase Trigger) {
            BlinkTime = Time.time;
            if (WillTrigger.Length > 0) {
                foreach (Interactable Triggerable in WillTrigger) {
                    Triggerable.Interact(this);
                }
            } else {
                Term.Println("FIXME: The button '" + gameObject.name + "' was pressed, but it doesn't have anything to trigger.");
            }
        }

        /// <summary>
        /// Does nothing, because it doesn't need to synced.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {}

        /// <summary>
        /// Does nothing, because it doesn't need to synced.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
        }

        public override InteractableSyncdata GetInteractableSyncdata() {
            return new InteractableSyncdata(false, true);
        }

        /// <summary>
        /// Buttons only act as triggers, so only interact is sent to the server.
        /// </summary>
        /// <returns>Sync Handletype containing sync information.</returns>
        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(false, 1000);
        }

        private void Start() {
            Material = Mesh.material;
        }

        private void Update() {
            float CurrentTime = Time.time - BlinkTime;
            if (CurrentTime < BlinkLength) {
                float Brightness = (1f - CurrentTime / BlinkLength) * BlinkBrightness;
                Color NewColor = new Color(Brightness * BlinkColor.r, 
                    Brightness * BlinkColor.g, Brightness * BlinkColor.b);
                Material.SetColor("_EmissionColor", NewColor);
            }
        }
    }
}
