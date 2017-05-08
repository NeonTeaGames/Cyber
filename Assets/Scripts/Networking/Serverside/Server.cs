using Cyber.Console;
using Cyber.Networking.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Networking.Serverside {

    /// <summary>
    /// Server-class used to host a server and communicate to clients.
    /// </summary>
    public class Server : MonoBehaviour {

        private static Server Singleton;

        /// <summary>
        /// Creates the server-component, and sets the singleton as itself.
        /// </summary>
        public Server() {
            Singleton = this;
        }

        // Static methods for public usage

        /// <summary>
        /// Launches the server if not already launched.
        /// Returns false if the server was already launched, true otherwise.
        /// 
        /// Generally instead of this you should use <see cref="NetworkEstablisher.StartServer(int)"/>
        /// </summary>
        /// <param name="port">Port used to host the server.</param>
        /// <returns>Weather the launch was successful.</returns>
        public static bool Launch(int port) {
            return Singleton.LaunchServer(port);
        }

        /// <summary>
        /// Attempts to send a message to all clients who are listening.
        /// Returns false if server wasn't active, true otherwise.
        /// </summary>
        /// <param name="msgType">Type of the message being sent.</param>
        /// <param name="message">The message being sent.</param>
        /// <returns>Weather sending was successful.</returns>
        public static bool SendToAll(short msgType, MessageBase message) {
            if (NetworkServer.active) {
                NetworkServer.SendToAll(msgType, message);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Attempts to send a message to a specific client.
        /// Returns false if server wasn't active, true otherwise.
        /// </summary>
        /// <param name="clientID">ID of the client which to send this message.</param>
        /// <param name="msgType">Type of message being sent.</param>
        /// <param name="message">The message being sent.</param>
        /// <returns>Weather sending was successful.</returns>
        public static bool Send(int clientID, short msgType, MessageBase message) {
            if (NetworkServer.active) {
                NetworkServer.SendToClient(clientID, msgType, message);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Is the server currently active.
        /// </summary>
        /// <returns>Weather the server is running or not</returns>
        public static bool IsRunning() {
            return NetworkServer.active;
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

        private void HandlePacket(NetworkMessage msg) {

            switch (msg.msgType) {
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
    }
}