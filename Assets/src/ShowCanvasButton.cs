using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShowCanvasButton: IAddCallback
{
    public override FinishCallback AddCallback(object[] args) {
        base.AddCallback(args);
        GameObject gameObject = args[0] as GameObject;
        if (args.Length != 1 || gameObject.name != "Button") {
            throw new Exception ("Invalid list of objects given!");
        }
        
        return delegate {
                Canvas canvas = gameObject.transform.parent.parent.GetComponent<Canvas>();
                canvas.worldCamera = Camera.current;
                gameObject.SetActive(true);
        };
    }
}
