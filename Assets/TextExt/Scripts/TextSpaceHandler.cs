using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class TextSpaceHandler {

    [SerializeField]
    private bool m_UseTextSpace;
    public bool UseTextSpace {
        get { return m_UseTextSpace; }
        set { m_UseTextSpace = value; }
    }

    [SerializeField]
    [Range(-10,10)]
    private float m_TextSpace;
    public float TextSpace {
        get { return m_TextSpace; }
        set { m_TextSpace = value; }
    }


    public void PopulateMesh(VertexHelper toFill) {
        if (m_UseTextSpace) {
            if (toFill.currentVertCount == 0) return; //顶点数量

            List<UIVertex> vertexs = new List<UIVertex>();
            toFill.GetUIVertexStream(vertexs);

            int indexCount = toFill.currentIndexCount; //顶点索引
            UIVertex vt;
            for (int i = 6; i < indexCount; i++) {
                vt = vertexs[i];
                vt.position += new Vector3(m_TextSpace * (i / 6), 0, 0);
                vertexs[i] = vt;
                if (i % 6 <= 2) {
                    toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6);
                }
                if (i % 6 == 4) {
                    toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6 - 1);
                }
            }
        }
    }
}
