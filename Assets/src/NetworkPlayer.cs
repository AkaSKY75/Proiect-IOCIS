using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button button;
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    // TO DO:
    /*
        La fiecare client nou conectat, pe ServerRpc pune camera player-ului pe disabled
        La fiecare client nou conectat, parcruge toti ceilalti clienti (inclusiv serverul) si pune prin ClientRpc camera lor pe disabled
    */
    public override void OnNetworkSpawn()
    {
        inputField.text = OwnerClientId.ToString();
        transform.name = "Player" + OwnerClientId.ToString();
        transform.position = new Vector3(-4581f, 9890.5f, -169f);
        if (NetworkManager.Singleton.ConnectedClients.Count > 1) {
            HideCameraForServerRpc(OwnerClientId);
            HideCameraForClientRpc(OwnerClientId);
        }
    }

    [ServerRpc]
    void HideCameraForServerRpc(ulong id, ServerRpcParams rpcParams = default)
    {
        NetworkManager.Singleton.ConnectedClients[id].PlayerObject.transform.Find("Camera").gameObject.SetActive(false);
    }

    [ClientRpc]
    void HideCameraForClientRpc(ulong id, ClientRpcParams rpcParams = default)
    {
        foreach(var connectedClient in NetworkManager.Singleton.ConnectedClients) {
            if (connectedClient.Key != id) {
                connectedClient.Value.PlayerObject.transform.Find("Camera").gameObject.SetActive(false);
            }
        }
    }

    public void UpdateButtonClick(Action f) {
        button.onClick.AddListener(() => { Debug.LogWarning("Button clicked"); f.Invoke(); });
    }

    public void ShowIP(ushort ip) {
        inputField.text = ip.ToString();
    }

    public ushort GetIP() {
        return System.Convert.ToUInt16(inputField.text);
    }
}