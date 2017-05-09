
namespace Cyber.Entities {

    /// <summary>
    /// A struct that gives the following information to Syncer about a SyncBase type:
    /// Does it require a hash difference when syncing?
    /// How often should this sync be done?
    /// </summary>
    public struct SyncHandletype {

        /// <summary>
        /// Weather hash difference should be checked before this sync can be done.
        /// If true, will override <see cref="TickInterval"/>.
        /// </summary>
        public bool RequireHash;

        /// <summary>
        /// How often in ticks should the syncer sync this.
        /// </summary>
        public int TickInterval;

        /// <summary>
        /// A one-liner to create this struct.
        /// </summary>
        /// <param name="requireHash">Weather the SyncBase requires a hash to be synced.</param>
        /// <param name="tickInterval">How often should this be synced.</param>
        public SyncHandletype(bool requireHash, int tickInterval) {
            RequireHash = requireHash;
            TickInterval = tickInterval;
        }

    }
}