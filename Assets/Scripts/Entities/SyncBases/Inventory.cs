
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
        /// This entity's <see cref="Items.Equipped"/> <see cref="Item"/>s.
        /// </summary>
        public Equipped Equipped;

        /// <summary>
        /// Creates the Inventory-component for a game object.
        /// </summary>
        public Inventory() {
            Drive = new Drive(10f);
            Equipped = new Equipped();
            if (Server.IsRunning()) {
                Drive.AddItem(ItemDB.Singleton.Get(0));
                Drive.AddItem(ItemDB.Singleton.Get(1));
            }
        }

        /// <summary>
        /// Generates a checksum for the inventory
        /// </summary>
        /// <returns>A checksum of the IDs of the items</returns>
        public override int GenerateChecksum() {
            var Items = Drive.GetItems().ToArray();
            int Checksum = 0;
            for (int i = 0; i < Items.Length; i++) {
                Checksum ^= Items[i].ID;
            }
            var EquippedItems = Equipped.GetEquippedList().ToArray();
            for (int i = 0; i < EquippedItems.Length; i++) {
                Checksum ^= EquippedItems[i].ID;
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
        /// Deserializes the ID's and creates them in the <see cref="Drive"/>.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            Debug.Log("Deserializing inventory!");

            byte[][] ByteArray = new byte[4][];
            ByteArray[0] = reader.ReadBytesAndSize();
            ByteArray[1] = reader.ReadBytesAndSize();
            ByteArray[2] = reader.ReadBytesAndSize();
            ByteArray[3] = reader.ReadBytesAndSize();
            int[] IDs = NetworkHelper.DeserializeIntArray(ByteArray);

            Drive.Clear();
            foreach (int id in IDs) {
                Drive.AddItem(ItemDB.Singleton.Get(id));
            }

            bool ReceivedSlots = reader.ReadBoolean();
            if (!ReceivedSlots) {
                Equipped.ClearAllEquipped();
                return;
            }

            byte[] Slots = reader.ReadBytesAndSize();

            Debug.Log(Slots);

            byte[][] EquippedIdsBytes = new byte[4][];
            EquippedIdsBytes[0] = reader.ReadBytesAndSize();
            EquippedIdsBytes[1] = reader.ReadBytesAndSize();
            EquippedIdsBytes[2] = reader.ReadBytesAndSize();
            EquippedIdsBytes[3] = reader.ReadBytesAndSize();
            int[] EquippedIds = NetworkHelper.DeserializeIntArray(EquippedIdsBytes);

            Equipped.ClearAllEquipped();
            for (int i = 0; i < Slots.Length; i++) {
                Equipped.SetSlot((EquipSlot) Slots[i], ItemDB.Singleton.Get(EquippedIds[i]));
            }
        }

        /// <summary>
        /// Serializes only the <see cref="Drive"/>'s item IDs.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            var Items = Drive.GetItems();
            int[] IDs = new int[Items.Count];
            for (int i = 0; i < Items.Count; i++) {
                IDs[i] = Items[i].ID;
            }
            byte[][] ByteArray = NetworkHelper.SerializeIntArray(IDs);

            writer.WriteBytesFull(ByteArray[0]);
            writer.WriteBytesFull(ByteArray[1]);
            writer.WriteBytesFull(ByteArray[2]);
            writer.WriteBytesFull(ByteArray[3]);

            var slotList = new List<EquipSlot>(Equipped.GetEquippedDict().Keys).ConvertAll(x => (byte) x);

            if (slotList.Count > 0) {
                writer.Write(true);
            } else {
                writer.Write(false);
            }

            slotList.Sort((a, b) => {
                return b - a;
            });

            var idList = new List<int>();
            slotList.ForEach(x => {
                idList.Add(Equipped.GetItem((EquipSlot) x).ID);
            });

            writer.WriteBytesFull(slotList.ToArray());

            byte[][] EquippedByteArray = NetworkHelper.SerializeIntArray(idList.ToArray());
            writer.WriteBytesFull(EquippedByteArray[0]);
            writer.WriteBytesFull(EquippedByteArray[1]);
            writer.WriteBytesFull(EquippedByteArray[2]);
            writer.WriteBytesFull(EquippedByteArray[3]);
        }
    }
}
