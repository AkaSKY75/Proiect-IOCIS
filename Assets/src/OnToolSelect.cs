using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnToolSelect : MonoBehaviour
{
    public Camera playerCam;
    public Camera cam;
    public int speed = 5;
    private enum CameraMovingStates:byte {
        NO = 0,
        MOVING,
        FINISHED
    }

    private CameraMovingStates cameraMovingState;

    public void OnStartMoveCamera() {
        cameraMovingState = CameraMovingStates.MOVING;
    }

    void Start() {
        cameraMovingState = CameraMovingStates.NO;
    }
    void Update()
    {
        if (cameraMovingState == CameraMovingStates.MOVING) {
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, cam.transform.position, speed * Time.deltaTime);

            if (playerCam.transform.position == cam.transform.position) {
                playerCam.transform.rotation = cam.transform.rotation;
                cameraMovingState = CameraMovingStates.FINISHED;
            }
        }
    }
}
