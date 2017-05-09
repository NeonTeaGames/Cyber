using Cyber.Console;
using Cyber.Entities;
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
        /// <param name="msgType"></param>
        /// <param name="message"></param>
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
        /// Returns if the client is running or not. 
        /// This is independant weather the client is connected or not.
        /// </summary>
        /// <returns>Weather the client is running or not</returns>
        public static bool IsRunning() {
            return Singleton.Running;
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
            NetClient.RegisterHandler(PktType.SyncPacket, HandlePacket);

            NetClient.RegisterHandler(MsgType.Connect, OnConnected);
            NetClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetClient.RegisterHandler(MsgType.Error, OnError);

            NetClient.Connect(ip, port);

            Debug.Log("Client launched!");
            Term.Println("Client launched!");

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
                MassIdentityPkt Identities = new MassIdentityPkt();
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
            case (PktType.SyncPacket):
                SyncHandler.HandleSyncPkt(msg);
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
    }
}
