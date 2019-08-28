using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(ImageExt),true)]
[CanEditMultipleObjects]
public class ImageEditor : GraphicEditor
{

    SerializedProperty m_SourceImage;
    SerializedProperty m_Full;
    SerializedProperty m_SegmentCount;

    protected override void OnEnable() {
        base.OnEnable();

        m_SourceImage = serializedObject.FindProperty("m_SourceImage");
        m_Full = serializedObject.FindProperty("m_IsFull");
        m_SegmentCount = serializedObject.FindProperty("m_SegmentCount");
    }


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_SourceImage);

        EditorGUI.PropertyField(GUIRect(0, 18), m_SegmentCount, new GUIContent());

        serializedObject.ApplyModifiedProperties();
    }

    private Rect GUIRect(float width, float height) {
        return GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(width <= 0), GUILayout.ExpandHeight(height <= 0));
    }
}
