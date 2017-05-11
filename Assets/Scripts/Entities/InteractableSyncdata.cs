
namespace Cyber.Entities {

    /// <summary>
    /// Contains information about the interactable SyncBase's syncing style.
    /// </summary>
    public struct InteractableSyncdata {

        /// <summary>
        /// Weather this interactable requires syncing or not.
        /// </summary>
        public bool RequiresSyncing;

        /// <summary>
        /// Weather interacting with this should send a TCP-packet or not.
        /// </summary>
        public bool PublicInteractions;
        
        /// <summary>
        /// Creates an InteractibleSyncdata struct.
        /// </summary>
        /// <param name="requiresSyncing">Weather this interactible requires syncing (like a door) or not (like a bell).</param>
        /// <param name="publicInteractions">Weather interacting with this interactible should send a TCP-packet (like a bell or a door) or not (like opening a screen where you can type).</param>
        public InteractableSyncdata(bool requiresSyncing, bool publicInteractions) {
            RequiresSyncing = requiresSyncing;
            PublicInteractions = publicInteractions;
        }

    }
}
