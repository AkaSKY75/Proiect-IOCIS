using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPlayerInProximity : MonoBehaviour
{
    public GameObject player;
    public Camera cam, playerCam;
    public Transform notepad;
    public GameObject nextButton;
    public int speed = 5;
    private enum CameraMovingStates:byte {
        NO = 0,
        MOVING,
        FINISHED
    }
    private CameraMovingStates playerCameraState;

    void Start() {
        playerCameraState = CameraMovingStates.NO;
    }

    void Update() {
        if (playerCameraState == CameraMovingStates.MOVING) {
            /*Vector3 targetDirection = notepad.position - playerCam.transform.position;
            Vector3 newDirection = Vector3.RotateTowards(playerCam.transform.forward, targetDirection, speed * Time.deltaTime, 0f);
            playerCam.transform.rotation = Quaternion.LookRotation(newDirection);*/

            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, cam.transform.position, speed * Time.deltaTime);

            if (playerCam.transform.position == cam.transform.position) {
                playerCam.transform.rotation = cam.transform.rotation;
                Cursor.visible = true;
                nextButton.SetActive(true);
                playerCameraState = CameraMovingStates.FINISHED;
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject == player && playerCameraState == CameraMovingStates.NO) {
            playerCameraState = CameraMovingStates.MOVING;
            player.GetComponent<Movements>().DisableMovements();
        }
    }
}
