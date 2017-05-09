
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
    }
}