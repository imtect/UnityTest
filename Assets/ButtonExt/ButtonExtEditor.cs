using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;


[CustomEditor(typeof(ButtonExt),true)]
[CanEditMultipleObjects]
public class ButtonExtEditor : ButtonEditor
{

    SerializedProperty m_Test;

    protected override void OnEnable() {
        base.OnEnable();

        m_Test = serializedObject.FindProperty("Test");
    }

    protected override void OnDisable() {
        base.OnDisable();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Test);
        serializedObject.ApplyModifiedProperties();
    }
}
