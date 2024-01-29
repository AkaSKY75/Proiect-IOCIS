using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleCamera : MonoBehaviour
{
    public Camera cam;
    private readonly Vector3[] cameraPos = new Vector3[] {
        new Vector3(-0.079999648f, 11.5f, -3.82999992f),        // 3PS
        new Vector3(-0.0599999987f, 8.31999969f, 0.319999993f)  // FPS
    };
    private enum CurrentCamera:byte {
        _3PS,
        FPS,
        LAST
    }
    CurrentCamera currentCamera;
    private void Start() {
        currentCamera = CurrentCamera._3PS;
    }

    public void toggleCamera() {
        currentCamera = (CurrentCamera)((byte)((byte)((byte)currentCamera + 1) % (byte) CurrentCamera.LAST));
        cam.transform.localPosition = cameraPos[(byte)currentCamera];
    }

    public void rotateCamera(float mouseDifY) {
        cam.transform.Rotate(-mouseDifY, 0f, 0f);
    }
}
