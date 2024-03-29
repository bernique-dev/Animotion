using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AniClipEditorWindow : AnimotionEditorWindowComponent {


        /// <summary>
        /// Interface's borders' color (Dark Mode only)
        /// </summary>
        public static readonly Color BORDER_COLOR = new Color32(30, 30, 30, 255);
        /// <summary>
        /// Menubar's buttons' width
        /// </summary>
        public static readonly float MENUBAR_BUTTON_WIDTH = 55f;

        /// <summary>
        /// Horizontal scrollbar's approximative height
        /// </summary>
        public static readonly float SCROLLBAR_HORIZONTAL_HEIGHT = 22;

        /// <summary>
        /// Rect containing the Timeline
        /// </summary>
        private Rect timelineRect {
            get {
                return new Rect(0, Screen.height - timeline.timelineHeight - SCROLLBAR_HORIZONTAL_HEIGHT, Screen.width, timeline.timelineHeight);
            }
        }

        private AniClipEditorWindowTimeline timeline;
        private AniClipEditorWindowPreview preview;

        /// <summary>
        /// Position defined by scrollbars
        /// </summary>
        public Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// Clip being edited
        /// </summary>
        public AniClip animotionClip;

        [MenuItem("Animotion/Animotion Clip Editor")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(AniClipEditorWindow), false, "Animotion Clip Editor");
        }

        /// <summary>
        /// Frames passed since the beginning of the clip
        /// </summary>
        public float framesPassed {
            get {
                return FrameParser.GetTotalFrames(time);
            }
        }

        public Vector2 mousePosition;

        /// <summary>
        /// Time passed since last frame
        /// </summary>
        private double editorDeltaTime = 0f;
        private double lastTimeSinceStartup = 0f;
        public float time;
        /// <summary>
        /// Is the animation being played in the window
        /// </summary>
        public bool isPlaying;

        private int clipIndex;

        public bool showDebug;


        private List<string> paths;
        private List<string> pathsWithoutExtension;

        public void OnGUI() {
            Draw();
        }

        private void OnEnable() {
            RefreshPaths();
        }

        private void OnFocus() {
            RefreshPaths();
        }

        public override void Draw() {
            base.Draw(); 
            if (isPlaying) time += (float)editorDeltaTime;
            mousePosition = Event.current.mousePosition;
            Repaint();
            ProcessEvent(Event.current);

            DrawMenuBar();

            if (!timeline) timeline = ScriptableObject.CreateInstance<AniClipEditorWindowTimeline>();
            timeline.animotionEditorWindow = this;
            timeline.timelineRect = timelineRect;
            timeline.ProcessEvent(Event.current);
            timeline.Draw();

            if (!preview) preview = ScriptableObject.CreateInstance<AniClipEditorWindowPreview>();
            preview.animotionEditorWindow = this;
            preview.ProcessEvent(Event.current);
            preview.Draw();

            SetEditorDeltaTime();


            if (showDebug) Handles.Label(new Vector2(9 * Screen.width / 10, Screen.height / 8), (isPlaying ? "is Playing" : "is not Playing") + "\n"

                                                                        + "Time:" + time
                                                                        + "\nInterval: " + timeline.timelineGraduationInterval
                                                                        + "\nTotal frames: " + FrameParser.GetTotalFrames(time)
                                                                        + "\nClip length: " + (animotionClip ? animotionClip.length : "no clip")
                                                                        + "\nFrames passed: " + framesPassed
                                                                        + "\nCurrent frame: " + FrameParser.GetParsedStringFromTime(time, FrameParser.FrameFormat.FORCED_MINUTE)
                                                                        + "\nCurrent time: " + TimeParser.GetParsedString(time)
                                                                        + "\n"
                                                                        + "\nframeDragged=" + timeline.frameDragged
                                                                        + "\nframeDataDragged=" + (timeline.frameDataDragged != null ? "(" + timeline.frameDataDragged.frame + ")" : "null"));
        }

        private void SetEditorDeltaTime() {
            if (lastTimeSinceStartup == 0f) {
                lastTimeSinceStartup = EditorApplication.timeSinceStartup;
            }
            editorDeltaTime = EditorApplication.timeSinceStartup - lastTimeSinceStartup;
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }

        public override void ProcessEvent(Event e) {
            if (animotionClip) EditorUtility.SetDirty(animotionClip);
            if (e.type == EventType.DragPerform || e.type == EventType.DragUpdated) {
                foreach (var obj in DragAndDrop.objectReferences) {
                    if (obj is AniClip) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();
                    }
                }
                // Object dropped on timeline
                if (e.type == EventType.DragPerform) {
                    foreach (var obj in DragAndDrop.objectReferences) {
                        if (obj is AniClip) animotionClip = obj as AniClip;
                    }
                }
            }
        }

        public void RefreshPaths() {
            paths = AssetDatabase.FindAssets("t: AniClip").ToList().Select(uuid => AssetDatabase.GUIDToAssetPath(uuid)).ToList();
            paths = paths.Where(p => p.Contains("Assets")).ToList();
            pathsWithoutExtension = paths.Select(a => a.Substring(6, a.Length - 6)).ToList();
        }

        /// <summary>
        /// Draw the menubar
        /// </summary>
        public void DrawMenuBar() {
            GUIStyle guiStyle = new GUIStyle();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            // TO CHECK
            {
                if (!animotionClip) {
                    if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(MENUBAR_BUTTON_WIDTH))) {
                        string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                        if (folderPath.Contains(".")) folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
                        if (folderPath == "") folderPath = "Assets";
                        string filePath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + "AnimotionClip.asset");

                        AniClip animotionClip = ScriptableObject.CreateInstance<AniClip>();
                        AssetDatabase.CreateAsset(animotionClip, filePath);
                        AssetDatabase.SaveAssets();

                        EditorGUIUtility.ExitGUI();
                    }
                }

                if (paths.Count <= 0) {
                    GUILayout.Label("No tree found in project");
                }
                else {
                    clipIndex = EditorGUILayout.Popup(clipIndex, pathsWithoutExtension.Select(a => a.Substring(1)).ToArray(), EditorStyles.toolbarDropDown);
                    AniClip tmpAnimotionClip = AssetDatabase.LoadAssetAtPath<AniClip>(paths[clipIndex]);
                    if (animotionClip != tmpAnimotionClip) {
                        animotionClip = tmpAnimotionClip;
                    }
                }
            }

            GUILayout.FlexibleSpace();
            showDebug = GUILayout.Toggle(showDebug, "Debug", EditorStyles.toolbarButton);
            if (animotionClip != null) {
                if (GUILayout.Button(animotionClip.name, EditorStyles.toolbarButton)) {
                    Selection.activeObject = animotionClip;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}
