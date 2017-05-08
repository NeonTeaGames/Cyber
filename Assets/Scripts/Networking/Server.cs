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

        NetworkServer.RegisterHandler(PktType.TextMessage, HandlePacket);

        NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        NetworkServer.RegisterHandler(MsgType.Error, OnError);

        Debug.Log("Server started on port " + port);
        Term.Println("Server started on port " + port);

        Term.AddCommand("send (message)", "Howl at the darkness of space. Does it echo though?", (args) => {
            Term.Println("You: " + args[0]);
            SendToAll(PktType.TextMessage, new TextMessage("Server: " + args[0]));
        });
    }

    public void HandlePacket(NetworkMessage msg) {

        switch(msg.msgType) {
        case PktType.TextMessage:
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

    public void SendToAll(short type, MessageBase message) {
        NetworkServer.SendToAll(type, message);
    }

    public void OnConnected(NetworkMessage msg) {
        Debug.Log("Someone connected!");
        Term.Println("Someone connected!");
    }

    public void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Someone disconnected.");
        Term.Println("Someone disconnected.");
    }

    public void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error on server");
        Term.Println("Encountered a network error on server");
    }
}
