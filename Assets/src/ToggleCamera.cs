using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleCamera : MonoBehaviour
{
    public Camera cam;
 
    private float yRotation = 0f; // Variabilă pentru a stoca rotația pe axa Y

    private readonly Vector3[] cameraPos = new Vector3[] {
        new Vector3(-0.079999648f, 11.5f, -3.82999992f),        // 3PS
        new Vector3(-0.0599999987f, 8.31999969f, 0.319999993f)  // FPS
    };
    private enum CurrentCamera:byte {
        _3PS,
        FPS,
        LAST
    }
    private CurrentCamera currentCamera;
    private void Start() {
        currentCamera = CurrentCamera._3PS;
        Cursor.visible = false;
    }

    public void toggleCamera() {
        currentCamera = (CurrentCamera)((byte)((byte)((byte)currentCamera + 1) % (byte) CurrentCamera.LAST));
        cam.transform.localPosition = cameraPos[(byte)currentCamera];
        
    }

    public void rotateCamera(float mouseDifY)
    {
        // Actualizează rotația pe axa Y și o limitează
        yRotation -= mouseDifY;
        yRotation = Mathf.Clamp(yRotation, 10f, 40f); // Limitează între -45 și 45 grade

        // Aplică rotația pe axa Y, menținând rotația actuală pe axele X și Z
        cam.transform.localEulerAngles = new Vector3(yRotation, cam.transform.localEulerAngles.y, 0);
    }
}
