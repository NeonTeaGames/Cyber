using Cyber.Console;
using Cyber.Entities;
using Cyber.Entities.SyncBases;
using Cyber.Items;
using Cyber.Networking.Messages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Networking.Clientside {

    /// <summary>
    /// Client-class used to connecting to a server and communicating to it.
    /// Also handles all events incoming from the server and forwards them where they should be handled.
    /// </summary>
    /// <remarks>
    /// The pipeline of requests sent by the client:
    /// 1. Player sends command to server (e.g. player presses 'w') and then processes it on the client.
    /// 2. Server receives the command and determines weather it's possible
    /// 3. Server then sends the results(along with a timestamp) to the clients
    /// 4. Clients receive their results and if the action was possible, starts the action and catches up to the server with the timestamp
    /// </remarks>
    public class Client : MonoBehaviour {

        private NetworkClient NetClient;
        private bool Running = false;

        private static Client Singleton;

        private Spawner Spawner;

        private SyncHandler SyncHandler;

        /// <summary>
        /// The player of this client
        /// </summary>
        private CConnectedPlayer Player;
        private Dictionary<int, CConnectedPlayer> Players = new Dictionary<int, CConnectedPlayer>();

        /// <summary>
        /// Creates the client and sets it as the signleton.
        /// </summary>
        public Client() {
            Singleton = this;
        }

        // Static interface for usage outside of Client

        /// <summary>
        /// Will launch the client and attempt to connect to the server.
        /// Returns false if client is already running, otherwise true.
        /// 
        /// Generally instead of this you should use <see cref="NetworkEstablisher.StartClient(string, int)"/>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>Weather the launch was successful or not.</returns>
        public static bool Launch(string ip, int port) {
            return Singleton.LaunchClient(ip, port);
        }

        /// <summary>
        /// Send messages to the server if the connection is active.
        /// If client is not active, this will return false, otherwise true.
        /// </summary>
        /// <param name="msgType">The message type.</param>
        /// <param name="message">The message contents.</param>
        /// <returns>Weather the send was successful or not.</returns>
        public static bool Send(short msgType, MessageBase message) {
            if (Singleton.Running) {
                Singleton.NetClient.Send(msgType, message);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Sends messages to the server by a specified channel.
        /// If the client is not active, this will return false, otherwise true.
        /// </summary>
        /// <param name="msgType">The message type.</param>
        /// <param name="message">The message contents.</param>
        /// <param name="channelID">The ID of the channel to be used.</param>
        /// <returns></returns>
        public static bool SendByChannel(short msgType, MessageBase message, byte channelID) {
            if (Singleton.Running) {
                Singleton.NetClient.SendByChannel(msgType, message, channelID);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns if the client is running or not. 
        /// This is independant weather the client is connected or not.
        /// </summary>
        /// <returns>Weather the client is running or not</returns>
        public static bool IsRunning() {
            return !(Singleton == null || !Singleton.Running);
        }

        /// <summary>
        /// Returns if the client is connected or not.
        /// </summary>
        /// <returns>Weather the client is connected or not.</returns>
        public static bool IsConnected() {
            return Singleton.NetClient.isConnected;
        }

        /// <summary>
        /// Returns the connected player.
        /// </summary>
        /// <returns>The connected player.</returns>
        public static CConnectedPlayer GetConnectedPlayer() {
            return Singleton.Player;
        }

        /// <summary>
        /// Properly shut down the client.
        /// </summary>
        /// <returns>True if the client was active, false if not.</returns>
        public static bool Shutdown() {
            if (IsRunning()) {
                Send(PktType.Disconnect, new DisconnectPkt());
                Singleton.NetClient.Shutdown();
                return true;
            } else {
                return false;
            }
        }

        private void Start() {
            Spawner = GetComponent<Spawner>();
            SyncHandler = new SyncHandler(Spawner.SyncDB);
        }

        private bool LaunchClient(string ip, int port) {
            if (Running) {
                return false;
            }

            ConnectionConfig Config = new ConnectionConfig();
            NetworkChannelID.ReliableSequenced = Config.AddChannel(QosType.ReliableSequenced);
            NetworkChannelID.UnreliableSequenced = Config.AddChannel(QosType.UnreliableSequenced);
            NetworkChannelID.Unreliable = Config.AddChannel(QosType.Unreliable);
            NetworkServer.Configure(Config, 10);

            NetClient = new NetworkClient();
            NetClient.Configure(Config, 10);

            Running = true;

            NetClient.RegisterHandler(PktType.TextMessage, HandlePacket);
            NetClient.RegisterHandler(PktType.Identity, HandlePacket);
            NetClient.RegisterHandler(PktType.MassIdentity, HandlePacket);
            NetClient.RegisterHandler(PktType.SpawnEntity, HandlePacket);
            NetClient.RegisterHandler(PktType.MoveCreature, HandlePacket);
            NetClient.RegisterHandler(PktType.Sync, HandlePacket);
            NetClient.RegisterHandler(PktType.Interact, HandlePacket);
            NetClient.RegisterHandler(PktType.StaticObjectIds, HandlePacket);
            NetClient.RegisterHandler(PktType.Disconnect, HandlePacket);
            NetClient.RegisterHandler(PktType.InventoryAction, HandlePacket);

            NetClient.RegisterHandler(MsgType.Connect, OnConnected);
            NetClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetClient.RegisterHandler(MsgType.Error, OnError);

            NetClient.Connect(ip, port);

            Debug.Log("Connecting..");
            Term.Println("Connecting..");

            return true;
        }

        private void HandlePacket(NetworkMessage msg) {
            switch (msg.msgType) {
            case (PktType.TextMessage):
                TextMessagePkt TextMsg = new TextMessagePkt();
                TextMsg.Deserialize(msg.reader);
                Term.Println(TextMsg.Message);
                break;
            case (PktType.Identity):
                IdentityPkt Identity = new IdentityPkt();
                Identity.Deserialize(msg.reader);
                var Conn = new CConnectedPlayer(Identity.ConnectionID);
                if (Identity.Owned) {
                    Player = Conn;
                } else {
                    Debug.Log(Conn.ConnectionID + " connected!");
                    Term.Println(Conn.ConnectionID + " connected!");
                }
                Players.Add(Conn.ConnectionID, Conn);
                break;
            case (PktType.MassIdentity):
                IntListPkt Identities = new IntListPkt();
                Identities.Deserialize(msg.reader);
                foreach (int currId in Identities.IdList) {
                    Players.Add(currId, new CConnectedPlayer(currId));
                }
                break;
            case (PktType.SpawnEntity):
                SpawnEntityPkt SpawnPkt = new SpawnEntityPkt();
                SpawnPkt.Deserialize(msg.reader);

                EntityType EntityType = SpawnPkt.EntityType;
                // Check if you are the owner and if they are spawning an NPC
                if (SpawnPkt.OwnerID == Player.ConnectionID && EntityType == EntityType.NPC) {
                    // Change it into a PC instead.
                    EntityType = EntityType.PC;
                }
                GameObject Obj = Spawner.Spawn(EntityType, SpawnPkt.Position, SpawnPkt.SyncBaseIDList);
                if (SpawnPkt.OwnerID > -1) {
                    Players[SpawnPkt.OwnerID].Character = Obj.GetComponent<Character>();
                }

                if (SpawnPkt.OwnerID == Player.ConnectionID) {
                    gameObject.AddComponent<ClientSyncer>();
                }
                break;
            case (PktType.MoveCreature):
                MoveCreaturePkt MoveCreature = new MoveCreaturePkt();
                MoveCreature.Deserialize(msg.reader);

                SyncBase SyncBase = Spawner.SyncDB.Get(MoveCreature.SyncBaseID);
                if (SyncBase != null || SyncBase is Character ) {
                    Character Character = (Character) SyncBase;
                    Character.Move(MoveCreature.Direction);
                } else {
                    Debug.LogError("SyncBase " + MoveCreature.SyncBaseID + " is not a Creature");
                    Term.Println("SyncBase " + MoveCreature.SyncBaseID + " is not a Creature");
                }

                break;
            case (PktType.Interact):
                InteractionPkt Interaction = new InteractionPkt();
                Interaction.Deserialize(msg.reader);
                if (Interaction.OwnerSyncBaseID == Player.Character.ID) {
                    break;
                }

                SyncBase Target = Spawner.SyncDB.Get(Interaction.InteractSyncBaseID);
                if (Target != null && Target is Interactable) {
                    ((Interactable) Target).Interact(
                        Spawner.SyncDB.Get(Interaction.OwnerSyncBaseID),
                        Interaction.InteractionType);
                } else {
                    Term.Println("Server has sent an erroneus SyncBase ID!");
                }
                break;
            case (PktType.Sync):
                SyncHandler.HandleSyncPkt(msg);
                break;
            case (PktType.StaticObjectIds):
                IntListPkt StaticIds = new IntListPkt();
                StaticIds.Deserialize(msg.reader);
                Spawner.SyncDB.SetStaticObjectsIDs(StaticIds.IdList);
                break;
            case (PktType.Disconnect):
                DisconnectPkt Disconnect = new DisconnectPkt();
                Disconnect.Deserialize(msg.reader);

                Term.Println(Disconnect.ConnectionID + " disconnected!");
                Spawner.Remove(Players[Disconnect.ConnectionID].Character.gameObject);
                Players.Remove(Disconnect.ConnectionID);
                break;
            case (PktType.InventoryAction):
                InventoryActionPkt InventoryActionPkt = new InventoryActionPkt();
                InventoryActionPkt.Deserialize(msg.reader);

                SyncBase CurrSyncBase = Spawner.SyncDB.Get(InventoryActionPkt.SyncBaseID);
                Inventory Inventory;
                if (CurrSyncBase is Inventory) {
                    Inventory = (Inventory) CurrSyncBase;
                } else {
                    break;
                }

                switch (InventoryActionPkt.Action) {
                case InventoryAction.Equip:
                    Item Item = ItemDB.Singleton.Get(InventoryActionPkt.RelatedInt);
                    Inventory.Equipped.SetSlot(Item.Slot, Item);
                    break;
                case InventoryAction.Unequip:
                    EquipSlot Slot = (EquipSlot) InventoryActionPkt.RelatedInt;
                    Inventory.Equipped.ClearSlot(Slot);
                    break;
                case InventoryAction.Use:
                    EquipSlot UseSlot = (EquipSlot) InventoryActionPkt.RelatedInt;
                    Item UseItem = Inventory.Equipped.GetItem(UseSlot);
                    Character Character = CurrSyncBase.GetComponent<Character>();
                    if (UseItem != null && UseItem.Action != null && Character != null &&
                            Player.Character != Character) {
                        // Item exists, it has an action, and the character 
                        // isn't controlled by the client (no double-actions).
                        UseItem.Action(Character);
                    }
                    break;
                }
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
                NetClient.Send(PktType.TextMessage, new TextMessagePkt("A Client: " + args[0]));
            });

            Term.AddCommand("disconnect", "Disconnect from the server.", (args) => {
                NetClient.Send(PktType.Disconnect, new DisconnectPkt());
            });
        }

        private void OnDisconnected(NetworkMessage msg) {
            Debug.Log("Disconnected!");
            Term.Println("Disconnected!");

            foreach (var Entry in Players) {
                Spawner.Remove(Entry.Value.Character.gameObject);
            }
            Players.Clear();
            Running = false;
        }

        private void OnError(NetworkMessage msg) {
            Debug.LogError("Encountered a network error. Shutting down.");
            Term.Println("Encountered a network error. Shutting down.");
            NetClient.Disconnect();
            Running = false;
        }
    }
}
