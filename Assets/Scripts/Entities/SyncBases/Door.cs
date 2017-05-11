using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Entities.SyncBases {
    public class Door : Interactable {

        /// <summary>
        /// The root of the door mesh. This will be scaled to animate the door.
        /// </summary>
        public Transform DoorRoot;

        /// <summary>
        /// The openness of the door.
        /// </summary>
        public bool IsOpen = false;

        /// <summary>
        /// Toggles the openness of the door.
        /// </summary>
        public override void Interact() {
            IsOpen = !IsOpen;
        }

        /// <summary>
        /// Reads the openness of the door from the server.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            IsOpen = reader.ReadBoolean();
        }

        /// <summary>
        /// Writes the openness of the door.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(IsOpen);
        }

        /// <summary>
        /// Return the Sync Handletype information for <see cref="Syncer"/>.
        /// </summary>
        /// <returns>Sync Handletype containing sync information.</returns>
        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(false, 10);
        }

        private void Update() {
            float DoorScale = IsOpen ? 0.01f : 1;
            if (DoorRoot.localScale.x != DoorScale) {
                Vector3 Scale = DoorRoot.localScale;
                if (Mathf.Abs(Scale.x - DoorScale) < 0.01) {
                    Scale.x = DoorScale;
                } else {
                    Scale.x = Mathf.Lerp(Scale.x, DoorScale, 5f * Time.deltaTime);
                }
                DoorRoot.localScale = Scale;
            }
        }
    }
}