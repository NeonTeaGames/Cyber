using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {
 
    /// <summary>
    /// Mesh database, contains all the meshes in the game. This should be
    /// used if meshes are needed at runtime.
    /// </summary>
    public class MeshDB : MonoBehaviour {

        /// <summary>
        /// The meshes that can be used at runtime.
        /// </summary>
        public Mesh[] Meshes;
    }
}