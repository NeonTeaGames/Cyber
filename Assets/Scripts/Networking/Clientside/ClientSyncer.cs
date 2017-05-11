
using Cyber.Networking.Messages;
using UnityEngine;

namespace Cyber.Networking.Clientside {

    /// <summary>
    /// Syncer used on the clientside which sends sync packets from the client to the server containing their movement direction, rotation and possible checksums.
    /// </summary>
    public class ClientSyncer : MonoBehaviour {

        private const int ChecksumInterval = 10; // Every 10 ticks -> 1 per second

        private const float TickInterval = 1 / 10f;
        private float TimeSinceLastTick = TickInterval;
        private int TickCounter = 0;

        private int SyncIDCounter = 0;

        private void Update() {
            TimeSinceLastTick += Time.deltaTime;
            if (TimeSinceLastTick >= TickInterval) {

                ClientSyncPkt SyncPkt = new ClientSyncPkt();

                if (TickCounter % ChecksumInterval == 0) {
                    // Add checksums
                }

                var PlayerCharacter = Client.GetConnectedPlayer().Character;
                SyncPkt.ClientSyncID = SyncIDCounter++;
                SyncPkt.MoveDirection = PlayerCharacter.GetMovementDirection();
                SyncPkt.Rotation = PlayerCharacter.GetRotation();

                Client.SendByChannel(PktType.ClientSync, SyncPkt, NetworkChannelID.Unreliable);

                if (TickCounter < int.MaxValue) {
                    TickCounter += 1;
                } else {
                    TickCounter = 0;
                }
                TimeSinceLastTick -= TickInterval;
            }
        }

    }
}
