using System;
using UnityEngine;
using UnityEngine.UI;
using Cyber.Networking.Clientside;
using Cyber.Networking.Serverside;

namespace Cyber.Networking {

    /// <summary>
    /// This class is used pretty much anywhere in order to make the "first step" of networking.
    /// It adds the proper components to World Root and tells them to start.
    /// </summary>
    public class NetworkEstablisher : MonoBehaviour {

        /// <summary>
        /// Input field for the IP to which connect to.
        /// </summary>
        [Tooltip("Required field only if StartClient() is used.")]
        public InputField IPField;
        /// <summary>
        /// Input field for the client port
        /// </summary>
        [Tooltip("Required field only if StartClient() is used.")]
        public InputField ClientPortField;

        /// <summary>
        /// Input field for the server port
        /// </summary>
        [Tooltip("Required field only if StartServer() is used.")]
        public InputField ServerPortField;

        /// <summary>
        /// World Root node, a GameObject.
        /// </summary>
        public GameObject WorldRoot;

	    // Use this for initialization
	    void Start () {
            Term.AddCommand("join", "joins a server at localhost:3935", (args) => {
                StartClient("localhost", 3935);
            });
            Term.AddCommand("join (ip)", "joins a server at given ip and port 3935", (args) => {
                string ip = args[0];
                StartClient(ip, 3935);
            });
            Term.AddCommand("join (ip) (port)", "joins a server at given ip and port", (args) => {
                string ip = args[0];
                int port = 3935;
                int.TryParse(args[1], out port);
                StartClient(ip, port);
            });

            Term.AddCommand("host", "host a server at port 3935", (args) => {
                StartServer(3935);
            });
            Term.AddCommand("host (port)", "host a server at given port", (args) => {
                int port = 3935;
                int.TryParse(args[0], out port);
                StartServer(port);
            });
        }

        /// <summary>
        /// Starts the client using given input fields. Otherwise functions like <see cref="StartClient(string, int)"/>.
        /// </summary>
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

        /// <summary>
        /// Starts the client with the given ip and port.
        /// This initializes the Client component and launches it properly.
        /// </summary>
        /// <param name="ip">IP used to connect.</param>
        /// <param name="port">port of the host.</param>
        public void StartClient(string ip, int port) {
            WorldRoot.AddComponent<Client>();
            Client.Launch(ip, port);
        }

        /// <summary>
        /// Starts the server using given input fields. Otherwise functions like <see cref="StartServer(int)"/>.
        /// </summary>
        public void StartServer() {
            string PortText = ServerPortField.text;
            int Port = 3935;
            if (PortText.Length > 0) {
                Port = Int32.Parse(PortText);
            }
            StartServer(Port);
        }

        /// <summary>
        /// Starts the server using given port.
        /// This initializes the port and launches the server properly.
        /// </summary>
        /// <param name="port">port used for the server.</param>
        public void StartServer(int port) {
            WorldRoot.AddComponent<Server>();
            Server.Launch(port);
        }
    }
}
