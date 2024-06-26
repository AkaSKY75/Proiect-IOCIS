using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnPlayerInProximity : MonoBehaviour
{
    [SerializeField]
    private GameObject baseNextTarget;

    void OnTriggerStay(Collider collider)
    {
        Transform playerTransform = collider.transform;
        if (playerTransform.name.Contains("Player")) {
            Destroy(transform.GetComponent<BoxCollider>());
            playerTransform.GetComponent<HelpPanelControl>().HideHelpPanel();
            playerTransform.GetComponent<Movements>().DisableMovements();
            
            Camera camera = playerTransform.Find("Camera").ConvertTo<Camera>();
            camera.gameObject.SetActive(false);
            Camera duplicateCamera = Instantiate(camera, playerTransform);
            if (duplicateCamera == null) {
                throw new System.Exception("Instantiation failed for duplicated camera!");
            }
            duplicateCamera.gameObject.SetActive(true);
            Camera.SetupCurrent(duplicateCamera);
            
            OnCameraRetarget onCameraRetargetScript = transform.GetComponent<OnCameraRetarget>();
            onCameraRetargetScript.SetPlayerCam(duplicateCamera);
            onCameraRetargetScript.SetNextTarget(playerTransform.gameObject);

            OnCameraRetarget onCameraRetargetScriptPlayer = playerTransform.GetComponent<OnCameraRetarget>();
            if (onCameraRetargetScriptPlayer == null) {
                throw new Exception("`OnCameraRetarget` component was not found for " + transform.name);
            }
            onCameraRetargetScriptPlayer.SetNextTarget(baseNextTarget);

            onCameraRetargetScript.OnStartMoveCamera();
            enabled = false;
        }
    }
}
