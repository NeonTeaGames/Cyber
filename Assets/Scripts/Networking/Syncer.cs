
using UnityEngine;

namespace Cyber.Networking {


    public abstract class Syncer : MonoBehaviour {

        private int TickCounter = 0;

        private readonly float TickInterval;
        private float TimeSinceLastTick = 0;

        private int SyncPacketID = 0;

        public Syncer(float tickInterval) {
            TickInterval = tickInterval;
            TimeSinceLastTick = tickInterval;
        }

        public int NextSyncID() {
            return SyncPacketID++;
        }

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

        public abstract void PerformTick(int Tick);

    }
}
