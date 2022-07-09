using UnityEditor;
using UnityEngine;

public static class SpriteUtils {

    public static Sprite GetSprite(Texture2D texture) {
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rec, new Vector2(0, 0), 1);
    }

    public static Texture2D GetTexture(this Sprite sprite) {
        Rect rect = sprite.rect;
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height);
        Color[] data = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        tex.SetPixels(data);
        tex.filterMode = FilterMode.Point;
        tex.Apply(true);
        return tex;
    }
}