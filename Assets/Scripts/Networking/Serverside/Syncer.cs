
using Cyber.Console;
using Cyber.Entities;
using Cyber.Entities.SyncBases;
using Cyber.Networking.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Networking.Serverside {

    /// <summary>
    /// Keeps stuff in-sync over at clients. Periodically collects stuff that needs to be synced and then sends them on the next 'tick.'
    /// </summary>
    public class Syncer : MonoBehaviour {

        private SyncDB Database;

        private int TickCounter = 0;
        private const float TickInterval = 1f / 10;

        private float TimeSinceLastTick = TickInterval;

        private List<int> QueuedSyncs = new List<int>();
        private List<int> DirtySyncBases = new List<int>();

        private int SyncPacketID = 0;

        /// <summary>
        /// Mark a SyncBase "Dirty", which makes it eligible to sync. 
        /// </summary>
        /// <param name="syncBaseID">The ID of the SyncBase. See <see cref="SyncBase.ID"/></param>
        public void DirtSyncBase(int syncBaseID) {
            if (DirtySyncBases.Contains(syncBaseID)) {
                return;
            }
            DirtySyncBases.Add(syncBaseID);
        }

        /// <summary>
        /// Queue a SyncBase directly, so it will be synced next time a sync tick is called.
        /// </summary>
        /// <param name="SyncBaseID">The ID of the SyncBase. See <see cref="SyncBase.ID"/></param>
        public void QueueSyncBase(int SyncBaseID) {
            QueuedSyncs.Add(SyncBaseID);
        }

        private void Start() {
            Database = GetComponent<SyncDB>();
        }

        private void Update() {
            TimeSinceLastTick += Time.deltaTime;
            if (TimeSinceLastTick >= TickInterval) {

                var Categorized = Database.GetCategorizedDatabase();
                List<int> checksummedIds = new List<int>();
                List<int> checksums = new List<int>();

                foreach (Type type in Categorized.Keys) {
                    SyncHandletype Handletype = Database.GetSyncHandletypes()[type];
                    if (TickCounter % Handletype.TickInterval == 0) {
                        foreach (int SyncBaseID in Categorized[type]) {
                            bool Contains = DirtySyncBases.Contains(SyncBaseID);
                            if (Contains == Handletype.RequireHash || Contains) {
                                QueueSyncBase(SyncBaseID);
                                if (Contains) {
                                    DirtySyncBases.Remove(SyncBaseID);
                                }
                            }
                            if (Handletype.RequireHash) {
                                checksummedIds.Add(SyncBaseID);
                                checksums.Add(Database.Get(SyncBaseID).GenerateChecksum());
                            }
                        }
                    }
                }

                if (QueuedSyncs.Count > 0) {
                    int[] SyncIDs = QueuedSyncs.ToArray();
                    SyncPkt SyncPacket = new SyncPkt(Database, SyncIDs, checksummedIds.ToArray(), checksums.ToArray(), SyncPacketID++);
                    Server.SendToAllByChannel(PktType.Sync, SyncPacket, NetworkChannelID.Unreliable);
                    
                    QueuedSyncs.Clear();
                }


                if (TickCounter < int.MaxValue) {
                    TickCounter++;
                } else {
                    TickCounter = 0;
                }
                TimeSinceLastTick -= TickInterval;
            }


        }

    }
}
