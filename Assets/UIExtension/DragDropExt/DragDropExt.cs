using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropExt : MonoSingleton<DragDropExt>
{

    private void Awake() {
        Debug.Log("DragDropExt Singleton excute!");
    }


    public void  Show() {
        Debug.Log("DragDropExt Singleton excute!");
    }
}
