
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Packet containing integers, used in many packet types, such as <see cref="PktType.MassIdentity"/> and <see cref="PktType.StaticObjectIdsPkt"/>.
    /// </summary>
    public class IntListPkt : MessageBase {

        /// <summary>
        /// List of Integers.
        /// </summary>
        public int[] IdList;

        /// <summary>
        /// Create a packet containing integers.
        /// </summary>
        /// <param name="idList"></param>
        public IntListPkt(int[] idList) {
            IdList = idList;
        }

        /// <summary>
        /// Parameter-less constructor using when deserializing the message.
        /// </summary>
        public IntListPkt() {

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
