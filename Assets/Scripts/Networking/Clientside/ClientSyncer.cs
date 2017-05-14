
using Cyber.Networking.Messages;

namespace Cyber.Networking.Clientside {

    /// <summary>
    /// Syncer used on the clientside which sends sync packets from the client to the server containing their movement direction, rotation and possible checksums.
    /// </summary>
    public class ClientSyncer : Syncer {

        /// <summary>
        /// Creates a Client syncer, constructor only defined because of inheritance of <see cref="Syncer"/>.
        /// </summary>
        public ClientSyncer() : base(1f / 10) {}

        /// <summary>
        /// Performs a client sync tick.
        /// </summary>
        /// <param name="Tick">The number of the tick, which can be used to determine a few things, like when to sync certain things.</param>
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
