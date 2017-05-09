using Cyber.Entities;

namespace Cyber.Networking.Clientside {

    /// <summary>
    /// Represents a connected player on the clientside. This class is used by clients.
    /// The C stands for "Client".
    /// </summary>
    class CConnectedPlayer {

        /// <summary>
        /// Connection ID on the global perspective.
        /// </summary>
        public readonly int ConnectionID;

        /// <summary>
        /// The player's controlled character.
        /// </summary>
        public Character Character;

        public CConnectedPlayer(int connectionID) {
            ConnectionID = connectionID;
        }

    }
}
