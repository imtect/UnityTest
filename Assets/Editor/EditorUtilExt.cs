using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.AnimatedValues;

public class EditorUtilExt : Editor {

    [MenuItem("GameObject/UI/UIExt/TextExt")]
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

    [MenuItem("GameObject/UI/UIExt/ImageExt")]
    public static void CreateImageExt() {
        GameObject imageRoot = new GameObject("Image", typeof(RectTransform), typeof(ImageExt));
        ResetInCanvasFor((RectTransform)imageRoot.transform);
        var imageExt = imageRoot.GetComponent<ImageExt>();
        imageExt.overrideSprite = null;
        imageExt.color = Color.white;
        imageExt.transform.localPosition = Vector3.zero;
        imageExt.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
    }

    [MenuItem("GameObject/UI/UIExt/ButtonExt")]
    public static void CreateButtonExt() {
        GameObject imageRoot = new GameObject("Button", typeof(RectTransform), typeof(ButtonExt));
        ResetInCanvasFor((RectTransform)imageRoot.transform);
        var btnExt = imageRoot.GetComponent<ButtonExt>();
        btnExt.transform.localPosition = Vector3.zero;
        btnExt.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
    }

    [MenuItem("GameObject/UI/UIExt/ScrollRectExt")]
    public static void CreateScrollRectExt() {
        GameObject imageRoot = new GameObject("Button", typeof(RectTransform), typeof(ButtonExt));
        ResetInCanvasFor((RectTransform)imageRoot.transform);
        var btnExt = imageRoot.GetComponent<ButtonExt>();
        btnExt.transform.localPosition = Vector3.zero;
        btnExt.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
    }

    public static void LayoutGroup(AnimBool animBool, SerializedProperty property) {
        if (EditorGUILayout.BeginFadeGroup(animBool.faded)) {
            EditorGUILayout.PropertyField(property);
        }
        EditorGUILayout.EndFadeGroup();
    }

    public static void LayoutGroup(AnimBool animBool, SerializedProperty[] propertys) {
        if (EditorGUILayout.BeginFadeGroup(animBool.faded)) {
            foreach (var item in propertys) {
                EditorGUILayout.PropertyField(item);
            }
        }
        EditorGUILayout.EndFadeGroup();
    }

    public static void LayoutGroup(AnimBool animBool, System.Action action) {
        if (EditorGUILayout.BeginFadeGroup(animBool.faded)) {
            action();
        }
        EditorGUILayout.EndFadeGroup();
    }


    public static void LayoutF(System.Action action,string label, ref bool open,bool box = false) {
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

    public static void Space(float space = 4f) {
        GUILayout.Space(space);
    }

    public static Rect GUIRect(float width,float height) {
        return GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(width <= 0), GUILayout.ExpandHeight(height <= 0));
    }

    public static void LayoutV(System.Action action,bool box = false) {
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

    public static void LayoutH(System.Action action, bool box = false) {
        if (box) {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            GUILayout.BeginHorizontal(style);
        } else {
            GUILayout.BeginHorizontal();
        }
        action();
        GUILayout.EndHorizontal();
    }

    public static void ResetInCanvasFor(RectTransform root) {

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

    public static bool InCanvas(Transform tf) {
        while (tf.parent) {
            tf = tf.parent;
            if (tf.GetComponent<Canvas>()) {
                return true;
            }
        }
        return false;
    }

    public static Transform GetCreateCanvas() {
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
