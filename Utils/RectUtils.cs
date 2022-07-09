using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectUtils {

    public static Rect GetRect(Vector2 center, Vector2 size) {
        return new Rect(center - size / 2, size);
    }

    public static Rect GetRectFromCenter(this Vector2 center, Vector2 size) {
        return GetRect(center, size);
    }
    public static Rect GetRectFromSize(this Vector2 size, Vector2 center) {
        return GetRect(center, size);
    }

    public static Vector2 GetLeftSideCenter(this Rect rect) {
        return rect.center - new Vector2(rect.width / 2, 0);
    }

    public static Vector2 GetRightSideCenter(this Rect rect) {
        return rect.center + new Vector2(rect.width / 2, 0);
    }

    public static Vector2 GetTopSideCenter(this Rect rect) {
        return rect.center - new Vector2(0, rect.height / 2);
    }

    public static Vector2 GetDownSideCenter(this Rect rect) {
        return rect.center + new Vector2(0, rect.height / 2);
    }

}