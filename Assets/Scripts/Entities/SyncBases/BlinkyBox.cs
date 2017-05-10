using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cyber.Networking;
using Cyber.Console;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// Object that blinks when interacted with.
    /// </summary>
    public class BlinkyBox : Interactable {

        /// <summary>
        /// The lamp mesh that will blink.
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

        private double BlinkTime = 0;
        private Material Material;

        /// <summary>
        /// When a BlinkyBox is interacted with, it blinks once, light lasting
        /// for <see cref="BlinkLength"/> seconds.
        /// </summary>
        public override void Interact() {
            BlinkTime = NetworkHelper.GetCurrentSystemTime();
        }

        /// <summary>
        /// Update the blink time if the server has a newer blinktime
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            double ServerBlinkTime = reader.ReadDouble();
            if (ServerBlinkTime > BlinkTime) {
                BlinkTime = ServerBlinkTime;
            }
        }

        /// <summary>
        /// Write the blink time so others will sync if they have older blinks.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(BlinkTime);
        }

        /// <summary>
        /// The blinky box doesn't need a hash and it should update every 5 ticks.
        /// </summary>
        /// <returns>Sync Handletype containing sync information.</returns>
        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(false, 5);
        }

        private void Start() {
            Material = Mesh.material;
        }

        private void Update() {
            float Time = (float) (NetworkHelper.GetCurrentSystemTime() - BlinkTime);
            if (Time < BlinkLength) {
                float Brightness = (1f - Time / BlinkLength) * BlinkBrightness;
                Color NewColor = new Color(Brightness * BlinkColor.r, 
                    Brightness * BlinkColor.g, Brightness * BlinkColor.b);
                Material.SetColor("_EmissionColor", NewColor);
            }
        }
    }
}
