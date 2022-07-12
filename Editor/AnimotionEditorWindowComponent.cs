using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using System.IO;
using System.Reflection;

namespace Animotion {
    public class AnimotionEditorWindowComponent : EditorWindow {

        public string currentDirectory {
            get {
            
                Assembly assembly = Assembly.GetExecutingAssembly();
                UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);

                return packageInfo != null ? "Packages/com.bernique.animotion/" : "Assets/Animotion/";
            }
        }

        public virtual void Draw() { }

        public virtual void ProcessEvent(Event e) { }

    }
}
