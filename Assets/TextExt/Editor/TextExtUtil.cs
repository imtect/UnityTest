using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextExtUtil : Editor {

    [MenuItem("GameObject/UI/TextExt")]
    public static void CreateTextExt() {
        GameObject textRoot = new GameObject("Text", typeof(RectTransform), typeof(TextExt));
        ResetInCanvasFor((RectTransform)textRoot.transform);
        var textExt = textRoot.GetComponent<TextExt>();
        textExt.text = "TextExt";
        textExt.color = Color.white;
        textExt.alignment = TextAnchor.MiddleCenter;
        textRoot.transform.localPosition = Vector3.zero;
        textRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
    }

    public static void TextSpacingGUI(SerializedProperty m_UseTextSpacing, SerializedProperty m_TextSpacing, ref bool m_TextSpacingPanelOpen) {
        LayoutF(() => {
            EditorGUILayout.PropertyField(m_UseTextSpacing);
            if (m_UseTextSpacing.boolValue) {
                Space();
                LayoutH(() => {
                    EditorGUI.PropertyField(GUIRect(0, 18), m_TextSpacing, new GUIContent());
                });
            }
        }, "Text Spacing", ref m_TextSpacingPanelOpen, true);
    }

    private static void LayoutF(System.Action action,string label, ref bool open,bool box = false) {
        bool _open = open;
        LayoutV(() => {
            _open = GUILayout.Toggle(
                _open,
                label,
                GUI.skin.GetStyle("foldout"),
                GUILayout.ExpandWidth(true),
                GUILayout.Height(18)
            );
            if (_open) {
                action();
            }
        }, box);
        open = _open;
    }

    private static void Space(float space = 4f) {
        GUILayout.Space(space);
    }

    private static Rect GUIRect(float width,float height) {
        return GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(width <= 0), GUILayout.ExpandHeight(height <= 0));
    }

    private static void LayoutV(System.Action action,bool box = false) {
        if (box) {
            GUIStyle style = new GUIStyle(GUI.skin.box) {
                padding = new RectOffset(6, 6, 2, 2)
            };
            GUILayout.BeginVertical(style);
        } else {
            GUILayout.BeginVertical();
        }
        action();
        GUILayout.EndVertical();
    }

    private static void LayoutH(System.Action action, bool box = false) {
        if (box) {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            GUILayout.BeginHorizontal(style);
        } else {
            GUILayout.BeginHorizontal();
        }
        action();
        GUILayout.EndHorizontal();
    }

    private static void ResetInCanvasFor(RectTransform root) {

        root.SetParent(Selection.activeTransform);
        if (!InCanvas(root)) {
            Transform canvasTR = GetCreateCanvas();
            root.SetParent(canvasTR);
        }
        if (!Transform.FindObjectOfType<EventSystem>()) {
            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }
        root.localScale = Vector3.one;
        root.localPosition = new Vector3(root.localPosition.x, root.localPosition.y, 0);
        Selection.activeGameObject = root.gameObject;
    }

    private static bool InCanvas(Transform tf) {
        while (tf.parent) {
            tf = tf.parent;
            if (tf.GetComponent<Canvas>()) {
                return true;
            }
        }
        return false;
    }

    private static Transform GetCreateCanvas() {
        Canvas c = Object.FindObjectOfType<Canvas>();
        if (c) {
            return c.transform;
        } else {
            GameObject canvas = new GameObject("Canvas");
            c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            return canvas.transform;
        }
    }

}
