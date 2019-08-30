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
    [Range(3, 100)]
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
    private float m_Thickness = 1;
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
    [Range(3, 100)]
    private int m_FilletSegments;
    public int FilletSegments {
        get { return m_FilletSegments; }
        set { m_FilletSegments = value; }
    }

    private Vector2 m_UVCenter;
    private Vector2 m_UVScale;



    protected override void OnEnable() {
        base.OnEnable();
        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        m_UVCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
        m_UVScale = new Vector2((uv.z - uv.x) / tw, (uv.w - uv.y) / th);


        float outterRadius = tw < th ? rectTransform.pivot.x * tw : rectTransform.pivot.x * th;
        m_Thickness = outterRadius / 2;

        m_FilletRadius = tw < th ? tw * 0.2f : th * 0.2f;
    }

    protected override void OnDisable() {
        base.OnDisable();
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
                PopulateTilletRectMesh(toFill);
                break;
            case ImageShape.Ring:
                PopulateRingMesh(toFill);
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

        //radius
        m_Radius = tw < th ? rectTransform.pivot.x * tw : rectTransform.pivot.x * th;

        //segments
        float degreeDelta = (float)(2 * Mathf.PI / m_SegmentCount);
        int curSegments = (int)(m_SegmentCount * m_FillPercent);

        //加入中心点
        CreateUIVertex(Vector2.zero, vh);

        float curDegree = 0;
        int veticeCount = curSegments + 1;
        //加入圆周点
        for (int i = 0; i < veticeCount; i++) {
            float cosA = Mathf.Cos(curDegree);
            float sinA = Mathf.Sin(curDegree);
            CreateUIVertex(new Vector2(cosA * m_Radius, sinA * m_Radius), vh);
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
        vh.Clear();

        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;

        float outterRadius = tw < th ? rectTransform.pivot.x * tw : rectTransform.pivot.x * th;
        float intterRadius = outterRadius - m_Thickness;

        if (m_Thickness > outterRadius) m_Thickness = outterRadius;


        float curDegree = 0;
        float degreeDelta = 2 * Mathf.PI / m_SegmentCount;
        int curSegements = m_SegmentCount;
        int verticeCount = curSegements * 2;
        Vector2 _uv = Vector2.zero;
        //顶点
        for (int i = 0; i < verticeCount; i += 2) {
            float cosA = Mathf.Cos(curDegree);
            float sinA = Mathf.Sin(curDegree);
            curDegree += degreeDelta;

            //内环顶点
            CreateUIVertex(new Vector2(cosA * intterRadius, sinA * intterRadius), vh);
            //外环顶点
            CreateUIVertex(new Vector2(cosA * outterRadius, sinA * outterRadius), vh);
        }

        //索引
        int triangleCount = curSegements * 2 * 3;
        for (int i = 0, vIdx = 0; i < triangleCount; i += 6, vIdx += 2) {
            if (vIdx == verticeCount - 2) {
                vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                vh.AddTriangle(verticeCount - 2, 0, 1);
            } else {
                vh.AddTriangle(vIdx + 1, vIdx, vIdx + 3);
                vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
            }
        }
    }

    protected void PopulateTilletRectMesh(VertexHelper vh) {
        vh.Clear();

        //rect
        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;
        float halttw = rectTransform.pivot.x * tw;
        float haltth = rectTransform.pivot.y * th;

        Vector2 v1 = new Vector2(halttw - m_FilletRadius, haltth - m_FilletRadius);//右上角圆点
        Vector2 v2 = new Vector2(m_FilletRadius - halttw, haltth - m_FilletRadius);//左上角圆点
        Vector2 v3 = new Vector2(m_FilletRadius - halttw, m_FilletRadius - haltth);//左下角圆点
        Vector2 v4 = new Vector2(halttw - m_FilletRadius, m_FilletRadius - haltth);//右下角圆点

        int m_curSegment = m_FilletSegments * 4;
        float deltaDegree = Mathf.PI / 2 / m_curSegment;
        float curDegree = 0;
        List<UIVertex> tt = new List<UIVertex>();
        for (int i = 0; i < m_curSegment; i += 4) {
            tt.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree) * m_FilletRadius + v1.x, Mathf.Sin(curDegree) * m_FilletRadius + v1.y), vh));//右上角
            tt.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree + Mathf.PI / 2) * m_FilletRadius + v2.x, Mathf.Sin(curDegree + Mathf.PI / 2) * m_FilletRadius + v2.y), vh));//左上角
            tt.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree + Mathf.PI) * m_FilletRadius + v3.x, Mathf.Sin(curDegree + Mathf.PI) * m_FilletRadius + v3.y), vh));//左下角
            tt.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree + Mathf.PI * 3 / 2) * m_FilletRadius + v4.x, Mathf.Sin(curDegree + Mathf.PI * 3 / 2) * m_FilletRadius + v4.y), vh));//右下角
            curDegree += deltaDegree;
            index++;
        }

        int triangleCount = m_curSegment * 4 * 3;
        vh.AddTriangle(0, 2, 1);
        vh.AddTriangle(0, 3, 2);
        for (int i = 0, vIdx = 0; vIdx < m_curSegment; i += 12, vIdx += 4) {
            vh.AddTriangle(vIdx, 1, vIdx + 4);
            vh.AddTriangle(vIdx + 1, 2, vIdx + 5);
            vh.AddTriangle(vIdx + 2, 3, vIdx + 6);
            vh.AddTriangle(vIdx + 3, 0, vIdx + 7);
        }
    }

    //其实就是判断点是否在多边形内的算法，Ray-Crossing算法
    protected void RayCrossing() {

    }

    float index = 1;

    protected UIVertex CreateUIVertex(Vector2 pos, VertexHelper vh) {
        var uIVertex = new UIVertex();
        uIVertex.color = color;
        uIVertex.position = pos;
        uIVertex.uv0 = new Vector2(pos.x * m_UVScale.x + m_UVCenter.x, pos.y * m_UVScale.y + m_UVCenter.y);
        vh.AddVert(uIVertex);
        return uIVertex;
    }
}