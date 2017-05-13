
using Cyber.Items;
using Cyber.Networking;
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
        /// Creates the Inventory-component for a game object.
        /// </summary>
        public Inventory() {
            Drive = new Drive(10f);
            Drive.AddItem(ItemDB.Singleton.Get(0));
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
        }
    }
}
