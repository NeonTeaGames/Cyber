using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Client-class used to connecting to a server and communicating to it.
/// Also handles all events incoming from the server and forwards them where they should be handled.
/// </summary>
public class Client : MonoBehaviour {

    NetworkClient NetClient;
    private bool Running = false;

    private static Client Singleton;

    public Client() {
        Singleton = this;
    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	}

    // Launches the client at given IP and port.

    private bool LaunchClient(string ip, int port) {
        if (Running) {
            return false;
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

        return true;
    }

    // Handles custom packets

    private void HandlePacket(NetworkMessage msg) {
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

    // Built-in handles for some events
    
    private void OnConnected(NetworkMessage msg) {
        Debug.Log("Connected!");
        Term.Println("Connected!");

        Term.AddCommand("send (message)", "Send a message across the vastness of space and time!", (args) => {
            Term.Println("You: " + args[0]);
            NetClient.Send(PktType.TextMessage, new TextMessage("A Client: " + args[0]));
        });
    }

    private void OnDisconnected(NetworkMessage msg) {
        Debug.Log("Disconnected!");
        Term.Println("Disconnected!");
        Running = false;
    }

    private void OnError(NetworkMessage msg) {
        Debug.LogError("Encountered a network error. Shutting down.");
        Term.Println("Encountered a network error. Shutting down.");
        NetClient.Disconnect();
        Running = false;
    }

    // Static interface for usage outside of Client

    /// <summary>
    /// Will launch the client and attempt to connect to the server.
    /// Returns false if client is already running, otherwise true.
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static bool Launch(string ip, int port) {
        return Singleton.LaunchClient(ip, port);
    }

    /// <summary>
    /// Send messages to the server if the connection is active.
    /// If client is not active, this will return false, otherwise true.
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool Send(short msgType, MessageBase message) {
        if (Singleton.Running) {
            Singleton.NetClient.Send(msgType, message);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Returns if the client is running or not. 
    /// This is independant weather the client is connected or not.
    /// </summary>
    /// <returns></returns>
    public static bool isRunning() {
        return Singleton.Running;
    }

    /// <summary>
    /// Returns if the client is connected or not.
    /// </summary>
    /// <returns></returns>
    public static bool isConnected() {
        return Singleton.NetClient.isConnected;
    }
    
}
