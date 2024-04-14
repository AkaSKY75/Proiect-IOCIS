using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HelpPanelControl : MonoBehaviour
{   
    private bool isShown;
    public GameObject panel;

    private Action[] action;

    private void Start() {
        isShown = true;
        action = new Action[2] {
            HideHelpPanel,
            ShowHelpPanel
        };
    }

    public void ToggleHelpPanel() {
        isShown = !isShown;
        action[Convert.ToInt32(isShown)]();
    }

    public void HideHelpPanel() {
        isShown = false;
        panel.SetActive(false);
    }
    public void ShowHelpPanel() {
        isShown = true;
        panel.SetActive(true);
    }
}
