using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Sprites;

public class DrawCircle : Image
{

    float tw;
    float th;


    protected override void OnPopulateMesh(VertexHelper toFill) {
        toFill.Clear();

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

        float uvCenterX = (uv.x + uv.z) * 0.5f;
        float uvCenterY = (uv.x + uv.w) * 0.5f;
        float uvScaleX = (uv.z - uv.x) / tw;
        float uvScaleY = (uv.w - uv.y) / th;


    }

    protected void CalculateCircle() {
        UIVertex uIVertex = new UIVertex();
       // uIVertex.
    }

}
