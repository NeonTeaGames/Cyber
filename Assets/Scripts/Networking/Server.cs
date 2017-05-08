using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

    private static Server Singleton;
    
	// Use this for initialization
	void Start () {
        Singleton = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool LaunchServer(int port) {
        if (NetworkServer.active) {
            return false;
        }

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

        return true;
    }

    // Custon packet handler

    private void HandlePacket(NetworkMessage msg) {

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

    // Internal built-in event handler

    private void OnConnected(NetworkMessage msg) {
        Debug.Log("Someone connected!");
        Term.Println("Someone connected!");
    }

    private void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Someone disconnected.");
        Term.Println("Someone disconnected.");
    }

    private void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error on server");
        Term.Println("Encountered a network error on server");
    }

    // Static methods for public usage

    /// <summary>
    /// Launches the server if not already launched.
    /// Returns false if the server was already launched, true otherwise.
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static bool Launch(int port) {
        return Singleton.LaunchServer(port);
    }

    /// <summary>
    /// Attempts to send a message to all clients who are listening.
    /// Returns false if server wasn't active, true otherwise.
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool SendToAll(short msgType, MessageBase message) {
        if (NetworkServer.active) {
            NetworkServer.SendToAll(msgType, message);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Attempts to send a message to a specific client.
    /// Returns false if server wasn't active, true otherwise.
    /// </summary>
    /// <param name="clientID"></param>
    /// <param name="msgType"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool Send(int clientID, short msgType, MessageBase message) {
        if (NetworkServer.active) {
            NetworkServer.SendToClient(clientID, msgType, message);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Is the server currently active.
    /// </summary>
    /// <returns></returns>
    public static bool isRunning() {
        return NetworkServer.active;
    }
}
