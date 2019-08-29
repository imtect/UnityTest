using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Sprites;

[AddComponentMenu("ImageExt")]
public class ImageExt : Image {

    /// <summary>
    /// Image Shape Can be Normal,Circle,FilletRect
    /// </summary>
    public enum ImageShape {
        Normal,
        Circle,
        FilletRect,
        Ring,
    }


    [SerializeField]
    private ImageShape m_ImageShape = ImageShape.Normal;
    public ImageShape ImageType {
        get { return m_ImageShape; }
        set { m_ImageShape = value; }
    }


    [SerializeField]
    [Range(0, 100)]
    private int m_SegmentCount = 48;
    public int SegmentCount {
        get { return m_SegmentCount; }
        set { m_SegmentCount = value; }
    }

    [SerializeField]
    private float m_Radius;
    public float Radius {
        get { return m_Radius; }
        set { m_Radius = value; }
    }

    [SerializeField]
    private bool m_Full = true;
    public bool Full {
        get { return m_Full; }
        set { m_Full = value; }
    }

    [SerializeField]
    [Range(0, 1)]
    private float m_FillPercent = 1;
    public float FillPercent {
        get { return m_FillPercent; }
        set { m_FillPercent = value; }
    }


    [SerializeField]
    private float m_Thickness;
    public float Thickness {
        get { return m_Thickness; }
        set { m_Thickness = value; }
    }


    [SerializeField]
    private float m_FilletRadius;
    public float FilletRadius {
        get { return m_FilletRadius; }
        set { m_FilletRadius = value; }
    }

    [SerializeField]
    private float m_FilletSegments;
    public float FilletSegments {
        get { return m_FilletSegments; }
        set { m_FilletSegments = value; }
    }


    protected override void OnPopulateMesh(VertexHelper toFill) {
        base.OnPopulateMesh(toFill);

        switch (m_ImageShape) {
            case ImageShape.Normal:
                break;
            case ImageShape.Circle:
                PopulateCircleMesh(toFill);
                break;
            case ImageShape.FilletRect:
                break;
            case ImageShape.Ring:
                break;
            default:
                break;
        }
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {


        return true;
    }

    protected void PopulateCircleMesh(VertexHelper vh) {

        vh.Clear();

        //宽高
        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;

        //uv
        Vector4 uv = GetUV();
        float uvCenterX = (uv.x + uv.z) * 0.5f;
        float uvCenterY = (uv.y + uv.w) * 0.5f;
        float uvScaleX = (uv.z - uv.x) / tw;
        float uvScaleY = (uv.w - uv.y) / th;

        //radius
        m_Radius = tw < th ? rectTransform.pivot.x * tw : rectTransform.pivot.x * th;

        //segments
        float degreeDelta = (float)(2 * Mathf.PI / m_SegmentCount);
        int curSegments = (int)(m_SegmentCount * m_FillPercent);

        //加入中心点
        Vector2 curVertice = Vector2.zero;
        CreateUIVertex(curVertice, new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY), vh);

        float curDegree = 0;
        int veticeCount = curSegments + 1;
        //加入圆周点
        for (int i = 0; i < veticeCount; i++) {
            float cosA = Mathf.Cos(curDegree);
            float sinA = Mathf.Sin(curDegree);
            curVertice = new Vector2(cosA * m_Radius, sinA * m_Radius);
            CreateUIVertex(curVertice, new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY), vh);

            curDegree += degreeDelta;
        }

        //创建三角索引
        int triangleCount = curSegments * 3;
        for (int i = 0, vIdx = 1; i < triangleCount; i += 3, vIdx++) {
            if (vIdx == veticeCount - 1 && m_FillPercent == 1) {
                vh.AddTriangle(veticeCount - 1, 0, 1);
            } else {
                vh.AddTriangle(vIdx, 0, vIdx + 1);
            }
        }
    }

    protected void PopulateRingMesh(VertexHelper vh) { 

    }

    protected void PopulateTilletRectMesh() {

    }

    //其实就是判断点是否在多边形内的算法，Ray-Crossing算法
    protected void RayCrossing() {

    }

    protected void CreateUIVertex(Vector2 pos, Vector2 uv, VertexHelper vh) {
        var uIVertex = new UIVertex();
        uIVertex.color = color;
        uIVertex.position = pos;
        uIVertex.uv0 = uv;
        vh.AddVert(uIVertex);
    }

    protected Vector4 GetUV() {
        return overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
    }



}