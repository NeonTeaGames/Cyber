using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LaunchServer(int port) {

        ConnectionConfig Config = new ConnectionConfig();
        Config.AddChannel(QosType.ReliableSequenced);
        Config.AddChannel(QosType.UnreliableSequenced);
        NetworkServer.Configure(Config, 10);

        NetworkServer.Listen(port);

        NetworkServer.RegisterHandler(PktType.TestMessage, HandlePacket);

        NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        NetworkServer.RegisterHandler(MsgType.Error, OnError);

        Debug.Log("Server started on port " + port);
    }

    public void HandlePacket(NetworkMessage msg) {

        switch(msg.msgType) {
        case PktType.TestMessage:
            TextMessage TextMsg = new TextMessage();
            TextMsg.Deserialize(msg.reader);
            Debug.Log("Received message: " + TextMsg.Message);
            break;
        default:
            Debug.LogError("Received an unknown packet, id: " + msg.msgType);
            break;
        }

    }

    public void OnConnected(NetworkMessage msg) {
        Debug.Log("Someone connected!");
    }

    public void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Someone disconnected?");
    }

    public void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error on server");
    }
}
