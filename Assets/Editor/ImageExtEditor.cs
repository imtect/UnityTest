using UnityEngine;
using UnityEditor.UI;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(ImageExt), true)]
[CanEditMultipleObjects]
public class ImageExtEditor : ImageEditor {
    SerializedProperty m_Sprite;
    SerializedProperty m_ImageShape;

    //Circle
    SerializedProperty m_SegmentCount;
    SerializedProperty m_FillPercent;
    SerializedProperty m_Full;

    //Ring
    SerializedProperty m_Thickness;

    //TilletRect
    SerializedProperty m_FilletRadius;
    SerializedProperty m_FilletSegments;

    AnimBool m_ShowShape;
    AnimBool m_ShowNormal;
    AnimBool m_ShowCircle;
    AnimBool m_ShowTilletRect;
    AnimBool m_ShowRing;


    protected override void OnEnable() {
        base.OnEnable();

        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_ImageShape = serializedObject.FindProperty("m_ImageShape");

        m_SegmentCount = serializedObject.FindProperty("m_SegmentCount");
        m_FillPercent = serializedObject.FindProperty("m_FillPercent");
        m_Full = serializedObject.FindProperty("m_Full");

        m_Thickness = serializedObject.FindProperty("m_Thickness");

        m_FilletRadius = serializedObject.FindProperty("m_FilletRadius");
        m_FilletSegments = serializedObject.FindProperty("m_FilletSegments");


        var shapeEnum = (ImageExt.ImageShape)m_ImageShape.enumValueIndex;

        m_ShowShape = new AnimBool(m_Sprite.objectReferenceValue != null);
        m_ShowNormal = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.Normal);
        m_ShowCircle = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.Circle);
        m_ShowTilletRect = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.FilletRect);
        m_ShowRing = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.Ring);
        m_ShowShape.valueChanged.AddListener(Repaint);
        m_ShowNormal.valueChanged.AddListener(Repaint);
        m_ShowCircle.valueChanged.AddListener(Repaint);
        m_ShowTilletRect.valueChanged.AddListener(Repaint);
        m_ShowRing.valueChanged.AddListener(Repaint);

    }

    protected override void OnDisable() {
        base.OnDisable();
        m_ShowShape.valueChanged.RemoveListener(Repaint);
        m_ShowNormal.valueChanged.RemoveListener(Repaint);
        m_ShowCircle.valueChanged.RemoveListener(Repaint);
        m_ShowTilletRect.valueChanged.RemoveListener(Repaint);
        m_ShowRing.valueChanged.RemoveListener(Repaint);
    }


    public override void OnInspectorGUI() {

        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Space();

        m_ShowShape.target = m_Sprite.objectReferenceValue != null;
        EditorUtilExt.LayoutGroup(m_ShowShape, () => { ShapeGUI(); });

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Sprite's custom properties based on the selected image shape
    /// </summary>
    protected void ShapeGUI() {
        EditorGUILayout.PropertyField(m_ImageShape);
        ++EditorGUI.indentLevel;
        {
            ImageExt.ImageShape shapeEnum = (ImageExt.ImageShape)m_ImageShape.enumValueIndex;
            m_ShowNormal = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.Normal);
            m_ShowCircle = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.Circle);
            m_ShowTilletRect = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.FilletRect);
            m_ShowRing = new AnimBool(!m_ImageShape.hasMultipleDifferentValues && shapeEnum == ImageExt.ImageShape.Ring);
            EditorUtilExt.LayoutGroup(m_ShowCircle, () => {
                EditorGUILayout.PropertyField(m_SegmentCount);
                EditorGUILayout.PropertyField(m_FillPercent);
                EditorGUILayout.PropertyField(m_Full);
            });
            EditorUtilExt.LayoutGroup(m_ShowTilletRect, () => {
                EditorGUILayout.PropertyField(m_SegmentCount);
                EditorGUILayout.PropertyField(m_Thickness);
            });
            EditorUtilExt.LayoutGroup(m_ShowRing, () => {
                EditorGUILayout.PropertyField(m_FilletSegments);
                EditorGUILayout.PropertyField(m_FilletRadius);
            });
        }
        --EditorGUI.indentLevel;
    }
}
