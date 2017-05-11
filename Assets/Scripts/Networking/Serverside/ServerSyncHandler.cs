
using Cyber.Entities.SyncBases;
using Cyber.Networking.Messages;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cyber.Networking.Serverside {

    /// <summary>
    /// The Server sync handler handles sync packets coming from the client, see <see cref="PktType.ClientSync"/>.
    /// </summary>
    public class ServerSyncHandler {

        private Dictionary<int, int> LastSyncIDReceived = new Dictionary<int, int>();
        private Dictionary<int, SConnectedPlayer> Players;

        /// <summary>
        /// Creates a nwe Server Sync handler for handling sync packets that are coming from the client.
        /// </summary>
        /// <param name="players"></param>
        public ServerSyncHandler(Dictionary<int, SConnectedPlayer> players) {
            Players = players;
        }

        /// <summary>
        /// Handle the given sync from a client. Must be checked to be <see cref="PktType.ClientSync"/> first.
        /// </summary>
        /// <param name="message"></param>
        public void HandleSyncPkt(NetworkMessage message) {
            ClientSyncPkt SyncPkt = new ClientSyncPkt();
            SyncPkt.Deserialize(message.reader);
            int LastID = -1;
            if (LastSyncIDReceived.ContainsKey(message.conn.connectionId)) {
                LastID = LastSyncIDReceived[message.conn.connectionId];
            }
            if (SyncPkt.ClientSyncID > LastID) {
                SyncPkt.ReadTheRest(message.reader);
                LastSyncIDReceived[message.conn.connectionId] = SyncPkt.ClientSyncID;

                Character PlayerCharacter = Players[message.conn.connectionId].Character;
                PlayerCharacter.Move(SyncPkt.MoveDirection);
                PlayerCharacter.SetRotation(SyncPkt.Rotation);
            }
            // Disregard the package, it's too old.
        }

        public void ClearConnectionFromSyncDict(int connectionID) {
            LastSyncIDReceived.Remove(connectionID);
        }

    }
}
