using UnityEngine;
using UnityEngine.Networking;
using Cyber.Console;
using Cyber.Util;

namespace Cyber.Entities.SyncBases {
    public class Computer : Interactable {
        public const string KeyCodeLeft = "KeyLeft";
        public const string KeyCodeRight = "KeyRight";
        public delegate void RunProgram(Computer host, string key);

        /// <summary>
        /// The screen this computer will print its output on.
        /// </summary>
        public TextTextureApplier Screen;

        /// <summary>
        /// The "left" key for this computer. Might cause actions depending 
        /// on the program.
        /// </summary>
        public Button KeyLeft;

        /// <summary>
        /// The "right" key for this computer. Might cause actions depending 
        /// on the program.
        /// </summary>
        public Button KeyRight;

        /// <summary>
        /// The function that is run when inputs are triggered. Can cause 
        /// state changes in the host computer.
        /// </summary>
        public RunProgram Program;

        public override void Interact(SyncBase Trigger) {
            if (Trigger == KeyLeft) {
                Screen.SetTextProperties(new TextTextureProperties("\n   Pressed left!"));
            } else if (Trigger == KeyRight) {
                Screen.SetTextProperties(new TextTextureProperties("\n   Pressed right!"));
            } else {
                Screen.SetTextProperties(new TextTextureProperties(""));
            }
        }

        public override void Deserialize(NetworkReader reader) {
        }

        public override void Serialize(NetworkWriter writer) {
        }

        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(true, 10);
        }

        public override InteractableSyncdata GetInteractableSyncdata() {
            return new InteractableSyncdata(true, true);
        }
    }
}