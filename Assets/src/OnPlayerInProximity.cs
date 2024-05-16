using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnPlayerInProximity : MonoBehaviour
{

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
            onCameraRetargetScript.OnStartMoveCamera();
            enabled = false;
        }
    }
}
