
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// This Packet contains sync data from the client to the server like movement direction and rotation.
    /// </summary>
    public class ClientSyncPkt : MessageBase {

        /// <summary>
        /// ID of this client sync packet. This is separate from the server sync id.
        /// </summary>
        public int ClientSyncID;

        /// <summary>
        /// Current movement direction of the player
        /// </summary>
        public Vector3 MoveDirection;

        /// <summary>
        /// Current rotation of the player
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// Create an empty client sync packet.
        /// </summary>
        public ClientSyncPkt() { }

        /// <summary>
        /// Deserializes the sync packet. Does not deserialize everything, only the ID.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            ClientSyncID = reader.ReadInt32();
        }

        /// <summary>
        /// Reads the rest of the sync packet. Must be deserialized first.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadTheRest(NetworkReader reader) {
            MoveDirection = reader.ReadVector3();
            Rotation = reader.ReadVector3();
        }

        /// <summary>
        /// Serializes the entire packet for sending.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(ClientSyncID);
            writer.Write(MoveDirection);
            writer.Write(Rotation);
        }
    }
}
