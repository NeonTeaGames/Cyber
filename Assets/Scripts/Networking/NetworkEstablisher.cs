using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * This class is used pretty much anywhere in order to make the "first step" of networking.
 * It adds the proper components to World Root and tells them to start.
 * */
public class NetworkEstablisher : MonoBehaviour {

    [Tooltip("Required field only if StartClient() is used.")]
    public InputField IPField;
    [Tooltip("Required field only if StartClient() is used.")]
    public InputField ClientPortField;

    [Tooltip("Required field only if StartServer() is used.")]
    public InputField ServerPortField;

    public GameObject WorldRoot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartClient() {
        string IP = IPField.text;
        if (IP.Length == 0) {
            IP = "localhost";
        }
        string PortText = ClientPortField.text;
        int Port = 3935;
        if (PortText.Length > 0) {
            Port = Int32.Parse(PortText);
        }
        StartClient(IP, Port);
    }

    public void StartClient(string ip, int port) {
        Client Client = WorldRoot.AddComponent<Client>();
        Client.LaunchClient(ip, port);
    }

    public void StartServer() {
        string PortText = ServerPortField.text;
        int Port = 3935;
        if (PortText.Length > 0) {
            Port = Int32.Parse(PortText);
        }
        StartServer(Port);
    }

    public void StartServer(int port) {
        Server Server = WorldRoot.AddComponent<Server>();
        Server.LaunchServer(port);
    }
}
