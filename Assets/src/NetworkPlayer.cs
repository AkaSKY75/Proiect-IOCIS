using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerCamera; 
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button button;

    void Start()
    {
        InitializeCamera();
    }

    void InitializeCamera()
    {
        if (IsLocalPlayer)
        {
            playerCamera.SetActive(true);
        }
        else
        {
            playerCamera.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
            inputField.text = OwnerClientId.ToString();
            transform.name = "Player" + OwnerClientId.ToString();
            transform.position = new Vector3(-4581f, 9890.5f, -169f);
    }

    public void UpdateButtonClick(Action f)
    {
        button.onClick.AddListener(() => { Debug.LogWarning("Button clicked"); f.Invoke(); });
    }

    public void ShowIP(ushort ip)
    {
        if (IsLocalPlayer)
        {
            inputField.text = ip.ToString();
        }
    }

    public ushort GetIP()
    {
        if (IsLocalPlayer)
        {
            return Convert.ToUInt16(inputField.text);
        }
        return 0;
    }
}
