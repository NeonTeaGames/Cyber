
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Includes information about interacting an interactible. Applicable only for some interactibles.
    /// </summary>
    public class InteractionPkt : MessageBase {

        /// <summary>
        /// ID of the interactible.
        /// </summary>
        public int InteractSyncBaseID;

        /// <summary>
        /// Id of the interactor.
        /// </summary>
        public int OwnerSyncBaseID;

        /// <summary>
        /// Creates an InteraktionPkt, which contains the message "someone interacted".
        /// </summary>
        /// <param name="syncBaseID">The Sync Base ID of the interacted thing.</param>
        public InteractionPkt(int syncBaseID) {
            InteractSyncBaseID = syncBaseID;
        }

        /// <summary>
        /// Empty constructor for deserialization.
        /// </summary>
        public InteractionPkt() {}

        /// <summary>
        /// Deserializes SyncBaseID for the recipent.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            InteractSyncBaseID = reader.ReadInt32();
            OwnerSyncBaseID = reader.ReadInt32();
        }

        /// <summary>
        /// Serializes the SyncBaseID for sending.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(InteractSyncBaseID);
            writer.Write(OwnerSyncBaseID);
        }

    }
}
