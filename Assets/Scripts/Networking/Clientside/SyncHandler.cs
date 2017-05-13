
using Cyber.Console;
using Cyber.Entities;
using Cyber.Entities.SyncBases;
using Cyber.Networking.Messages;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cyber.Networking.Clientside {
    
    /// <summary>
    /// A Short clientside class for handling sync packages.
    /// It simply keeps track of sync-packages and will not apply them if they are too old.
    /// </summary>
    public class SyncHandler {

        private SyncDB SyncDB;
        private int LatestSyncID = -1;

        /// <summary>
        /// Creates the SyncHandler with SyncDB.
        /// </summary>
        /// <param name="syncDB"></param>
        public SyncHandler(SyncDB syncDB) {
            SyncDB = syncDB;
        }

        /// <summary>
        /// Handle a given Network message. Must be checked to be <see cref="PktType.Sync"/> first.
        /// </summary>
        /// <param name="message"></param>
        public void HandleSyncPkt(NetworkMessage message) {
            SyncPkt SyncPacket = new SyncPkt(SyncDB);
            SyncPacket.Deserialize(message.reader);
            if (LatestSyncID < SyncPacket.SyncPacketID) {
                LatestSyncID = SyncPacket.SyncPacketID;
                SyncPacket.ApplySync(message.reader);

                int[] SyncBases = SyncPacket.ChecksummedSyncBases;
                int[] Checksums = SyncPacket.Checksums;
                if (SyncBases.Length > 0) {
                    List<int> FailedSyncBases = new List<int>();
                    for (int i = 0; i < SyncBases.Length; i++) {
                        SyncBase SyncBase = SyncDB.Get(SyncBases[i]);
                        if (SyncBase.GenerateChecksum() != Checksums[i]) {
                            FailedSyncBases.Add(SyncBase.ID);
                        }
                    }

                    Client.Send(PktType.FailedChecksums, new IntListPkt(FailedSyncBases.ToArray()));
                }
            }
            // Otherwise disregard the sync.

        }

    }
}
