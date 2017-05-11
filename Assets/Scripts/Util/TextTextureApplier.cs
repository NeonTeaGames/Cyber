using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Util;

namespace Cyber.Util {

    /// <summary>
    /// Applies a texture to a specificed material containing specified text.
    /// </summary>
    public class TextTextureApplier : MonoBehaviour {

        /// <summary>
        /// The text and related properties that defines the texture that 
        /// will be applied.
        /// </summary>
        public TextTextureProperties TextProperties = new TextTextureProperties("");

        /// <summary>
        /// The mesh that has the material that should use this texture.
        /// </summary>
        public MeshRenderer Mesh;

        /// <summary>
        /// The index of the material that should use this texture.
        /// </summary>
        public int MaterialIndex = 0;

        /// <summary>
        /// Whether the Emissive or Albedo map is set.
        /// </summary>
        public bool Emissive = true;

        /// <summary>
        /// The brightness of the texture if this is <see cref="Emissive"/>.
        /// </summary>
        public float Brightness = 1f;

        private TextTextureProperties LastText = new TextTextureProperties("");
        private Material Material;
        private bool Dirty = true;

        /// <summary>
        /// Sets the text properties.
        /// </summary>
        /// <param name="Props">Properties.</param>
        public void SetTextProperties(TextTextureProperties Props) {
            TextProperties = Props;
            Dirty = true;
        }

        private void Start() {
            Material = Mesh.materials[MaterialIndex];
        }

        private void Update() {
            Texture2D Tex;
            if (Dirty && (Tex = TextTextureRenderer.GetText(TextProperties)) != null) {
                if (Emissive) {
                    Material.SetTexture("_EmissionMap", Tex);
                    Material.SetColor("_EmissionColor", 
                        new Color(Brightness, Brightness, Brightness));
                } else {
                    Material.SetTexture("_MainTex", Tex);
                }
                Dirty = false;
            }
        }
    }
}