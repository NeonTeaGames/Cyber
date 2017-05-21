using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {
    
    /// <summary>
    /// Utility functions for mesh-related things.
    /// </summary>
    public class MeshUtil : MonoBehaviour {

        /// <summary>
        /// Gets the bounds of a gameobject or its children with a mesh.
        /// </summary>
        /// <returns>The bounds of the object.</returns>
        /// <param name="obj">Object.</param>
        public static Bounds GetMeshBounds(GameObject obj) {
            MeshFilter Mesh = GetMeshFilter(obj);
            if (Mesh != null) {
                return Mesh.mesh.bounds;
            }
            SkinnedMeshRenderer SkinnedMesh = GetSkinnedMeshRenderer(obj);
            if (SkinnedMesh != null) {
                return SkinnedMesh.sharedMesh.bounds;
            }
            return new Bounds();
        }

        /// <summary>
        /// Sets the material in the objects or its children.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="mat">Mat.</param>
        public static void SetMaterial(GameObject obj, Material mat) {
            MeshRenderer MeshRenderer = MeshUtil.GetMeshRenderer(obj);
            if (MeshRenderer != null) {
                Material[] NewMats = new Material[MeshRenderer.materials.Length];
                for (int j = 0; j < MeshRenderer.materials.Length; j++) {
                    NewMats[j] = mat;
                }
                MeshRenderer.materials = NewMats;
            }
            SkinnedMeshRenderer SkinnedMeshRenderer = MeshUtil.GetSkinnedMeshRenderer(obj);
            if (SkinnedMeshRenderer != null) {
                Material[] NewMats = new Material[SkinnedMeshRenderer.materials.Length];
                for (int j = 0; j < SkinnedMeshRenderer.materials.Length; j++) {
                    NewMats[j] = mat;
                }
                SkinnedMeshRenderer.materials = NewMats;
            }
        }

        /// <summary>
        /// Gets a MeshFilter in a gameobject or its children.
        /// </summary>
        /// <returns>The MeshFilter in object.</returns>
        /// <param name="obj">Object.</param>
        public static MeshFilter GetMeshFilter(GameObject obj) {
            MeshFilter Mesh = obj.GetComponent<MeshFilter>();
            if (Mesh == null) {
                // No mesh found, search children
                Mesh = obj.GetComponentInChildren<MeshFilter>();
            }
            return Mesh;
        }

        /// <summary>
        /// Gets a MeshRenderer in a gameobject or its children.
        /// </summary>
        /// <returns>The MeshRenderer in object.</returns>
        /// <param name="obj">Object.</param>
        public static MeshRenderer GetMeshRenderer(GameObject obj) {
            MeshRenderer Mesh = obj.GetComponent<MeshRenderer>();
            if (Mesh == null) {
                // No mesh found, search children
                Mesh = obj.GetComponentInChildren<MeshRenderer>();
            }
            return Mesh;
        }

        /// <summary>
        /// Gets a SkinnedMeshRenderer in a gameobject or its children.
        /// </summary>
        /// <returns>The SkinnedMeshRenderer in object.</returns>
        /// <param name="obj">Object.</param>
        public static SkinnedMeshRenderer GetSkinnedMeshRenderer(GameObject obj) {
            SkinnedMeshRenderer Mesh = obj.GetComponent<SkinnedMeshRenderer>();
            if (Mesh == null) {
                // No mesh found, search children
                Mesh = obj.GetComponentInChildren<SkinnedMeshRenderer>();
            }
            return Mesh;
        }
    }
}