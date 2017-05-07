using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

    NetworkClient client;
    private bool running = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if (client.isConnected) {
        }
	}

    public void LaunchClient(string ip, int port) {
        if (running) {
            return;
        }

        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.UnreliableSequenced);
        NetworkServer.Configure(config, 10);

        client = new NetworkClient();
        client.Configure(config, 10);

        running = true;

        client.RegisterHandler(MsgType.Connect, OnConnected);
        client.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        client.RegisterHandler(MsgType.Error, OnError);

        client.Connect(ip, port);

        Debug.Log("Client launched!");
    }

    public void OnConnected(NetworkMessage msg) {
        Debug.Log("Connected!");
        client.Send(PktType.TestMessage, new TextMessage("Hai, I connected!"));
    }

    public void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Disconnected!");
    }

    public void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error. Shutting down.");
        client.Disconnect();
        running = false;
    }
}
