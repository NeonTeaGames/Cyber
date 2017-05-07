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
    public InputField clientPortField;

    [Tooltip("Required field only if StartServer() is used.")]
    public InputField serverPortField;

    public GameObject worldRoot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartClient() {
        string ip = IPField.text;
        if (ip.Length == 0) {
            ip = "localhost";
        }
        string portText = clientPortField.text;
        int port = 3935;
        if (portText.Length > 0) {
            port = Int32.Parse(portText);
        }
        StartClient(ip, port);
    }

    public void StartClient(string ip, int port) {
        Client client = worldRoot.AddComponent<Client>();
        client.LaunchClient(ip, port);
    }

    public void StartServer() {
        string portText = serverPortField.text;
        int port = 3935;
        if (portText.Length > 0) {
            port = Int32.Parse(portText);
        }
        StartServer(port);
    }

    public void StartServer(int port) {
        Server server = worldRoot.AddComponent<Server>();
        server.LaunchServer(port);
    }
}
