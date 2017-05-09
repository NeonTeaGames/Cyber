using Cyber.Console;
using Cyber.Entities;
using Cyber.Networking.Messages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Networking.Serverside {

    /// <summary>
    /// Server-class used to host a server and communicate to clients.
    /// </summary>
    /// \todo Change connection channels to Unreliable to optimize ping.
    /// \todo Remove PC/NPC and make a Human-type entity instead.
    public class Server : MonoBehaviour {

        private Dictionary<int, SConnectedPlayer> Players = new Dictionary<int, SConnectedPlayer>();
        private static Server Singleton;

        private Spawner Spawner;

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

            Spawner = GetComponent<Spawner>();

            ConnectionConfig Config = new ConnectionConfig();
            Config.AddChannel(QosType.ReliableSequenced);
            Config.AddChannel(QosType.UnreliableSequenced);
            NetworkServer.Configure(Config, 10);

            NetworkServer.Listen(port);

            NetworkServer.RegisterHandler(PktType.TextMessage, HandlePacket);
            NetworkServer.RegisterHandler(PktType.MoveCreature, HandlePacket);

            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetworkServer.RegisterHandler(MsgType.Error, OnError);

            Debug.Log("Server started on port " + port);
            Term.Println("Server started on port " + port);

            Term.AddCommand("send (message)", "Howl at the darkness of space. Does it echo though?", (args) => {
                Term.Println("You: " + args[0]);
                SendToAll(PktType.TextMessage, new TextMessagePkt("Server: " + args[0]));
            });

            return true;
        }

        private void HandlePacket(NetworkMessage msg) {

            switch (msg.msgType) {
                case PktType.TextMessage:
                    TextMessagePkt TextMsg = new TextMessagePkt();
                    TextMsg.Deserialize(msg.reader);
                    Term.Println(TextMsg.Message);
                    break;
                case PktType.MoveCreature:
                    MoveCreaturePkt MoveCreature = new MoveCreaturePkt();
                    MoveCreature.Deserialize(msg.reader);

                    // Check if the player is allowed to move this character
                    Character Controlled = Players[msg.conn.connectionId].Character.GetComponent<Character>();
                    Debug.Log(Controlled.ID);
                    Debug.Log(MoveCreature.SyncBaseID);
                    if (Controlled.ID != MoveCreature.SyncBaseID) {
                        break;
                    }

                    Controlled.Move(MoveCreature.Direction);

                    foreach (var Player in Players) {
                        if (Player.Value.ConnectionID == msg.conn.connectionId) {
                            continue;
                        }
                        NetworkServer.SendToClient(Player.Value.ConnectionID, PktType.MoveCreature, MoveCreature);
                    }
                    break;
                default:
                    Debug.LogError("Received an unknown packet, id: " + msg.msgType);
                    Term.Println("Received an unknown packet, id: " + msg.msgType);
                    break;
            }
        }

        // Internal built-in event handler

        private void OnConnected(NetworkMessage msg) {           
            // Get client's ID
            int Id = msg.conn.connectionId;

            Debug.Log(Id + " connected!");
            Term.Println(Id + " connected!");

            // Send all other clients a notification and collect their id's
            int[] IdList = new int[Players.Count];
            int TempCounter = 0;
            foreach (SConnectedPlayer P in Players.Values) {
                IdList[TempCounter++] = P.ConnectionID;
                NetworkServer.SendToClient(P.ConnectionID, PktType.Identity, new IdentityPkt(Id, false));
            }

            // Then send the client a list of all other clients
            NetworkServer.SendToClient(Id, PktType.MassIdentity, new MassIdentityPkt(IdList));

            // Add the player to the list
            SConnectedPlayer Player = new SConnectedPlayer(msg.conn.connectionId);
            Players.Add(Id, Player);

            // Send the previously collected list to the player
            NetworkServer.SendToClient(msg.conn.connectionId, 
                PktType.Identity, new IdentityPkt(msg.conn.connectionId, true));

            // Spawn the player and collet it's IDs
            Vector3 Position = new Vector3(0, 0, 0);
            GameObject Obj = Spawner.Spawn(EntityType.NPC, Position);
            int[] EntityIdList = Spawner.SyncDB.GetEntityIDs(Obj);
            Player.Character = Obj.GetComponent<Character>();

            NetworkServer.SendToAll(PktType.SpawnEntity, new SpawnEntityPkt(EntityType.NPC, Position, EntityIdList, Id));

            // Send every entity to the player who just connected.
            foreach (var Entry in Players) {
                if (Entry.Key == Id) {
                    continue;
                }
                Character Char = Players[Entry.Key].Character;
                GameObject CurrObj = Char.gameObject;
                int[] CurrEntityIdList = Spawner.SyncDB.GetEntityIDs(CurrObj);
                NetworkServer.SendToClient(Id, PktType.SpawnEntity, 
                    new SpawnEntityPkt(EntityType.NPC, CurrObj.transform.position, CurrEntityIdList, Entry.Key));
            }
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