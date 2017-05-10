using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// Base class for all interactible entities in the world.
    /// </summary>
    public abstract class Interactable : SyncBase {

        /// <summary>
        /// All interactables should implement their interactions by overriding this.
        /// </summary>
        public abstract void Interact();

    }
}