
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Packet containing a list of ID's of players currently connected.
    /// </summary>
    public class MassIdentityPkt : MessageBase {

        /// <summary>
        /// List of Connection ID's to send
        /// </summary>
        public int[] IdList;

        /// <summary>
        /// Create a Mass Identity packet used to send a list of currently online connections.
        /// </summary>
        /// <param name="idList"></param>
        public MassIdentityPkt(int[] idList) {
            IdList = idList;
        }

        /// <summary>
        /// Parameter-less constructor using when deserializing the message.
        /// </summary>
        public MassIdentityPkt() {

        }

        /// <summary>
        /// Used to deserialize a message received via networking.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            byte[][] ByteArray = new byte[4][];
            ByteArray[0] = reader.ReadBytesAndSize();
            ByteArray[1] = reader.ReadBytesAndSize();
            ByteArray[2] = reader.ReadBytesAndSize();
            ByteArray[3] = reader.ReadBytesAndSize();
            IdList = NetworkHelper.DeserializeIntArray(ByteArray);
        }

        /// <summary>
        /// Used to serialize the message before it is sent.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            byte[][] ByteArray = NetworkHelper.SerializeIntArray(IdList);
            writer.WriteBytesFull(ByteArray[0]);
            writer.WriteBytesFull(ByteArray[1]);
            writer.WriteBytesFull(ByteArray[2]);
            writer.WriteBytesFull(ByteArray[3]);
        }
    }
}
