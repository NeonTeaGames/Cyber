using Cyber.Console;
using Cyber.Entities;
using Cyber.Entities.SyncBases;
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
        /// The Syncer which syncs. <see cref="Syncer"/>
        /// </summary>
        public Syncer Syncer;

        private ServerSyncHandler ServerSyncHandler;

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
        /// Sends Message to all clients using specified channel. 
        /// <see cref="SendToAll(short, MessageBase)"/> defaults to <see cref="NetworkChannelID.ReliableSequenced"/>.
        /// </summary>
        /// <param name="msgType">Message type being sent.</param>
        /// <param name="message">Message contents.</param>
        /// <param name="channel">Channel being used.</param>
        /// <returns></returns>
        public static bool SendToAllByChannel(short msgType, MessageBase message, byte channel) {
            if (NetworkServer.active) {
                NetworkServer.SendByChannelToAll(msgType, message, channel);
                return true;
            } else {
                return false;
            }
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

        /// <summary>
        /// Properly shuts the server down.
        /// \todo Server should inform all client that the server was shut down.
        /// </summary>
        /// <returns>True if the server was running, false if not.</returns>
        public static bool Shutdown() {
            if (NetworkServer.active) {
                foreach (var Entry in Singleton.Players) {
                    Singleton.Spawner.Remove(Entry.Value.Character.gameObject);
                }
                Singleton.Players.Clear();
                NetworkServer.Shutdown();
                return true;
            } else {
                return false;
            }
        }

        private bool LaunchServer(int port) {
            if (NetworkServer.active) {
                return false;
            }

            Spawner = GetComponent<Spawner>();

            ConnectionConfig Config = new ConnectionConfig();
            NetworkChannelID.ReliableSequenced = Config.AddChannel(QosType.ReliableSequenced);
            NetworkChannelID.UnreliableSequenced = Config.AddChannel(QosType.UnreliableSequenced);
            NetworkChannelID.Unreliable = Config.AddChannel(QosType.Unreliable);
            NetworkServer.Configure(Config, 10);

            NetworkServer.Listen(port);

            Spawner.SyncDB.SetStaticObjectsIDs();

            NetworkServer.RegisterHandler(PktType.TextMessage, HandlePacket);
            NetworkServer.RegisterHandler(PktType.MoveCreature, HandlePacket);
            NetworkServer.RegisterHandler(PktType.Interact, HandlePacket);
            NetworkServer.RegisterHandler(PktType.ClientSync, HandlePacket);
            NetworkServer.RegisterHandler(PktType.Disconnect, HandlePacket);

            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetworkServer.RegisterHandler(MsgType.Error, OnError);

            Debug.Log("Server started on port " + port);
            Term.Println("Server started on port " + port);

            Term.AddCommand("send (message)", "Howl at the darkness of space. Does it echo though?", (args) => {
                Term.Println("You: " + args[0]);
                SendToAll(PktType.TextMessage, new TextMessagePkt("Server: " + args[0]));
            });

            Term.AddCommand("unhost", "Shuts down the server, shut it all down!", (args) => {
                Term.Println("Unhosting the server.");
                Shutdown();
            });

            Syncer = gameObject.AddComponent<Syncer>();

            ServerSyncHandler = new ServerSyncHandler(Players);

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
                if (Controlled.ID != MoveCreature.SyncBaseID) {
                    break;
                }

                Controlled.Move(MoveCreature.Direction);

                foreach (var Player in Players) {
                    if (Player.Value.Connection.connectionId == msg.conn.connectionId) {
                        continue;
                    }
                    MoveCreature.Timestamp = NetworkHelper.GetCurrentSystemTime();
                    NetworkServer.SendToClient(Player.Value.Connection.connectionId, PktType.MoveCreature, MoveCreature);
                }
                break;
            case PktType.Interact:
                InteractionPkt Interaction = new InteractionPkt();
                Interaction.Deserialize(msg.reader);

                Character Sender = Players[msg.conn.connectionId].Character;
                SyncBase Target = Spawner.SyncDB.Get(Interaction.InteractSyncBaseID);

                Interaction.OwnerSyncBaseID = Sender.ID;

                if (Target != null && Target is Interactable) {
                    Interactable Interacted = (Interactable) Target;
                    Vector3 Delta = Interacted.gameObject.transform.position - Sender.gameObject.transform.position;
                    float ServerInteractionDistance = Sender.InteractionDistance + Sender.MovementSpeed * 0.5f;
                    if (Delta.magnitude <= ServerInteractionDistance) {
                        Interacted.Interact(Sender, Interaction.InteractionType);
                        NetworkServer.SendToAll(PktType.Interact, Interaction);
                        if (Interacted.GetInteractableSyncdata().RequiresSyncing) {
                            Syncer.DirtSyncBase(Interacted.ID);
                        }
                    }
                } else {
                    Term.Println("Client has reported an erronous SyncBase ID!");
                }

                break;
            case PktType.ClientSync:
                ServerSyncHandler.HandleSyncPkt(msg);
                break;
            case PktType.Disconnect:
                msg.conn.Disconnect();
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

            Term.Println(Id + " connected!");

            // Send all other clients a notification and collect their id's
            int[] IdList = new int[Players.Count];
            int TempCounter = 0;
            foreach (SConnectedPlayer P in Players.Values) {
                IdList[TempCounter++] = P.Connection.connectionId;
                NetworkServer.SendToClient(P.Connection.connectionId, PktType.Identity, new IdentityPkt(Id, false));
            }

            // Then send the client a list of all other clients
            NetworkServer.SendToClient(Id, PktType.MassIdentity, new IntListPkt(IdList));

            // Add the player to the list
            SConnectedPlayer Player = new SConnectedPlayer(msg.conn);
            Players.Add(Id, Player);

            // Send the previously collected list to the player
            NetworkServer.SendToClient(msg.conn.connectionId, 
                PktType.Identity, new IdentityPkt(msg.conn.connectionId, true));

            // Spawn the player and collect it's IDs
            Vector3 Position = new Vector3(0, 0, 0);
            GameObject Obj = Spawner.Spawn(EntityType.NPC, Position);
            int[] EntityIdList = Spawner.SyncDB.GetEntityIDs(Obj);
            Player.Character = Obj.GetComponent<Character>();

            NetworkServer.SendToAll(PktType.SpawnEntity, new SpawnEntityPkt(EntityType.NPC, Position, EntityIdList, Id));

            // Send ID's of every existing static SyncBase object in the world.
            NetworkServer.SendToClient(Id, PktType.StaticObjectIds, new IntListPkt(Spawner.SyncDB.GetStaticSyncBaseIDList()));

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
            // Get client's ID
            int Id = msg.conn.connectionId;

            Term.Println(Id + " disconnected.");

            Spawner.Remove(Players[Id].Character.gameObject);
            Players.Remove(Id);
            ServerSyncHandler.ClearConnectionFromSyncDict(Id);

            foreach (var Entry in Players) {
                Send(Entry.Key, PktType.Disconnect, new DisconnectPkt(Id));
            }
        }

        private void OnError(NetworkMessage msg) {
            Debug.LogError("Encountered a network error on server");
            Term.Println("Encountered a network error on server");
        }
    }
}