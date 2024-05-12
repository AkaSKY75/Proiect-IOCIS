using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : MonoBehaviour
{
    private Thread thread = null;
    private ushort port;
    void Start() {
        var port = (ushort)Random.Range(1024, 65535);
        this.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", port);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.First().Value.GetComponent<NetworkPlayer>().UpdateButtonClick(ConnectToServer);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.First().Value.GetComponent<NetworkPlayer>().ShowIP(port);
    }

    void Update() {
        if (thread != null) {
            if (thread.ThreadState != ThreadState.Running) {
                thread = null;
                this.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", port);
                NetworkManager.Singleton.StartClient();
            }
        }
    }

    public void ConnectToServer() {
        port = NetworkManager.Singleton.SpawnManager.SpawnedObjects.First().Value.GetComponent<NetworkPlayer>().GetIP();
        NetworkManager.Singleton.Shutdown();
        thread = new Thread(new ThreadStart(() => {
            while (NetworkManager.Singleton.ShutdownInProgress) {
                Debug.LogWarning("Thread is running...");
                Thread.Sleep(2000);
            }
        }));
        thread.Start();
    }

}
