using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

[AddComponentMenu("ButtonExt")]
public class ButtonExt : Button
{

    [SerializeField]
    [Range(1,100)]
    public float Test = 1;
}
