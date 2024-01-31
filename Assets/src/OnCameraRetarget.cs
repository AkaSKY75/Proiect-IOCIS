using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnToolSelect : MonoBehaviour
{
    public Camera playerCam;
    public Camera cam;
    public int speed = 5;
    public int rotationSpeed = 40;
    private enum CameraMovingStates:byte {
        NO = 0,
        MOVING,
        ROTATING,
        FINISHED
    }

    private CameraMovingStates playerCameraState;

    public void OnStartMoveCamera() {
        playerCameraState = CameraMovingStates.MOVING;
    }

    void Start() {
        playerCameraState = CameraMovingStates.NO;
    }
    void Update()
    {
        if (playerCameraState == CameraMovingStates.MOVING) {
            Quaternion targetRotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
            playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, cam.transform.position, speed * Time.deltaTime);

            if (playerCam.transform.position == cam.transform.position) {
                if (playerCam.transform.rotation == cam.transform.rotation) {
                    playerCameraState = CameraMovingStates.FINISHED;
                } else {
                    playerCameraState = CameraMovingStates.ROTATING;
                }
            }
        } else if(playerCameraState == CameraMovingStates.ROTATING) {
            if (playerCam.transform.rotation != cam.transform.rotation) {
                Quaternion targetRotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
                playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            } else {
                playerCameraState = CameraMovingStates.FINISHED;
            }
        }
    }
}
