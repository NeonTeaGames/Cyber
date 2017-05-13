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
            // Set scales
            float Scale = 2.0f / text.Width;
            Text.fontSize = text.FontSize;
            Text.characterSize = text.FontSize * Scale * 0.25f;

            // Fix newlines
            Text.text = text.Text.Replace("\\n", "\n");
            float CharacterWidth = FontUtil.GetCharacterWidth(Text.font, 
                Text.fontSize, Text.fontStyle);
            string FixedText = "";
            int CutAmount = 0;
            for (int i = 0; i < Text.text.Length; i++) {
                int RowLength = i - CutAmount;
                string Line = Text.text.Substring(CutAmount, RowLength).Trim() + "\n";
                int NextWordLength = 0;
                if (Text.text[i] == ' ' && i + 1 < Text.text.Length) {
                    NextWordLength = Text.text.Substring(i + 1).Split(' ')[0].Length;
                }
                if ((Line.Length + NextWordLength) * CharacterWidth > text.Width - text.OffsetX ||
                        Text.text[i] == '\n') {
                    FixedText += Line;
                    CutAmount = i;
                }
            }
            FixedText += Text.text.Substring(CutAmount).Trim();
            Text.text = FixedText;

            // Setup render textures
            RenderTexture TextTexture = RenderTexture.GetTemporary(text.Width, text.Height);
            RenderTexture OldRT = Camera.targetTexture;

            // Calculate positions
            float OffsetX = -text.Width / 2f;
            float OffsetY = -text.Height / 2f;
            if (text.Centered) {
                OffsetX = 0;
                Text.anchor = TextAnchor.UpperCenter;
            } else {
                Text.anchor = TextAnchor.UpperLeft;
            }
            Camera.orthographicSize = 1.0f * text.Height / text.Width;
            Camera.transform.localPosition = new Vector3(
                -(text.OffsetX + OffsetX) * Scale, 
                (text.OffsetY + OffsetY) * Scale);

            // Setup camera
            Camera.targetTexture = TextTexture;
            Camera.backgroundColor = text.Background;

            // *click*
            Camera.Render();
            Camera.targetTexture = OldRT;

            // Copy the result to a Texture2D
            OldRT = RenderTexture.active;
            Texture2D Result = new Texture2D(text.Width, text.Height);
            RenderTexture.active = TextTexture;
            Result.ReadPixels(new Rect(0, 0, Result.width, Result.height), 0, 0);
            Result.Apply();

            // Clean up
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
            string Hash = CreateHash(text);
            if (forceRender || !Cache.ContainsKey(Hash)) {
                Cache[Hash] = Singleton.RenderText(text);
            }
            return Cache[Hash];
        }

        private static string CreateHash(TextTextureProperties text) {
            return text.Text + "," + text.OffsetX + "," + text.OffsetY + "," + 
                text.Width + "," + text.Height + "," +
                text.FontSize + "," + text.Background.r + "," +
                text.Background.g + "," + text.Background.b;
        }
    }
}