using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {
 
    /// <summary>
    /// Prefab database, contains all the prefabs that might be needed at
    /// runtime. Intended to be called through the static functions.
    /// </summary>
    public class PrefabDB : MonoBehaviour {
        private static PrefabDB Singleton;

        /// <summary>
        /// The prefabs that can be used at runtime.
        /// </summary>
        public GameObject[] Prefabs;

        /// <summary>
        /// Sets the Singleton.
        /// </summary>
        public PrefabDB() {
            Singleton = this;
        }

        /// <summary>
        /// Creates a new prefab with the given position, rotation and parent.
        /// </summary>
        /// <returns>The prefab.</returns>
        /// <param name="index">Index.</param>
        /// <param name="pos">The position (local, if parent is not null).</param>
        /// <param name="rot">The rotation (local, if parent is not null).</param>
        /// <param name="parent">The parent transform.</param>
        public static GameObject Create(int index, Transform parent, 
                Vector3 pos = new Vector3(), Quaternion rot = new Quaternion()) {
            GameObject Result = Instantiate(Singleton.Prefabs[index], pos, rot, parent);
            if (parent != null) {
                Result.transform.localPosition = pos;
                Result.transform.localRotation = rot;
            }
            return Result;
        }
    }
}