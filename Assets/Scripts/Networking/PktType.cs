
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
        /// This packet contains an <see cref="IntListPkt"/>.
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
        /// Packet containing sync data from the server.
        /// </summary>
        public const short Sync = 205;

        /// <summary>
        /// Packet telling that someone has made an interactive remark.
        /// </summary>
        public const short Interact = 206;

        /// <summary>
        /// Packet containing an id list of static objects existing in the ready game.
        /// This packet contains an <see cref="IntListPkt"/>.
        /// </summary>
        public const short StaticObjectIds = 207;

        /// <summary>
        /// Packet containing sync data from the client to the server, like movement direction and rotation.
        /// </summary>
        public const short ClientSync = 208;

        /// <summary>
        /// Packet informing the reciever that the client either wishes to disconnect from the server, or is disconnected.
        /// </summary>
        public const short Disconnect = 209;

        /// <summary>
        /// Packet containing SyncBase id's of the SyncBases which failed the checksum test.
        /// </summary>
        public const short FailedChecksums = 210;

        /// <summary>
        /// Packet containing an inventory action and an int relating somehow to this action.
        /// </summary>
        public const short InventoryAction = 211;

    }
}