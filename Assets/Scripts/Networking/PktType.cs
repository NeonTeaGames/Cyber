
namespace Cyber.Networking {

    /// <summary>
    /// Types of custom created packets.
    /// </summary>
    public class PktType {

        /// <summary>
        /// Test Text Message for sending stuff like chat.
        /// </summary>
        public const short TextMessage = 200;

        /// <summary>
        /// Packet containing identification details, server's given ID and if the ID belongs to you or not.
        /// </summary>
        public const short Identity = 201;

        /// <summary>
        /// Packet containing the identification details about everyone on the server before the client connected.
        /// </summary>
        public const short MassIdentity = 202;

        /// <summary>
        /// Packet containing necessary information about spawning an entity.
        /// </summary>
        public const short SpawnEntity = 203;

        /// <summary>
        /// Packet informing a creature must be moved or stopped.
        /// This Packet can be used by the client to inform a request to move or the server
        /// to inform actual movement.
        /// </summary>
        public const short MoveCreature = 204;

        /// <summary>
        /// Packet containing sync data.
        /// </summary>
        public const short SyncPacket = 205;

        /// <summary>
        /// Packet telling that someone has made an interactive remark.
        /// </summary>
        public const short InteractPkt = 206;

    }
}