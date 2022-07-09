using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colors {

    public static Color GetRandomColor() {
        return new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    public static Texture2D GetTexture(this Color col, int width, int height) {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    public static Texture2D GetTexture(this Color col, float width, float height) {
        return col.GetTexture((int)width, (int)height);
    }

    public static Texture2D GetTexture(this Color col, Vector2Int pos) {
        return col.GetTexture(pos.x, pos.y);
    }

    public static Texture2D GetTexture(this Color col, Vector2 pos) {
        Vector2Int posInt = Vector2Int.FloorToInt(pos);
        return col.GetTexture(posInt.x, posInt.y);
    }

    public static Texture2D GetTexture(this Color col, Rect rect) {
        return col.GetTexture(rect.width, rect.height);
    }

}
