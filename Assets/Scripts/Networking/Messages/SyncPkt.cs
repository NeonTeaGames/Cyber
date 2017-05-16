
using Cyber.Console;
using Cyber.Entities;
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Contains sync data to sync stuff with.
    /// </summary>
    public class SyncPkt : MessageBase {

        /// <summary>
        /// Timestamp of the sync packet when sent from the server.
        /// </summary>
        public double Timestamp;

        /// <summary>
        /// The Sync Packet ID of this packet.
        /// </summary>
        public int SyncPacketID;

        /// <summary>
        /// The Array of SyncID added in this SyncPkt
        /// </summary>
        public int[] SyncedSyncBases;

        /// <summary>
        /// The Array of SyncIDs that have their hashes in this syncpacket
        /// </summary>
        public int[] ChecksummedSyncBases;

        /// <summary>
        /// The Array of Checksums that are sent with <see cref="ChecksummedSyncBases"/>.
        /// </summary>
        public int[] Checksums;

        private SyncDB SyncDB;

        /// <summary>
        /// Creates a SyncPkt on the serverside.
        /// </summary>
        /// <param name="syncDB">SyncDB to sync from.</param>
        /// <param name="syncBases">The ID's of the SyncBases to sync.</param>
        /// <param name="checksummedSyncBases"></param>
        /// <param name="checksums"></param>
        /// <param name="syncPacketID">ID of the sync packet itself.</param>
        public SyncPkt(SyncDB syncDB, int[] syncBases, int[] checksummedSyncBases, int[] checksums, int syncPacketID, double timestamp) {
            SyncPacketID = syncPacketID;
            SyncDB = syncDB;
            SyncedSyncBases = syncBases;
            ChecksummedSyncBases = checksummedSyncBases;
            Checksums = checksums;
            timestamp = Timestamp;
        }

        /// <summary>
        /// Creates SyncPkt for deserializing.
        /// </summary>
        /// <param name="syncDB">SyncBase to sync to.</param>
        public SyncPkt(SyncDB syncDB) {
            SyncDB = syncDB;
            ChecksummedSyncBases = new int[0];
            Checksums = new int[0];
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

            bool checksums = reader.ReadBoolean();

            if (checksums) {
                byte[][] SummedIdsByteArray = new byte[4][];
                SummedIdsByteArray[0] = reader.ReadBytesAndSize();
                SummedIdsByteArray[1] = reader.ReadBytesAndSize();
                SummedIdsByteArray[2] = reader.ReadBytesAndSize();
                SummedIdsByteArray[3] = reader.ReadBytesAndSize();

                byte[][] SumsByteArray = new byte[4][];
                SumsByteArray[0] = reader.ReadBytesAndSize();
                SumsByteArray[1] = reader.ReadBytesAndSize();
                SumsByteArray[2] = reader.ReadBytesAndSize();
                SumsByteArray[3] = reader.ReadBytesAndSize();

                ChecksummedSyncBases = NetworkHelper.DeserializeIntArray(SummedIdsByteArray);
                Checksums = NetworkHelper.DeserializeIntArray(SumsByteArray);
            }
        }

        /// <summary>
        /// Serializes the SyncPkt and writes everything it needs.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(SyncPacketID);

            byte[][] SyncedIdsByteArray = NetworkHelper.SerializeIntArray(SyncedSyncBases);
            writer.WriteBytesFull(SyncedIdsByteArray[0]);
            writer.WriteBytesFull(SyncedIdsByteArray[1]);
            writer.WriteBytesFull(SyncedIdsByteArray[2]);
            writer.WriteBytesFull(SyncedIdsByteArray[3]);
            
            foreach (int syncId in SyncedSyncBases) {
                SyncDB.Get(syncId).Serialize(writer);
            }

            if (ChecksummedSyncBases.Length == 0) {
                writer.Write(false);
            } else {
                writer.Write(true);

                byte[][] SummedIdsByteArray = NetworkHelper.SerializeIntArray(ChecksummedSyncBases);
                writer.WriteBytesFull(SummedIdsByteArray[0]);
                writer.WriteBytesFull(SummedIdsByteArray[1]);
                writer.WriteBytesFull(SummedIdsByteArray[2]);
                writer.WriteBytesFull(SummedIdsByteArray[3]);
                
                byte[][] SumsByteArray = NetworkHelper.SerializeIntArray(Checksums);
                writer.WriteBytesFull(SumsByteArray[0]);
                writer.WriteBytesFull(SumsByteArray[1]);
                writer.WriteBytesFull(SumsByteArray[2]);
                writer.WriteBytesFull(SumsByteArray[3]);
            }
        }

    }
}
