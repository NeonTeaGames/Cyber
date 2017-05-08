using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Entities {
    
    /// <summary>
    /// A base class for all syncable components. An instance of 
    /// <see cref="SyncDB"/> will contain all of the game's synced components.
    /// </summary>
    public class SyncBase : MonoBehaviour {
        
        /// <summary>
        /// The ID this syncable component can be found with from its parent
        /// <see cref="SyncDB"/>.
        /// </summary>
        public uint ID;
    }
}