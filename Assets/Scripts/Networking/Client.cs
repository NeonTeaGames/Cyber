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

        NetClient.RegisterHandler(PktType.TextMessage, HandlePacket);

        NetClient.RegisterHandler(MsgType.Connect, OnConnected);
        NetClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        NetClient.RegisterHandler(MsgType.Error, OnError);

        NetClient.Connect(ip, port);

        Debug.Log("Client launched!");
        Term.Println("Client launched!");
    }

    public void HandlePacket(NetworkMessage msg) {
        switch (msg.msgType) {
        case (PktType.TextMessage):
            TextMessage TextMsg = new TextMessage();
            TextMsg.Deserialize(msg.reader);
            Term.Println(TextMsg.Message);
            break;
        default:
            Debug.LogError("Received an unknown packet, id: " + msg.msgType);
            Term.Println("Received an unknown packet, id: " + msg.msgType);
            break;
        }
    }

    public void Send(short type, MessageBase message) {
        NetClient.Send(type, message);
    }

    public void OnConnected(NetworkMessage msg) {
        Debug.Log("Connected!");
        Term.Println("Connected!");

        Term.AddCommand("send (message)", "Send a message across the vastness of space and time!", (args) => {
            Term.Println("You: " + args[0]);
            Send(PktType.TextMessage, new TextMessage("A Client: " + args[0]));
        });
    }

    public void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Disconnected!");
        Term.Println("Disconnected!");
    }

    public void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error. Shutting down.");
        Term.Println("Encountered a network error. Shutting down.");
        NetClient.Disconnect();
        Running = false;
    }
}
