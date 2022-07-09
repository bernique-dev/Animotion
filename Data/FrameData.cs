using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FrameData {

    public int frame = -1;
    public Sprite sprite;
    public List<ColliderData> colliderDataList;

    public FrameData(int _frame) {
        frame = _frame;
    }

}
