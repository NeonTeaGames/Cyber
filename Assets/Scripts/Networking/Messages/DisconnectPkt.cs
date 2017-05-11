
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Packet containing a mesasge, that someone has disconnected.
    /// </summary>
    public class DisconnectPkt : MessageBase {

        /// <summary>
        /// ID of the connection, which disconnected.
        /// </summary>
        public int ConnectionID;

        /// <summary>
        /// Creates a disconnect package.
        /// </summary>
        /// <param name="connectionID"></param>
        public DisconnectPkt(int connectionID) {
            ConnectionID = connectionID;
        }

        /// <summary>
        /// Creates an empty disconnect package for whatever reason.
        /// </summary>
        public DisconnectPkt() { }

        /// <summary>
        /// Deserializes the disconnect package.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            ConnectionID = reader.ReadInt32();
        }

        /// <summary>
        /// Serializes the disconnect package for sending!
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(ConnectionID);
        }

    }
}
