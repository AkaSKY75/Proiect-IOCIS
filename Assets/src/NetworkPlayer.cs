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

    public override void OnNetworkSpawn()
    {
        Move();
    }

    public void Move()
    {
        SubmitPositionRequestServerRpc();
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        transform.position = new Vector3(-4581f ,9890.5f ,-169f);
        Position.Value = transform.position;
    }

    void Update()
    {
        Position.Value = transform.position;
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