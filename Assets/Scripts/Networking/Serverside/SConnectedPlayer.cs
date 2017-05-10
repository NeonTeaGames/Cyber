using Cyber.Entities.SyncBases;

namespace Cyber.Networking.Serverside {

    /// <summary>
    /// Represents a player on the server. This class is used by the server.
    /// The S stands for "Server".
    /// </summary>
    class SConnectedPlayer {

        /// <summary>
        /// The player's connection ID.
        /// </summary>
        public readonly int ConnectionID;

        /// <summary>
        /// The player's controlled character
        /// </summary>
        public Character Character;

        /// <summary>
        /// Create a connected player and give it a connection ID;
        /// </summary>
        /// <param name="connectionID">The player's connection ID</param>
        public SConnectedPlayer(int connectionID) {
            ConnectionID = connectionID;
        }

    }
}
