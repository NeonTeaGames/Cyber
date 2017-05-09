
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Packet containing a list of ID's of players currently connected.
    /// </summary>
    public class MassIdentityPkt : MessageBase {

        public int[] IdList;

        public MassIdentityPkt(int[] idList) {
            IdList = idList;
        }

        public MassIdentityPkt() {

        }

        public override void Deserialize(NetworkReader reader) {
            byte[][] ByteArray = new byte[4][];
            ByteArray[0] = reader.ReadBytesAndSize();
            ByteArray[1] = reader.ReadBytesAndSize();
            ByteArray[2] = reader.ReadBytesAndSize();
            ByteArray[3] = reader.ReadBytesAndSize();
            IdList = NetworkHelper.DeserializeIntArray(ByteArray);
        }

        public override void Serialize(NetworkWriter writer) {
            byte[][] ByteArray = NetworkHelper.SerializeIntArray(IdList);
            writer.WriteBytesFull(ByteArray[0]);
            writer.WriteBytesFull(ByteArray[1]);
            writer.WriteBytesFull(ByteArray[2]);
            writer.WriteBytesFull(ByteArray[3]);
        }
    }
}
