using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AnimotionEditorWindowComponent : EditorWindow {

        public virtual void Draw() { }

        public virtual void ProcessEvent(Event e) { }

    }
}
