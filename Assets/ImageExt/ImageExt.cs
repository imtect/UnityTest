using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;


[AddComponentMenu("ImageExt")]
public class ImageExt : Image {

    [SerializeField]
    private bool m_IsFull = true;
    public bool Full {
        get { return m_IsFull; }
        set { m_IsFull = value; }
    }

    [SerializeField]
    [Range(0,100)]
    private int m_SegmentCount = 24;
    public int SegmentCount {
        get { return m_SegmentCount; }
        set { m_SegmentCount = value; }
    }


    protected override void OnPopulateMesh(VertexHelper toFill) {
        base.OnPopulateMesh(toFill);


    }

}
