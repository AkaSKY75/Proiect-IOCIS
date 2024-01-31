using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnPlayerInProximity : MonoBehaviour
{
    public GameObject player;
    public Camera cam, playerCam;
    private GameObject playerCamDuplicated;
    public Transform notepad;
    public GameObject nextButton;
    public int speed = 5;
    public int rotationSpeed = 25;
    private bool shouldCollide;
    private Transform targetTransform;

    private enum CameraMovingStates:byte {
        NO = 0,
        MOVING,
        ROTATING,
        FINISHED
    }
    private CameraMovingStates playerCameraState;

    void Start() {
        playerCameraState = CameraMovingStates.NO;
        shouldCollide = true;
    }

    void Update() {
        if (playerCameraState == CameraMovingStates.MOVING) {
            /*Vector3 targetDirection = notepad.position - playerCam.transform.position;
            Vector3 newDirection = Vector3.RotateTowards(playerCam.transform.forward, targetDirection, speed * Time.deltaTime, 0f);
            playerCam.transform.rotation = Quaternion.LookRotation(newDirection);*/

            // Rotate towards the target's rotation
            Quaternion rotation = Quaternion.LookRotation(targetTransform.forward, targetTransform.up);
            playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, targetTransform.position, speed * Time.deltaTime);

            if (playerCam.transform.position == targetTransform.position) {
                if (playerCam.transform.rotation == targetTransform.rotation) {
                    if (shouldCollide == true) {
                        Cursor.visible = true;
                        nextButton.SetActive(true);
                        playerCameraState = CameraMovingStates.FINISHED;
                    } else {
                        player.GetComponent<Movements>().EnableMovements();
                        Destroy(playerCamDuplicated);
                    }
                } else {
                    playerCameraState = CameraMovingStates.ROTATING;
                }
            }
        } else if(playerCameraState == CameraMovingStates.ROTATING) {
            Debug.Log(playerCam.transform.rotation + " " + targetTransform.rotation);
            if (playerCam.transform.rotation != targetTransform.rotation) {
                Quaternion rotation = Quaternion.LookRotation(targetTransform.forward, targetTransform.up);
                playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            } else {
                if (shouldCollide == true) {
                    Cursor.visible = true;
                    nextButton.SetActive(true);
                } else {
                    player.GetComponent<Movements>().EnableMovements();
                    Destroy(playerCamDuplicated);
                }
                playerCameraState = CameraMovingStates.FINISHED;
            }
        }
    }

    public void ResetPlayerCamera() {
        targetTransform = playerCamDuplicated.transform;
        playerCameraState = CameraMovingStates.MOVING;
        shouldCollide = false;
        Destroy(transform.GetComponent<BoxCollider>());
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject == player && shouldCollide == true && playerCameraState == CameraMovingStates.NO) {
            playerCamDuplicated = new GameObject("CameraDuplicated");
            playerCamDuplicated.SetActive(false);
            playerCamDuplicated.transform.parent = player.transform;
            playerCamDuplicated.AddComponent<Camera>();
            playerCamDuplicated.GetComponent<Camera>().CopyFrom(playerCam);
            playerCamDuplicated.transform.parent = player.transform;
            Debug.Log(playerCam.transform.rotation + " " + playerCamDuplicated.transform.rotation);
            targetTransform = cam.transform;

            player.GetComponent<HelpPanelControl>().HideHelpPanel();
            playerCameraState = CameraMovingStates.MOVING;
            player.GetComponent<Movements>().DisableMovements();
        }
    }
}
