using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWebCamera: IAddCallback
{
    public override FinishCallback AddCallback(object[] args) {
        Transform webCamPanel = transform.Find("Camera(Clone)").Find("Canvas").Find("WebCamPanel");
        if (webCamPanel == null) {
            throw new Exception("Game object `WebCamPanel` was not found within " +
                transform.name + "->Camera(Clone)->Canvas->WebCamPanel");
        }
        webCamPanel.gameObject.SetActive(true);
        WebCamera webCameraComponent = transform.GetComponent<WebCamera>();
        if (webCameraComponent == null) {
            throw new Exception(transform.name + " game object doesn't have `WebCamera` component!");
        }
        return delegate {
            // should activate final callback here
            transform.GetComponent<RemoveDuplicateCamera>().enabled = true;
            transform.GetComponent<AnimationState>().SetArmsUpAnimationState();
            webCameraComponent.OnStartWebCamera(webCamPanel.Find("RawImage"));
            transform.GetComponent<OnWebCamera>().enabled = false;
        };
    }
}
