using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class TestMask : MonoBehaviour
{

    Button btn;

    public Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        btn = GameObject.Find("Button").GetComponent<Button>();
        btn.onClick.AddListener(() => {
            Debug.Log("btn is clicked!!!");
        });



      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector4 outerUV = DataUtility.GetOuterUV(sprite);
            Vector4 innerUV = DataUtility.GetInnerUV(sprite);
            Vector4 padding = DataUtility.GetPadding(sprite);
            Vector2 minSize = DataUtility.GetMinSize(sprite);
            print(outerUV);
            print(innerUV);
            print(padding);
            print(minSize);
            print("-------------------------------------------");
        }
    }
}
