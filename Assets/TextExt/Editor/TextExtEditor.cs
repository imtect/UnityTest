using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TextExt),true)]
[CanEditMultipleObjects]
public class TextExtEditor : GraphicEditor {

    private static bool m_TextSpacingPanelOpen = false;

    SerializedProperty m_text;
    SerializedProperty m_fontdata;

    //序列化属性
    SerializedProperty m_UseTextSpacing;
    SerializedProperty m_TextSpacing;

    protected override void OnEnable() {
        base.OnEnable();

        m_text = serializedObject.FindProperty("m_Text");
        m_fontdata = serializedObject.FindProperty("m_FontData");

        m_UseTextSpacing = serializedObject.FindProperty("m_textSpaceHandler.m_UseTextSpace");
        m_TextSpacing = serializedObject.FindProperty("m_textSpaceHandler.m_TextSpace");



        m_TextSpacingPanelOpen = EditorPrefs.GetBool("UGUIPlus.m_TextSpacingPanelOpen", m_TextSpacingPanelOpen);
    }

    public override void OnInspectorGUI() {   
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_text);
        EditorGUILayout.PropertyField(m_fontdata);
        ExtGUI();
        serializedObject.ApplyModifiedProperties();
    }

    private void ExtGUI() {
        TextExtUtil.TextSpacingGUI(m_UseTextSpacing, m_TextSpacing, ref m_TextSpacingPanelOpen);
    }
}
