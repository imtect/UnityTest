using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    ImageExt Img;

    void Start() {

        Img = GameObject.Find("Image").GetComponent<ImageExt>();
        Img.PointerClick = () => {
            Debug.Log("Image is PointerClick!");
        };
        Img.PointerDown = () => {
            Debug.Log("Image is PointerDown!");
        };
        Img.PointerEner = () => {
            Debug.Log("Image is PointerEner!");
        };
        Img.PointerExit = () => {
            Debug.Log("Image is PointerExit!");
        };
        Img.PointerUp = () => {
            Debug.Log("Image is PointerUp!");
        };
    }
}
