using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[AddComponentMenu("TextExt")]
public class TextExt : Text {

    [SerializeField]
    TextSpaceHandler m_textSpaceHandler = new TextSpaceHandler();

    //UI调用
    protected override void OnPopulateMesh(VertexHelper toFill) {
        base.OnPopulateMesh(toFill);

        m_textSpaceHandler.PopulateMesh(toFill);
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void OnDisable() {
        base.OnDisable();
    }
}
