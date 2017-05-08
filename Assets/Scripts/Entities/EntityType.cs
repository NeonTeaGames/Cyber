using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Entities {
    
    /// <summary>
    /// Types of entities. Every entity type has its own prefab, see 
    /// <see cref="Spawner"/>.
    /// </summary>
    public enum EntityType {
        
        /// <summary>
        /// Player character.
        /// </summary>
        PC,

        /// <summary>
        /// Non player character.
        /// </summary>
        NPC

    }
}