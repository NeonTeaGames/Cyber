
using UnityEngine;

namespace Cyber.Networking {

    /// <summary>
    /// An abstract syncer, used by <see cref="Serverside.ServerSyncer"/> and <see cref="Clientside.ClientSyncer"/>.
    /// </summary>
    public abstract class Syncer : MonoBehaviour {

        private int TickCounter = 0;

        private readonly float TickInterval;
        private float TimeSinceLastTick = 0;

        private int SyncPacketID = 0;

        /// <summary>
        /// Initializes the syncer, basically just sets the <see cref="TickInterval"/>.
        /// </summary>
        /// <param name="tickInterval"></param>
        public Syncer(float tickInterval) {
            TickInterval = tickInterval;
            TimeSinceLastTick = tickInterval;
        }

        /// <summary>
        /// Returns the next ID for a sync packet.
        /// </summary>
        /// <returns></returns>
        public int NextSyncID() {
            return SyncPacketID++;
        }

        /// <summary>
        /// Performs a tick on the syncer, called from <see cref="Update"/>.
        /// </summary>
        /// <param name="Tick">The tick number, used to e.g. determine when to sync certain things.</param>
        public abstract void PerformTick(int Tick);

        private void Update() {
            TimeSinceLastTick += Time.deltaTime;

            if (TimeSinceLastTick > TickInterval) {
                PerformTick(TickCounter);

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
