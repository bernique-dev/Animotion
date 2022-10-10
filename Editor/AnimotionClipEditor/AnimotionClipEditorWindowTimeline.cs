using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AnimotionClipEditorWindowTimeline : AnimotionEditorWindowComponent {

        public static readonly float TIMELINE_GRADUATION_BAR_HEIGHT = 20;

        public static readonly float TIMELINE_GRADUATION_BAR_GRADUATION_HEIGHT = 5;
        public static readonly float TIMELINE_GRADUATION_BAR_BIG_GRADUATION_HEIGHT = 8;
        public static readonly float TIMELINE_GRADUATION_BAR_BIG_GRADUATION_TEXT_OFFSET = 18;

        public float timelineSpriteHeight {
            get {
                return timelineRect.height / 2;
            }
        }
        public Vector2 timelineSpriteSize {
            get {
                return new Vector2(25, 25);
            }
        }

        public float timelineGraduationInterval {
            get {
                return m_timelineGraduationInterval;
            }
            set {
                m_timelineGraduationInterval = Mathf.Clamp(value, 390, 12000);
            }
        }
        private float m_timelineGraduationInterval = 1300;

        public static readonly Color TIMELINE_GRADUATION_BAR_BACKGROUND_COLOR = new Color32(40, 40, 40, 255);
        public static readonly Color TIMELINE_BACKGROUND_COLOR = new Color32(60, 60, 60, 255);

        public static readonly float TIMELINE_GRADUATION_X_OFFSET = 22;
        public float timelinePositionXOffset = 150;
        public float timelineGraduationStart;
        public float timelineHeight = 200f;

        /// <summary>
        /// Top and left side's width (used for cursor detection when changing timeline's dimensions)
        /// </summary>
        public float vertexWidth = 12;

        /// <summary>
        /// Area detecting the cursor to change timeline's height
        /// </summary>
        public Rect topVertexRect {
            get {
                return new Rect(new Vector2(0, timelineRect.position.y - vertexWidth / 2), new Vector2(Screen.width, vertexWidth));
            }
        }

        /// <summary>
        /// Area detecting the cursor to change timeline's width
        /// </summary>
        public Rect leftVertexRect {
            get {
                return new Rect(timelineRect.position - new Vector2(vertexWidth / 2, 0), new Vector2(vertexWidth, timelineRect.height));
            }
        }

        /// <summary>
        /// Area containing the timeline's graduations
        /// </summary>
        public Rect timelineGraduationRect {
            get {
                return new Rect(timelineRect.xMin, timelineRect.yMin, timelineRect.xMax, TIMELINE_GRADUATION_BAR_HEIGHT);
                //return new Rect(timelineViewRect.xMin, timelineViewRect.yMin, timelineViewRect.xMax, TIMELINE_GRADUATION_BAR_HEIGHT);
            }
        }

        /// <summary>
        /// Area corresponding to the total timeline (Overflow accessible with scrollbars)
        /// </summary>
        public Rect timelineViewRect {
            get {
                return new Rect(timelineRect.position, new Vector2(timelineRect.width * 10, timelineRect.height - 13));
            }
        }

        /// <summary>
        /// Area containing the properties
        /// </summary>
        public Rect propertiesRect {
            get {
                // -1 => to see the border on the properties' right
                return new Rect(new Vector2(0, timelineRect.yMin), new Vector2(timelinePositionXOffset - 1, timelineHeight));
            }
        }

        /// <summary>
        /// Area of the timeline on screen
        /// </summary>
        public Rect timelineRect {
            set {
                m_timelineRect = new Rect(value.position + new Vector2(timelinePositionXOffset, 0), new Vector2(value.width - timelinePositionXOffset, value.height));
            }
            get {
                return m_timelineRect;
            }
        }

        /// <summary>
        /// Clip being edited
        /// </summary>
        public AnimotionClip animotionClip {
            get {
                return animotionEditorWindow.animotionClip;
            }
        }


        private Rect m_timelineRect;
        private Vector2 scrollPos;

        public bool draggingLeft;
        public bool draggingTop;

        /// <summary>
        /// EditorWindow being used
        /// </summary>
        public AnimotionClipEditorWindow animotionEditorWindow;

        public int frameDragged {
            get {
                return frameDataDragged != null ? frameDataDragged.frame : -1;
            }
        }
        public FrameData frameDataDragged = null;


        private void OnEnable() {
            frameDataDragged= null;
        }

        public override void Draw() {
            base.Draw();
            Vector2 rectBottomLeftCorner = new Vector2(timelineGraduationRect.xMin, timelineGraduationRect.yMax);
            Vector2 dynamicRectBottomLeftCorner = rectBottomLeftCorner + new Vector2(0, scrollPos.y);


            Handles.BeginGUI();

            EditorGUI.DrawRect(timelineRect, TIMELINE_BACKGROUND_COLOR); // Timeline Background
            EditorGUI.DrawRect(timelineGraduationRect, TIMELINE_GRADUATION_BAR_BACKGROUND_COLOR); // Timeline graduations background
            Handles.color = AnimotionClipEditorWindow.BORDER_COLOR;
            Handles.DrawLine(propertiesRect.min, new Vector2(timelineGraduationRect.xMax, timelineGraduationRect.yMin)); // Upper border (separation between timeline/properties and the rest)

            scrollPos = GUI.BeginScrollView(timelineRect, scrollPos, timelineViewRect, true, false);

            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle.fontSize = 10;

            //  Draws (if AnimotionClip selected) rect corresponding to its length on the timeline
            if (animotionClip != null) {
                EditorGUI.DrawRect(new Rect(rectBottomLeftCorner + new Vector2(TIMELINE_GRADUATION_X_OFFSET, 0), new Vector2(timelineGraduationInterval * animotionClip.length / 100, timelineViewRect.height)), TIMELINE_GRADUATION_BAR_BACKGROUND_COLOR);
            }

            // Draws the graduations
            float xOffset = Mathf.Clamp(TIMELINE_GRADUATION_X_OFFSET - timelineGraduationStart, 0, TIMELINE_GRADUATION_X_OFFSET) + timelineGraduationStart % timelineGraduationInterval;
            for (float i = 0; i <= timelineViewRect.width / timelineGraduationInterval; i = (float)Math.Round(i + (timelineGraduationInterval < 625 ? .1f : .01f), 2)) {

                double checkedValue = Mathf.RoundToInt((timelineGraduationStart + i) * 100);
                bool isIntPartMultipleOf5 = checkedValue % 5 == 0;

                float barHeight = isIntPartMultipleOf5 ? TIMELINE_GRADUATION_BAR_BIG_GRADUATION_HEIGHT : TIMELINE_GRADUATION_BAR_GRADUATION_HEIGHT;


                if (isIntPartMultipleOf5 || (timelineGraduationInterval >= 40 && timelineGraduationInterval < 500) || timelineGraduationInterval >= 3000) {
                    float textOffset = TIMELINE_GRADUATION_BAR_BIG_GRADUATION_TEXT_OFFSET;
                    string timeCodeText = FrameParser.GetParsedString((i + timelineGraduationStart) * 100, FrameParser.FrameFormat.OPTIONAL_MINUTE_OPTIONAL_EXTRA_ZERO_SECOND)
                                            //+ "\n"
                                            //+ checkedValue+  "\n" 
                                            //+ ((timelineGraduationStart + i) * 100).ToString("F2") 
                                            //+ "\n" + i
                                            //+ "\n" + isIntPartMultipleOf5
                                            ;
                    //string timeCodeText = "XX";
                    Vector2 textAnchorOffset = new Vector2(xOffset + i * timelineGraduationInterval - (i == 0 ? 0 : centeredStyle.fontSize * timeCodeText.Length / 6), -textOffset);
                    Handles.Label(dynamicRectBottomLeftCorner + textAnchorOffset, timeCodeText, centeredStyle);
                }

                //Small graduation in graduations
                Handles.color = Color.white;
                Handles.DrawLine(dynamicRectBottomLeftCorner + new Vector2(xOffset + i * timelineGraduationInterval, 0), dynamicRectBottomLeftCorner + new Vector2(xOffset + i * timelineGraduationInterval, -barHeight));
                //Graduations inside timeline
                Handles.color = new Color32(255, 255, 255, (byte)(isIntPartMultipleOf5 ? 60 : 20));
                Handles.DrawLine(dynamicRectBottomLeftCorner + new Vector2(xOffset + i * timelineGraduationInterval, 0), new Vector2(xOffset + dynamicRectBottomLeftCorner.x + i * timelineGraduationInterval, Screen.height + scrollPos.y));
            }

            Handles.color = AnimotionClipEditorWindow.BORDER_COLOR;
            Handles.DrawLine(timelineRect.min + new Vector2(scrollPos.x, 0), new Vector2(timelineRect.xMin, timelineRect.yMax + AnimotionClipEditorWindow.SCROLLBAR_HORIZONTAL_HEIGHT) + new Vector2(0, scrollPos.y)); // Separation between properties and timeline
            Handles.DrawLine(rectBottomLeftCorner, new Vector2(timelineViewRect.xMax, dynamicRectBottomLeftCorner.y)); // Graduation lower border (separation between graduation and the rest of the timeline)

            //Resets the timeline when cursor goes at the end
            if (animotionClip != null && animotionEditorWindow.framesPassed >= animotionClip.length) {
                animotionEditorWindow.time = 0;
            }

            Handles.color = Color.white;
            Handles.DrawLine(timelineRect.min + new Vector2(TIMELINE_GRADUATION_X_OFFSET + animotionEditorWindow.framesPassed * timelineGraduationInterval / 100, 0),
                            timelineRect.min + new Vector2(TIMELINE_GRADUATION_X_OFFSET + animotionEditorWindow.framesPassed * timelineGraduationInterval / 100, Screen.height + scrollPos.y)); //Draw the time cursor 


            if (animotionClip) {
                
                //Draws frames' sprite
                for (int i = 0; i <= Mathf.Max(animotionClip.length, animotionClip.GetFrameNumber()); i++) {
                    if (i < animotionClip.GetFrameNumber()) {
                        FrameData frameData = animotionClip.GetFrame(i);
                        if (frameData != null) {
                            if (frameData.sprite) {
                                Sprite sprite = animotionClip.GetFrame(i).sprite;
                                Texture texture = sprite.GetTexture();
                                GUI.DrawTexture(RectUtils.GetRect(timelineRect.min + new Vector2(TIMELINE_GRADUATION_X_OFFSET + i * timelineGraduationInterval / 100, timelineSpriteHeight), timelineSpriteSize * Mathf.Clamp(timelineGraduationInterval / 1000,1,Mathf.Infinity)), texture);
                            }
                        }
                    }
                }
            }

            //Draws dragged frame
            if (frameDataDragged != null) {
                if (frameDataDragged.sprite) {
                    Texture texture = frameDataDragged.sprite.GetTexture();
                    Rect rect = RectUtils.GetRect(timelineRect.min + new Vector2(TIMELINE_GRADUATION_X_OFFSET + GetFrameFromTimelinePosition(animotionEditorWindow.mousePosition) * timelineGraduationInterval / 100, timelineSpriteHeight), timelineSpriteSize * 6 / 5);
                    GUI.DrawTexture(rect, texture);
                }
            }

            GUI.EndScrollView();

            SetupSides();
            DrawProperties();

            Handles.EndGUI();
        }

        /// <summary>
        /// Draws the properties menu
        /// </summary>
        public void DrawProperties() {
            if (animotionClip) {
                Vector2 playButtonSize = new Vector2(25, 25);
                Rect playButton = RectUtils.GetRect(new Vector2(propertiesRect.width / 2, propertiesRect.yMin + 2.5f + playButtonSize.y / 2), playButtonSize);

                Texture2D buttonTexture = EditorGUIUtility.Load(animotionEditorWindow.isPlaying ? currentDirectory + "Editor/Sprites/media_player_ui_button_pause.png" : currentDirectory + "Editor/Sprites/media_player_ui_button_play.png") as Texture2D;

                //if (GUI.Button(playButton, animotionEditorWindow.isPlaying ? "Pause" : "Play", EditorStyles.miniButton)) {
                if (GUI.Button(playButton, buttonTexture)) {
                        animotionEditorWindow.isPlaying = !animotionEditorWindow.isPlaying;
                }

                Vector2 imageLengthDimensions = new Vector2(36, 20);
                Vector2 lengthFieldOffset = new Vector2(-4, -3);
                Rect imageLengthRect = new Rect(propertiesRect.max - imageLengthDimensions + lengthFieldOffset, imageLengthDimensions);
                animotionClip.length = EditorGUI.IntField(imageLengthRect, animotionEditorWindow.animotionClip.length);
            }
        }

        /// <summary>
        /// Sets up the sides detection visual system (changes the cursor when hovering the sides rect)
        /// </summary>
        private void SetupSides() {
            //EditorGUI.DrawRect(leftVertexRect, new Color32(255, 255, 255, 20));
            //EditorGUI.DrawRect(topVertexRect, new Color32(255, 255, 255, 20));
            EditorGUIUtility.AddCursorRect(leftVertexRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(topVertexRect, MouseCursor.ResizeVertical);
        }

        /// <summary>
        /// Processes the parameter's event
        /// </summary>
        /// <param name="e">Event processesd</param>
        public override void ProcessEvent(Event e) {
            base.ProcessEvent(e);
            if (timelineRect.Contains(e.mousePosition)) {
                if (animotionClip) {
                    for (int i = 0; i <= animotionClip.GetFrameNumber(); i++) {
                        FrameData frameData = animotionClip.GetFrame(i);
                        if (frameData != null) {
                            if (frameData.sprite) {
                                Rect rect = RectUtils.GetRect(timelineRect.min + new Vector2(TIMELINE_GRADUATION_X_OFFSET + i * timelineGraduationInterval / 100, timelineRect.height / 2), Vector2.one * 25);
                                if (rect.Contains(e.mousePosition) && e.type == EventType.MouseDown) {
                                    if (frameDragged < 0) {
                                        frameDataDragged = animotionClip.GetFrame(i);
                                        animotionClip.DeleteFrame(frameDragged);
                                    }
                                }
                            }
                        }
                    }
                }

                if (e.isMouse) {
                    // Right click for parameters
                    if (e.type == EventType.ContextClick) {
                        int frame = GetFrameFromTimelinePosition(e.mousePosition);
                        GenericMenu menu = new GenericMenu();
                        //menu.AddItem(new GUIContent(frame.ToString()), false, () => Debug.Log(frame));
                        if (animotionClip.IsThereFrame(frame)) {
                            menu.AddItem(new GUIContent("Erase Key"), false, () => animotionClip.DeleteFrame(frame));
                        }
                        if (animotionClip.IsThereFrame(frame)) {
                            menu.AddItem(new GUIContent("Delete Keys"), !animotionClip.IsEmpty(), () => animotionClip.DeleteAllFrames());
                        }
                        else {
                            menu.AddDisabledItem(new GUIContent("Delete Keys"));
                        }
                        menu.ShowAsContext();
                    }
                }

                if (e.type == EventType.DragPerform || e.type == EventType.DragUpdated) {
                    foreach (var obj in DragAndDrop.objectReferences) {
                        if (obj is Sprite) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            DragAndDrop.AcceptDrag();
                        }
                    }
                    // Object dropped on timeline
                    if (e.type == EventType.DragPerform) {
                        //Debug.Log(GetFrameFromTimelinePosition(e.mousePosition));
                        int frameFromTimeline = GetFrameFromTimelinePosition(e.mousePosition);

                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++) {

                            Sprite sprite = DragAndDrop.objectReferences[i] as Sprite;
                            if (sprite) {
                                animotionClip.AddFrame(frameFromTimeline + i, sprite);
                            }
                            else {
                                Debug.LogWarning("Not a sprite");
                            }
                        }
                    }
                }

                if (e.type == EventType.MouseDown) {
                    //  Moves times only if frame not selected
                    if (frameDataDragged == null) {
                        if (e.button == 0) {
                            animotionEditorWindow.time = GetRoundedTimeFromTimelinePosition(e.mousePosition);
                        }
                        if (e.button == 2) {
                            animotionEditorWindow.isPlaying = !animotionEditorWindow.isPlaying;
                        }
                    }
                }
            }

            if (timelineGraduationRect.Contains(e.mousePosition)) {
                if (e.type == EventType.ScrollWheel) {
                    //Debug.Log("Scroll " + e.delta);
                    float delta = e.delta.y + e.delta.y * timelineGraduationInterval / 100;
                    timelineGraduationInterval -= delta;
                    //scrollPos += new Vector2(-delta, 0);
                }
            }

            // Handles timeline dimensions' change
            if (e.button == 0) {
                if ((leftVertexRect.Contains(e.mousePosition) && e.type == EventType.MouseDown) || (draggingLeft && e.type == EventType.MouseDrag)) {
                    EditorGUI.DrawRect(leftVertexRect, Color.green);
                    timelinePositionXOffset = Mathf.Clamp(timelinePositionXOffset + e.delta.x, 0, Mathf.Infinity);
                    draggingLeft = true;
                }
                if ((topVertexRect.Contains(e.mousePosition) && e.type == EventType.MouseDown) || (draggingTop && e.type == EventType.MouseDrag)) {
                    EditorGUI.DrawRect(topVertexRect, Color.magenta);
                    draggingTop = true;
                    timelineHeight = Mathf.Clamp(timelineHeight - e.delta.y, 100, Screen.height * 2 / 3);
                }
            }



            if (e.type == EventType.MouseUp) {
                draggingLeft = false;
                draggingTop = false;
                if (frameDataDragged != null) {
                    animotionClip.AddFrame(GetFrameFromTimelinePosition(e.mousePosition), frameDataDragged);
                    frameDataDragged = null;
                }
            }

        }

        private void OnInspectorUpdate() {
        }

        private int GetFrameFromTimelinePosition(Vector2 pos) {
            return Mathf.RoundToInt(GetFloatFrameFromTimelinePosition(pos));
        }

        private float GetFloatFrameFromTimelinePosition(Vector2 pos) {
            return 100 * Mathf.Clamp((scrollPos.x - (Screen.width - timelineRect.width) + pos.x - TIMELINE_GRADUATION_X_OFFSET) / timelineGraduationInterval, 0, Mathf.Infinity);
        }


        private float GetRoundedTimeFromTimelinePosition(Vector2 pos) {
            return ((float)GetFrameFromTimelinePosition(pos)) / 60f;
        }

    }
}