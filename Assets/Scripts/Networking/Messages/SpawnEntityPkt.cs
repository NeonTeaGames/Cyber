
using Cyber.Entities;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {
    
    /// <summary>
    /// Packet which tells the client to spawn an entity corresponding to the data inside.
    /// </summary>
    public class SpawnEntityPkt : MessageBase {

        /// <summary>
        /// The type of entity to spawn.
        /// </summary>
        public EntityType EntityType;
        /// <summary>
        /// The position of the entity, where it should be spawned
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// The List of Sync ID's for the entity.
        /// </summary>
        public int[] SyncBaseIDList;

        /// <summary>
        /// Connection ID of the owner of this entity
        /// </summary>
        public int OwnerID;

        /// <summary>
        /// Create a packet of information about spawning an entity to send the clients.
        /// </summary>
        /// <param name="entityType">Type of entity to spawn.</param>
        /// <param name="position">Position where the entity will be spawned.</param>
        /// <param name="syncIDList">List of Sync ID's for the entity.</param>
        /// <param name="ownerID">The Connection ID who owns this entity (or -1).</param>
        public SpawnEntityPkt(EntityType entityType, Vector3 position, int[] syncIDList, int ownerID) {
            EntityType = entityType;
            Position = position;
            SyncBaseIDList = syncIDList;
            OwnerID = ownerID;
        }
        
        /// <summary>
        /// Parameter-less constructor using when deserializing the message.
        /// </summary>
        public SpawnEntityPkt() {

        }

        /// <summary>
        /// Used to deserialize a message received via networking.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            EntityType = (EntityType) reader.ReadInt16();
            Position = reader.ReadVector3();
            OwnerID = reader.ReadInt32();

            byte[][] ByteArray = new byte[4][];
            ByteArray[0] = reader.ReadBytesAndSize();
            ByteArray[1] = reader.ReadBytesAndSize();
            ByteArray[2] = reader.ReadBytesAndSize();
            ByteArray[3] = reader.ReadBytesAndSize();
            SyncBaseIDList = NetworkHelper.DeserializeIntArray(ByteArray);
        }

        /// <summary>
        /// Used to serialize the message before it is sent.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write((short) EntityType);
            writer.Write(Position);
            writer.Write(OwnerID);

            byte[][] ByteArray = NetworkHelper.SerializeIntArray(SyncBaseIDList);
            writer.WriteBytesFull(ByteArray[0]);
            writer.WriteBytesFull(ByteArray[1]);
            writer.WriteBytesFull(ByteArray[2]);
            writer.WriteBytesFull(ByteArray[3]);
        }

    }
}
