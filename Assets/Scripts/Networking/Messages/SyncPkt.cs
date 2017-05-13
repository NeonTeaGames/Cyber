
using Cyber.Console;
using Cyber.Entities;
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Contains sync data to sync stuff with.
    /// </summary>
    public class SyncPkt : MessageBase {

        /// <summary>
        /// The Sync Packet ID of this packet.
        /// </summary>
        public int SyncPacketID;

        /// <summary>
        /// The Array of SyncID added in this SyncPkt
        /// </summary>
        public int[] SyncedSyncBases;

        private SyncDB SyncDB;

        /// <summary>
        /// Creates a SyncPkt on the serverside.
        /// </summary>
        /// <param name="syncDB">SyncDB to sync from.</param>
        /// <param name="syncBases">The ID's of the SyncBases to sync.</param>
        /// <param name="syncPacketID">ID of the sync packet itself.</param>
        public SyncPkt(SyncDB syncDB, int[] syncBases, int syncPacketID) {
            SyncPacketID = syncPacketID;
            SyncDB = syncDB;
            SyncedSyncBases = syncBases;
        }

        /// <summary>
        /// Creates SyncPkt for deserializing.
        /// </summary>
        /// <param name="syncDB">SyncBase to sync to.</param>
        public SyncPkt(SyncDB syncDB) {
            SyncDB = syncDB;
        }

        /// <summary>
        /// Deserializes the SynkPkt with ONLY the Sync Packet ID.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            SyncPacketID = reader.ReadInt32();
        }

        /// <summary>
        /// Applies the SyncPkt.
        /// </summary>
        /// <param name="reader"></param>
        public void ApplySync(NetworkReader reader) {
            byte[][] ByteArray = new byte[4][];
            ByteArray[0] = reader.ReadBytesAndSize();
            ByteArray[1] = reader.ReadBytesAndSize();
            ByteArray[2] = reader.ReadBytesAndSize();
            ByteArray[3] = reader.ReadBytesAndSize();
            SyncedSyncBases = NetworkHelper.DeserializeIntArray(ByteArray);

            foreach (int syncId in SyncedSyncBases) {
                SyncDB.Get(syncId).Deserialize(reader);
            }
        }

        /// <summary>
        /// Serializes the SyncPkt and writes everything it needs.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(SyncPacketID);

            byte[][] ByteArray = NetworkHelper.SerializeIntArray(SyncedSyncBases);
            writer.WriteBytesFull(ByteArray[0]);
            writer.WriteBytesFull(ByteArray[1]);
            writer.WriteBytesFull(ByteArray[2]);
            writer.WriteBytesFull(ByteArray[3]);
            
            foreach (int syncId in SyncedSyncBases) {
                SyncDB.Get(syncId).Serialize(writer);
            }
        }

    }
}
