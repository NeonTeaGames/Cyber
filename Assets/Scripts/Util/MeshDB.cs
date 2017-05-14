using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {
 
    /// <summary>
    /// Mesh database, contains all the meshes in the game. This should be
    /// used if meshes are needed at runtime.
    /// </summary>
    public class MeshDB : MonoBehaviour {
        private static MeshDB Singleton;

        /// <summary>
        /// The meshes that can be used at runtime.
        /// </summary>
        public Mesh[] Meshes;

        /// <summary>
        /// Sets the Singleton.
        /// </summary>
        public MeshDB() {
            Singleton = this;
        }

        /// <summary>
        /// Get the mesh at the defined index.
        /// </summary>
        /// <returns>The mesh.</returns>
        /// <param name="index">Index.</param>
        public static Mesh GetMesh(int index) {
            return Singleton.Meshes[index];
        }
    }
}