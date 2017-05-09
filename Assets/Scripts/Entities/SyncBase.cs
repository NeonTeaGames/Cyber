using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Entities {
    
    /// <summary>
    /// A base class for all syncable components. An instance of 
    /// <see cref="SyncDB"/> will contain all of the game's synced components.
    /// </summary>
    public abstract class SyncBase : MonoBehaviour {
        
        /// <summary>
        /// The ID this syncable component can be found with from its parent
        /// <see cref="SyncDB"/>.
        /// </summary>
        public int ID;

        /// <summary>
        /// Return the Sync Handletype information for <see cref="Syncer"/>.
        /// </summary>
        /// <returns>Sync Handletype containing sync information.</returns>
        public abstract SyncHandletype GetSyncHandletype();

        /// <summary>
        /// Deserializes this SyncBase for further use.
        /// </summary>
        /// <param name="reader"></param>
        public abstract void Deserialize(NetworkReader reader);

        /// <summary>
        /// Serialize this SyncBase into a sync packet.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Serialize(NetworkWriter writer);

        /// <summary>
        /// Generates a checksum of the contents, or 0 if no checksum is overriden.
        /// </summary>
        /// <returns>The integer checksum.</returns>
        public virtual int GenerateChecksum() {
            return 0;
        }
    }
}