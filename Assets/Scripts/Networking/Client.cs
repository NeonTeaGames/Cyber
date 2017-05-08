using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

    NetworkClient NetClient;
    private bool Running = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if (NetClient.isConnected) {
        }
	}

    public void LaunchClient(string ip, int port) {
        if (Running) {
            return;
        }

        ConnectionConfig Config = new ConnectionConfig();
        Config.AddChannel(QosType.ReliableSequenced);
        Config.AddChannel(QosType.UnreliableSequenced);
        NetworkServer.Configure(Config, 10);

        NetClient = new NetworkClient();
        NetClient.Configure(Config, 10);

        Running = true;

        NetClient.RegisterHandler(MsgType.Connect, OnConnected);
        NetClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        NetClient.RegisterHandler(MsgType.Error, OnError);

        NetClient.Connect(ip, port);

        Debug.Log("Client launched!");
    }

    public void OnConnected(NetworkMessage msg) {
        Debug.Log("Connected!");
        NetClient.Send(PktType.TestMessage, new TextMessage("Hai, I connected!"));
    }

    public void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Disconnected!");
    }

    public void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error. Shutting down.");
        NetClient.Disconnect();
        Running = false;
    }
}
