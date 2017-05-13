using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontUtil : MonoBehaviour {
    public static float GetCharacterWidth(Font font, int fontSize, FontStyle fontStyle) {
        CharacterInfo CharInfo;
        font.RequestCharactersInTexture("W", fontSize, fontStyle);
        font.GetCharacterInfo('W', out CharInfo, fontSize, fontStyle);
        return CharInfo.glyphWidth - 1;
    }
}
