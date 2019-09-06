using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Sprites;
using UnityEngine.EventSystems;
using System;


[AddComponentMenu("ImageExt")]
public class ImageExt : Image, 
    IPointerClickHandler, 
    IPointerDownHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerUpHandler,
    IDragHandler{

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
    private bool m_isDragValid = false;
    public bool CanDrag {
        get { return m_isDragValid; }
        set { m_isDragValid = value; }
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
    private int m_FilletSegments = 31;
    public int FilletSegments {
        get { return m_FilletSegments; }
        set { m_FilletSegments = value; }
    }

    private float m_width;
    private float m_heigh;
    private Vector2 m_UVCenter;
    private Vector2 m_UVScale;
    private List<Vector2> m_outterVertices;
    private List<Vector2> m_innerVertices;

    public Action PointerEner;
    public Action PointerExit;
    public Action PointerDown;
    public Action PointerUp;
    public Action PointerClick;


    protected override void OnEnable() {
        base.OnEnable();

        m_outterVertices = new List<Vector2>();
        m_innerVertices = new List<Vector2>();

        m_width = rectTransform.rect.width;
        m_heigh = rectTransform.rect.height;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        m_UVCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
        m_UVScale = new Vector2((uv.z - uv.x) / m_width, (uv.w - uv.y) / m_heigh);


        float outterRadius = m_width < m_heigh ? rectTransform.pivot.x * m_width : rectTransform.pivot.x * m_heigh;
        m_Thickness = outterRadius / 2;

        m_FilletRadius = m_width < m_heigh ? m_width * 0.2f : m_heigh * 0.2f;
    }

    protected override void OnDisable() {
        base.OnDisable();

        PointerEner     = null;
        PointerExit     = null;
        PointerDown     = null;
        PointerUp       = null;
        PointerClick    = null;
    }


    protected override void OnPopulateMesh(VertexHelper toFill) {
        base.OnPopulateMesh(toFill);

        switch (m_ImageShape) {
            case ImageShape.Normal:
                m_outterVertices.Add(new Vector2(m_width / 2, m_heigh / 2));
                m_outterVertices.Add(new Vector2(m_width / 2, -m_heigh / 2));
                m_outterVertices.Add(new Vector2(-m_width / 2, -m_heigh / 2));
                m_outterVertices.Add(new Vector2(-m_width / 2, m_heigh / 2));
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
        Sprite spirte = overrideSprite;
        if (spirte == null) return true;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
        return Contains(local, m_outterVertices, m_innerVertices);
    }

    protected void PopulateCircleMesh(VertexHelper vh) {

        vh.Clear();

        m_outterVertices.Clear();

        //宽高
        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        m_UVCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
        m_UVScale = new Vector2((uv.z - uv.x) / tw, (uv.w - uv.y) / th);

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
            m_outterVertices.Add(CreateUIVertex(new Vector2(cosA * m_Radius, sinA * m_Radius), vh).position);
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

        m_innerVertices.Clear();
        m_outterVertices.Clear();

        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        m_UVCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
        m_UVScale = new Vector2((uv.z - uv.x) / tw, (uv.w - uv.y) / th);

        float outterRadius = tw < th ? rectTransform.pivot.x * tw : rectTransform.pivot.x * th;
        float intterRadius = outterRadius - m_Thickness;

        if (m_Thickness > outterRadius) m_Thickness = outterRadius;
        if (m_Thickness <= 0) m_Thickness = 0;

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
            m_innerVertices.Add(CreateUIVertex(new Vector2(cosA * intterRadius, sinA * intterRadius), vh).position);
            //外环顶点
            m_outterVertices.Add(CreateUIVertex(new Vector2(cosA * outterRadius, sinA * outterRadius), vh).position);
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

        m_outterVertices.Clear();

        //rect
        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;
        float halttw = rectTransform.pivot.x * tw;
        float haltth = rectTransform.pivot.y * th;

        //半径不能超过宽度的一般
        float half = tw > th ? th * 0.5f : tw * 0.5f;
        FilletRadius = FilletRadius > half ? half : FilletRadius;

        if (FilletRadius <= 0) FilletRadius = 0;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        m_UVCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
        m_UVScale = new Vector2((uv.z - uv.x) / tw, (uv.w - uv.y) / th);

        Vector2 v1 = new Vector2(halttw - m_FilletRadius, haltth - m_FilletRadius);//右上角圆点
        Vector2 v2 = new Vector2(m_FilletRadius - halttw, haltth - m_FilletRadius);//左上角圆点
        Vector2 v3 = new Vector2(m_FilletRadius - halttw, m_FilletRadius - haltth);//左下角圆点
        Vector2 v4 = new Vector2(halttw - m_FilletRadius, m_FilletRadius - haltth);//右下角圆点

        int m_curSegment = m_FilletSegments * 4;
        float deltaDegree = Mathf.PI / 2 / (m_FilletSegments - 1);
        float curDegree = 0;
        for (int i = 0; i < m_curSegment; i += 4) {
            m_outterVertices.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree) * m_FilletRadius + v1.x, Mathf.Sin(curDegree) * m_FilletRadius + v1.y), vh).position);//右上角
            m_outterVertices.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree + Mathf.PI / 2) * m_FilletRadius + v2.x, Mathf.Sin(curDegree + Mathf.PI / 2) * m_FilletRadius + v2.y), vh).position);//左上角
            m_outterVertices.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree + Mathf.PI) * m_FilletRadius + v3.x, Mathf.Sin(curDegree + Mathf.PI) * m_FilletRadius + v3.y), vh).position);//左下角
            m_outterVertices.Add(CreateUIVertex(new Vector2(Mathf.Cos(curDegree + Mathf.PI * 3 / 2) * m_FilletRadius + v4.x, Mathf.Sin(curDegree + Mathf.PI * 3 / 2) * m_FilletRadius + v4.y), vh).position);//右下角
            curDegree += deltaDegree;
        }

        vh.AddTriangle(0, 2, 1);
        vh.AddTriangle(0, 3, 2);

        for (int i = 0; i < 4; i++) {

            int maxIndex = (m_FilletSegments - 1) * 4 + i;

            //偶数个顶点
            if (m_FilletSegments % 2 == 0) {
                //TODO:........

            } else {//奇数个顶点 
                int bigTriangleCount = (int)((m_FilletSegments + 1) * 0.5);
                for (int vIdx = 0; vIdx < bigTriangleCount; vIdx++) {
                    if (8 * vIdx + i == maxIndex) {
                        int angle = i + 1 == 4 ? 0 : i + 1;
                        vh.AddTriangle(i, angle, maxIndex);
                    } else {
                        vh.AddTriangle(i, 8 * (vIdx + 1) + i, vIdx == 0 ? (4 + i) : (vIdx * 8 + i));
                    }
                }
                for (int vIdx2 = 1; vIdx2 < m_FilletSegments - 1 - bigTriangleCount; vIdx2++) {
                    vh.AddTriangle(8 * vIdx2 + i, 8 * (vIdx2 + 1) + i, 8 * (vIdx2 + 1) + i - 4);
                }
            }
        }
    }

    void TravelAllTriangle(int startIndex, int endIndex, VertexHelper vt, int travelCount = 1) {

        int curLeft = startIndex;
        int curRight = endIndex;
        int curCenter = (int)(endIndex) / 2;

        for (int i = 0; i < travelCount; i++) {
            if (curCenter % 2 == 0) {


                vt.AddTriangle(curLeft, curRight, curCenter);

                if (true) {
                    travelCount = (int)Mathf.Pow(2, travelCount);
                    TravelAllTriangle(curLeft, curRight, vt, travelCount);
                }
            } else {

            }
        }
    }




    //其实就是判断点是否在多边形内的算法，Ray-Crossing算法
    private bool Contains(Vector2 p, List<Vector2> outterVertices, List<Vector2> innerVertices) {
        var crossNumber = 0;
        RayCrossing(p, innerVertices, ref crossNumber);//检测内环
        RayCrossing(p, outterVertices, ref crossNumber);//检测外环
        return (crossNumber & 1) == 1;
    }

    /// <summary>
    /// 使用RayCrossing算法判断点击点是否落在多边形里
    /// </summary>
    /// <param name="p"></param>
    /// <param name="vertices"></param>
    /// <param name="crossNumber"></param>
    private void RayCrossing(Vector2 p, List<Vector2> vertices, ref int crossNumber) {
        for (int i = 0, count = vertices.Count; i < count; i++) {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % count];

            //点击点水平线必须与两顶点线段相交
            if (((v1.y <= p.y) && (v2.y > p.y))
                || ((v1.y > p.y) && (v2.y <= p.y))) {
                //只考虑点击点右侧方向，点击点水平线与线段相交，且交点x > 点击点x，则crossNumber+1
                if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x)) {
                    crossNumber += 1;
                }
            }
        }
    }

    protected UIVertex CreateUIVertex(Vector2 pos, VertexHelper vh) {
        var uIVertex = new UIVertex();
        uIVertex.color = color;
        uIVertex.position = pos;
        uIVertex.uv0 = new Vector2(pos.x * m_UVScale.x + m_UVCenter.x, pos.y * m_UVScale.y + m_UVCenter.y);
        vh.AddVert(uIVertex);
        return uIVertex;
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        PointerEner?.Invoke();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        PointerExit?.Invoke();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        PointerDown?.Invoke();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        PointerUp?.Invoke();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        PointerClick?.Invoke();
    }
        
    void IDragHandler.OnDrag(PointerEventData eventData) {
        if (m_isDragValid) {
            var rect = GetComponent<RectTransform>();
            Vector3 pos = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.enterEventCamera, out pos);
            rect.position = pos;
        }
    }
}