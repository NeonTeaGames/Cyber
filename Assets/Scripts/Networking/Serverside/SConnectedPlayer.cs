using Cyber.Entities.SyncBases;
using UnityEngine.Networking;

namespace Cyber.Networking.Serverside {

    /// <summary>
    /// Represents a player on the server. This class is used by the server.
    /// The S stands for "Server".
    /// </summary>
    public class SConnectedPlayer {

        /// <summary>
        /// The player's connection ID.
        /// </summary>
        public readonly NetworkConnection Connection;

        /// <summary>
        /// The player's controlled character
        /// </summary>
        public Character Character;

        /// <summary>
        /// Create a connected player and give it a connection;
        /// </summary>
        /// <param name="connectionID">The player's connection</param>
        public SConnectedPlayer(NetworkConnection connectionID) {
            Connection = connectionID;
        }

    }
}
