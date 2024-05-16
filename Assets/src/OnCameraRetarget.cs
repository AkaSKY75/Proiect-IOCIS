using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public delegate void FinishCallback();

public class OnCameraRetarget : MonoBehaviour
{
    public GameObject nextTarget;
    public int speed = 5;
    public int rotationSpeed = 40;
    private FinishCallback finishCallback;
    private int callbackNum = 1;
    private float threshold = 0.05f;
    private Camera nextTargetCam;
    private Camera playerCam;
    private enum CameraMovingStates:byte {
        NO = 0,
        MOVING,
        ROTATING,
        FINISHED
    }

    private CameraMovingStates playerCameraState;

    public void OnStartMoveCamera() {
        if (playerCam == null) {
            throw new Exception("playerCam is null!");
        }
        playerCameraState = CameraMovingStates.MOVING;
    }

    private void CheckIfTargetShouldRetargetOrReturn() {
        OnCameraRetarget onNextTargetCameraRetarget = nextTarget.GetComponent<OnCameraRetarget>();
        if (onNextTargetCameraRetarget != null) {
            onNextTargetCameraRetarget.SetPlayerCam(this.playerCam);
            if (onNextTargetCameraRetarget.nextTarget == nextTarget) {
                Camera originalCamera = this.playerCam.transform.parent.Find("Camera").ConvertTo<Camera>();
                GameObject player = originalCamera.transform.parent.gameObject;
                onNextTargetCameraRetarget.nextTarget = player;
                onNextTargetCameraRetarget.SetNextTargetCam(originalCamera);
                onNextTargetCameraRetarget.SetCallbacks(player);
            }
        }
    }

    public void SetPlayerCam(Camera camera) {
        this.playerCam = camera;
    }

    public void SetNextTargetCam(Camera camera) {
        this.nextTargetCam = camera;
    }

    public void SetCallbacks(GameObject nextTarget) {
        finishCallback = CheckIfTargetShouldRetargetOrReturn;
        AddCallback addCallback = nextTarget.GetComponent<AddCallback>();
        if (addCallback != null) {
            FinishCallback newCallbacks = addCallback.ReturnCallback();
            callbackNum += newCallbacks.GetInvocationList().Length;
            finishCallback += newCallbacks;
        }
    }

    void Start() {
        playerCameraState = CameraMovingStates.NO;
        nextTargetCam = nextTarget.transform.Find("Camera").ConvertTo<Camera>();
        SetCallbacks(nextTarget);
    }
    void Update()
    {
        if (playerCameraState == CameraMovingStates.MOVING) {
            // Debug.LogWarning("Position: " + playerCam.transform.position + ";" + nextTargetCam.transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(nextTargetCam.transform.forward, nextTargetCam.transform.up);
            playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, nextTargetCam.transform.position, speed * Time.deltaTime);

            if (Math.Abs(Math.Abs(playerCam.transform.position.x) - Math.Abs(nextTargetCam.transform.position.x)) < threshold &&
                    Math.Abs(Math.Abs(playerCam.transform.position.y) - Math.Abs(nextTargetCam.transform.position.y)) < threshold &&
                    Math.Abs(Math.Abs(playerCam.transform.position.z) - Math.Abs(nextTargetCam.transform.position.z)) < threshold) {
                if (Math.Abs(Math.Abs(playerCam.transform.rotation.eulerAngles.x) - Math.Abs(nextTargetCam.transform.rotation.eulerAngles.x)) < threshold &&
                    Math.Abs(Math.Abs(playerCam.transform.rotation.eulerAngles.y) - Math.Abs(nextTargetCam.transform.rotation.eulerAngles.y)) < threshold &&
                    Math.Abs(Math.Abs(playerCam.transform.rotation.eulerAngles.z) - Math.Abs(nextTargetCam.transform.rotation.eulerAngles.z)) < threshold) {
                    playerCameraState = CameraMovingStates.FINISHED;
                } else {
                    playerCameraState = CameraMovingStates.ROTATING;
                }
            }
        } else if(playerCameraState == CameraMovingStates.ROTATING) {
            if (Math.Abs(Math.Abs(playerCam.transform.rotation.eulerAngles.x) - Math.Abs(nextTargetCam.transform.rotation.eulerAngles.x)) >= threshold ||
                Math.Abs(Math.Abs(playerCam.transform.rotation.eulerAngles.y) - Math.Abs(nextTargetCam.transform.rotation.eulerAngles.y)) >= threshold ||
                Math.Abs(Math.Abs(playerCam.transform.rotation.eulerAngles.z) - Math.Abs(nextTargetCam.transform.rotation.eulerAngles.z)) >= threshold) {
                Quaternion targetRotation = Quaternion.LookRotation(nextTargetCam.transform.forward, nextTargetCam.transform.up);
                playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            } else {
                playerCameraState = CameraMovingStates.FINISHED;
            }
        } else if (playerCameraState == CameraMovingStates.FINISHED) {
            if (finishCallback.GetInvocationList().Length != callbackNum) {
                throw new Exception("Callbacks count mismatch: " + finishCallback.GetInvocationList().Length + " != " + callbackNum);
            }
            finishCallback();
            enabled = false;
        }
    }
}
