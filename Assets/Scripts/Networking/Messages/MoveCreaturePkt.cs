using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Packet informing the client to move a creature, or the server to request change in movement.
    /// More information at <see cref="PktType.MoveCreature"/>
    /// </summary>
    public class MoveCreaturePkt : MessageBase {

        /// <summary>
        /// Direction of the new movement (or (0, 0, 0) for stopping)
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// SyncBase ID of the Creature to be moved.
        /// </summary>
        public int SyncBaseID;

        /// <summary>
        /// Time when server received this request. Used for compensating ping.
        /// </summary>
        public double Timestamp;

        /// <summary>
        /// Creates a MoveCreaturePkt which contains the direction of desired movement (or (0, 0, 0) for stopping)
        /// </summary>
        /// <param name="direction">Direction of movement</param>
        /// <param name="syncBaseId">SyncBase ID of the Creature.</param>
        public MoveCreaturePkt(Vector3 direction, int syncBaseId) {
            Direction = direction;
            SyncBaseID = syncBaseId;
        }
        
        /// <summary>
        /// Parameter-less constructor using when deserializing the message.
        /// </summary>
        public MoveCreaturePkt() {

        }

        /// <summary>
        /// Used to deserialize a message received via networking.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            Direction = reader.ReadVector3();
            SyncBaseID = reader.ReadInt32();
            Timestamp = reader.ReadDouble();
        }

        /// <summary>
        /// Used to serialize the message before it is sent.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(Direction);
            writer.Write(SyncBaseID);
            writer.Write(Timestamp);
        }

    }
}
