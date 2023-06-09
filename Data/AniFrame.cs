using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AniFrame {

    public int frame = -1;
    public Sprite sprite;
    public List<AniCollider> colliderDataList;

    public AniFrame(int _frame) {
        frame = _frame;
    }

}
