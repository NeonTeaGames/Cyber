using UnityEngine;
using UnityEngine.Networking;
using Cyber.Console;
using Cyber.Util;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// Runs functions.
    /// </summary>
    /// \todo Implement programs in an editor-friendly way
    public class Computer : Interactable {
        /// <summary>The "left key" key code.</summary>
        public const string KeyCodeLeft = "KeyLeft";
        /// <summary>The "right key" key code.</summary>
        public const string KeyCodeRight = "KeyRight";

        /// <summary>
        /// The delegate which defines the function that will be run when the
        /// computer is interacted with (directly or through keys).
        /// </summary>
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

        /// <summary>
        /// Runs the <see cref="Program"/> with some input, determined by the
        /// Trigger.
        /// </summary>
        /// <param name="Trigger">Determines the keycode given to the 
        /// <see cref="Program"/>.</param>
        public override void Interact(SyncBase Trigger) {
            if (Trigger == KeyLeft) {
                Screen.SetTextProperties(new TextTextureProperties("\n   Pressed left!"));
            } else if (Trigger == KeyRight) {
                Screen.SetTextProperties(new TextTextureProperties("\n   Pressed right!"));
            } else {
                Screen.SetTextProperties(new TextTextureProperties(""));
            }
        }

        /// <summary>
        /// No serialization needed (yet).
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
        }


        /// <summary>
        /// No serialization needed (yet).
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
        }

        /// <summary>
        /// Computers could have lots of data, and therefore should be hashed
        /// to optimize bandwidth usage. No hurry in updating them though, so
        /// an update per 10 ticks is enough.
        /// </summary>
        /// <returns>The sync handletype.</returns>
        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(true, 10);
        }

        /// <summary>
        /// Computers should share state between different clients, so sync and
        /// send interactions to the network.
        /// </summary>
        /// <returns>The Interaction information.</returns>
        public override InteractableSyncdata GetInteractableSyncdata() {
            return new InteractableSyncdata(true, true);
        }
    }
}