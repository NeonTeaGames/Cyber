
using Cyber.Networking.Messages;

namespace Cyber.Networking.Clientside {

    /// <summary>
    /// Syncer used on the clientside which sends sync packets from the client to the server containing their movement direction, rotation and possible checksums.
    /// </summary>
    public class ClientSyncer : Syncer {

        public ClientSyncer() : base(1f / 10) {}

        public override void PerformTick(int Tick) {

            ClientSyncPkt SyncPkt = new ClientSyncPkt();

            var PlayerCharacter = Client.GetConnectedPlayer().Character;
            SyncPkt.ClientSyncID = NextSyncID();
            SyncPkt.MoveDirection = PlayerCharacter.GetMovementDirection();
            SyncPkt.Rotation = PlayerCharacter.GetRotation();

            Client.SendByChannel(PktType.ClientSync, SyncPkt, NetworkChannelID.Unreliable);
        }

    }
}
