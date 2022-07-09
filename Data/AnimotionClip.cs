using System.Linq;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="Animotion/Animotion Clip")]
public class AnimotionClip : ScriptableObject {
    
    public int length;
    public bool loop = true;
    [SerializeField] private List<FrameData> frameDataList;

    private int referenceMax {
        get {
            return Mathf.Max(length, frameDataList.Count);
        }
    }
    private int referenceMin {
        get {
            return Mathf.Min(length, frameDataList.Count);
        }
    }

    public void BalanceFrames(int frame) {
        if (frame >= frameDataList.Count) {
            while (frameDataList.Count <= frame) {
                frameDataList.Add(new FrameData(frameDataList.Count));
            }
        }
    }

    public void AddFrame(int frame, Sprite sprite) {
        BalanceFrames(frame);
        frameDataList[frame].sprite = sprite;
        frameDataList[frame].frame = frame;
    }

    public void AddFrame(int frame, FrameData frameData) {
        BalanceFrames(frame);
        frameDataList[frame] = frameData;
        frameDataList[frame].frame = frame;
    }

    public FrameData GetFrame(int frame) {
        return frameDataList.Count > frame ? frameDataList[frame] : null;
    }

    public FrameData GetLastFrame(int frame) {
        for (int i = Mathf.Min(frame, frameDataList.Count - 1); i >= 0 ; i--) {
            //Debug.Log(frame + "? - " + frameDataList.Count);
            if (frameDataList[i] != null) {
                if (frameDataList[i].sprite) {
                    return frameDataList[i];
                }
            }
        }
        return null;
    }

    public int GetFrameNumber() {
        return frameDataList.Count;
    }

    public void DeleteFrame(int frame) {
        if (frame < frameDataList.Count) frameDataList[frame] = null;
    }

    public void DeleteAllFrames() {
        frameDataList = new List<FrameData>();
    }

}
