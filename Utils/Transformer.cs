using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Transformer {
    
    public static Vector2 GetVector2(this Vector3 vector) {
        return new Vector2(vector.x, vector.y);
    }
    public static Vector3 GetVector3(this Vector2 vector) {
        return new Vector3(vector.x, vector.y, 0);
    }

}
