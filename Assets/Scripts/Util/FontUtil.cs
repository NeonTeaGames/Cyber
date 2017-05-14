using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {

    /// <summary>
    /// Conjures useful information about fonts.
    /// </summary>
    public class FontUtil : MonoBehaviour {

        /// <summary>
        /// Gets the width of the character.
        /// </summary>
        /// <returns>The character width.</returns>
        /// <param name="font">Font.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="fontStyle">Font style.</param>
        public static float GetCharacterWidth(Font font, int fontSize, FontStyle fontStyle) {
            CharacterInfo CharInfo;
            font.RequestCharactersInTexture("W", fontSize, fontStyle);
            font.GetCharacterInfo('W', out CharInfo, fontSize, fontStyle);
            return CharInfo.glyphWidth - 1;
        }
    }
}