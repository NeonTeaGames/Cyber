
using Cyber.Console;
using Cyber.Items;
using Cyber.Networking;
using Cyber.Networking.Serverside;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cyber.Entities.SyncBases {

    /// <summary>
    /// The Inventory component, used for managing the inventory of a <see cref="Character"/>.
    /// </summary>
    public class Inventory : SyncBase {

        /// <summary>
        /// Refrence of the actual <see cref="Drive"/>.
        /// </summary>
        public Drive Drive;

        /// <summary>
        /// The possible <see cref="Character"/> component associated with this Inventory. Used in <see cref="UseItemInSlot(EquipSlot)"/>.
        /// </summary>
        public Character Character;

        /// <summary>
        /// The <see cref="InventoryActionHandler"/> which handles inventory actions properly.
        /// </summary>
        public InventoryActionHandler ActionHandler;

        /// <summary>
        /// Creates the Inventory-component for a game object.
        /// </summary>
        public Inventory() {
            Drive = new Drive(10f);
            if (Server.IsRunning()) {
                Drive.AddItem(ItemDB.Singleton.Get(0));
                Drive.AddItem(ItemDB.Singleton.Get(1));
                Drive.AddItem(ItemDB.Singleton.Get(2));
            }
        }

        /// <summary>
        /// Generates a checksum for the inventory.
        /// </summary>
        /// <returns>A checksum of the IDs of the items</returns>
        public override int GenerateChecksum() {
            var Slots = Drive.GetSlots();
            int Checksum = 0;
            for (int i = 0; i < Slots.Length; i++) {
                // Times with primes and sprinkle some i to spice up the stew
                Checksum += (((Slots[i].Item != null) ? Slots[i].Item.ID : i) + 1) * 509 * (i + 1) * 53 + (Slots[i].Equipped ? 1789 : 431);
            }
            return Checksum;
        }

        /// <summary>
        /// Returns the sync handletype indicating how the inventory should be synced.
        /// </summary>
        /// <returns></returns>
        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(true, 10);
        }

        /// <summary>
        /// Uses the item in the left hand if something is equipped.
        /// </summary>
        public void UseItemInSlot(EquipSlot slot) {
            Item Item = Drive.GetSlot(slot);
            if (Item != null && Item.Action != null && Character != null) {
                Item.Action(Character);
            }
        }

        /// <summary>
        /// Deserializes the ID's and creates them in the <see cref="Drive"/>.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {

            byte[][] ByteArray = new byte[4][];
            ByteArray[0] = reader.ReadBytesAndSize();
            ByteArray[1] = reader.ReadBytesAndSize();
            ByteArray[2] = reader.ReadBytesAndSize();
            ByteArray[3] = reader.ReadBytesAndSize();
            int[] IDs = NetworkHelper.DeserializeIntArray(ByteArray);

            byte[] Equippeds = reader.ReadBytesAndSize();

            Drive.Clear();
            for (int i = 0; i < IDs.Length; i++) {
                int ID = IDs[i];
                if (ID >= 0) {
                    Drive.AddItemToIndex(ItemDB.Singleton.Get(ID), i);
                    Drive.GetSlots()[i].Equipped = (Equippeds[i] == 1 ? true : false);
                }
            }
        }

        /// <summary>
        /// Serializes only the <see cref="Drive"/>'s item IDs.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            Slot[] Slots = Drive.GetSlots();

            int[] IDs = new int[Slots.Length];
            byte[] Equippeds = new byte[Slots.Length];
            for (int i = 0; i < Slots.Length; i++) {
                if (Slots[i].Item == null) {
                    IDs[i] = -1;
                } else {
                    IDs[i] = Slots[i].Item.ID;
                }
                Equippeds[i] = (byte) ((Slots[i].Equipped) ? 1 : 0);
            }
            
            byte[][] ByteArray = NetworkHelper.SerializeIntArray(IDs);

            writer.WriteBytesFull(ByteArray[0]);
            writer.WriteBytesFull(ByteArray[1]);
            writer.WriteBytesFull(ByteArray[2]);
            writer.WriteBytesFull(ByteArray[3]);

            writer.WriteBytesFull(Equippeds);
        }

        private void Start() {
            Character = GetComponent<Character>();
            ActionHandler = new InventoryActionHandler(this, Character);
        }
    }
}
