using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Contains the Connection ID of the user and weather the ID belongs to the client or not.
    /// </summary>
    public class IdentityPkt : MessageBase {

        /// <summary>
        /// ID of the Connection
        /// </summary>
        public int ConnectionID;

        /// <summary>
        /// Weather the recieving client owns this ID or not.
        /// </summary>
        public bool Owned;

        /// <summary>
        /// Creates a packet containing an ID and the detail weather the recieving client owns this ID.
        /// </summary>
        /// <param name="connectionID">ID of the connection.</param>
        /// <param name="owned">Weather the recieving client owns this ID.</param>
        public IdentityPkt(int connectionID, bool owned) {
            ConnectionID = connectionID;
            Owned = owned;
        }

        /// <summary>
        /// Parameter-less constructor using when deserializing the message.
        /// </summary>
        public IdentityPkt() {
        }

        /// <summary>
        /// Used to deserialize a message received via networking.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            ConnectionID = reader.ReadInt32();
            Owned = reader.ReadBoolean();
        }

        /// <summary>
        /// Used to serialize the message before it is sent.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(ConnectionID);
            writer.Write(Owned);
        }
    }
}
