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
            hideHelpPanel,
            showHelpPanel
        };
    }

    public void toggleHelpPanel() {
        isShown = !isShown;
        action[Convert.ToInt32(isShown)]();
    }

    private void hideHelpPanel() {
        panel.SetActive(false);
    }
    private void showHelpPanel() {
        panel.SetActive(true);
    }
}
