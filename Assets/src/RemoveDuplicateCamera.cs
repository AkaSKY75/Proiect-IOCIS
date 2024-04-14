using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RemoveDuplicateCamera: IAddCallback
{
    public override FinishCallback AddCallback(object[] args) {
        base.AddCallback(args);
        GameObject gameObject = args[0] as GameObject;
        if (args.Length != 1 || gameObject.name != "Player") {
            throw new Exception ("Invalid list of objects given!");
        }
        return delegate {
            Transform duplicateCamera = gameObject.transform.Find("Camera(Clone)");
            Transform originalCamera = gameObject.transform.Find("Camera");
            if (duplicateCamera == null || originalCamera == null) {
                throw new Exception ("Couldn't find camera or duplicate camera of player!");
            }
            Destroy(duplicateCamera.gameObject);
            originalCamera.gameObject.SetActive(true);
        };
    }
}
