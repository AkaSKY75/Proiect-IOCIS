using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class HelpPanelControl : NetworkBehaviour
{   
    private bool isShown;

    private Action[] action;
    [SerializeField]
    private List<GameObject> objectsToHide;

    private void Start() {
        if (!IsOwner) {
            return;
        }
        isShown = true;
        action = new Action[2] {
            HideHelpPanel,
            ShowHelpPanel
        };
    }

    public void ToggleHelpPanel() {
        if (!IsOwner) {
            return;
        }
        isShown = !isShown;
        action[Convert.ToInt32(isShown)]();
    }

    public void HideHelpPanel() {
        isShown = false;
        foreach(var objectToHide in objectsToHide) {
            objectToHide.SetActive(false);
        }
    }
    public void ShowHelpPanel() {
        isShown = true;
        foreach(var objectToHide in objectsToHide) {
            objectToHide.SetActive(true);
        }
    }

    public void EnableWebCameraPanel(GameObject webCamPanel) {
        webCamPanel.SetActive(true);
    }

    public void DisableWebCameraPanel(GameObject webCamPanel) {
        webCamPanel.SetActive(false);
    }
}
