using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// Base class for all interactible entities in the world.
    /// </summary>
    public abstract class Interactable : SyncBase {

        /// <summary>
        /// All interactables should implement their interactions by overriding this.
        /// </summary>
        public abstract void Interact(SyncBase Trigger);
        
        /// <summary>
        /// Get Interaction information about this interactible.
        /// </summary>
        /// <returns>The Interaction information.</returns>
        public abstract InteractableSyncdata GetInteractableSyncdata();

    }
}