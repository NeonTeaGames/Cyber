using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Console;

namespace Cyber.Util {

    /// <summary>
    /// Renders textures that have text written on them.
    /// </summary>
    public class TextTextureRenderer : MonoBehaviour {
        private static TextTextureRenderer Singleton;
        private static Dictionary<string, Texture2D> Cache =
            new Dictionary<string, Texture2D>();

        /// <summary>
        /// The camera the text is rendered with.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The text that is rendered on screen.
        /// </summary>
        public TextMesh Text;

        /// <summary>
        /// Sets the singleton.
        /// </summary>
        public TextTextureRenderer() {
            Singleton = this;
        }

        private Texture2D RenderText(TextTextureProperties text) {
            Text.text = text.Text.Replace("\\n", "\n");
            Text.fontSize = text.FontSize;
            Text.characterSize = 0.5f * text.FontSize / text.Width;

            RenderTexture TextTexture = RenderTexture.GetTemporary(text.Width, text.Height);
            RenderTexture OldRT = Camera.targetTexture;
            Camera.targetTexture = TextTexture;
            Camera.backgroundColor = text.Background;
            Camera.Render();
            Camera.targetTexture = OldRT;

            OldRT = RenderTexture.active;
            Texture2D Result = new Texture2D(text.Width, text.Height);
            RenderTexture.active = TextTexture;
            Result.ReadPixels(new Rect(0, 0, Result.width, Result.height), 0, 0);
            Result.Apply();

            RenderTexture.active = OldRT;
            RenderTexture.ReleaseTemporary(TextTexture);

            return Result;
        }

        /// <summary>
        /// Renders a texture and caches it so it can be used as a texture
        /// for a material (like a computer screen for example).
        /// </summary>
        /// <returns>The text.</returns>
        /// <param name="text">Text.</param>
        /// <param name="forceRender">Whether the texture should be rendered 
        /// again even if it's cached.</param>
        public static Texture2D GetText(TextTextureProperties text, bool forceRender = false) {
            if (forceRender || !Cache.ContainsKey(text.Text)) {
                Cache[text.Text] = Singleton.RenderText(text);
            }
            return Cache[text.Text];
        }
    }
}