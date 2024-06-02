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
        return delegate {
            Transform duplicateCamera = transform.Find("Camera(Clone)");
            Transform originalCamera = transform.Find("Camera");
            if (duplicateCamera == null || originalCamera == null) {
                throw new Exception ("Couldn't find camera or duplicate camera of player!");
            }
            Destroy(duplicateCamera.gameObject);
            originalCamera.gameObject.SetActive(true);
            transform.GetComponent<Movements>().EnableMovements();
        };
    }
}
