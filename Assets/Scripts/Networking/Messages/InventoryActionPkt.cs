
using Cyber.Items;
using UnityEngine.Networking;

namespace Cyber.Networking.Messages {

    /// <summary>
    /// Packet containing an inventory action of a kind.
    /// </summary>
    public class InventoryActionPkt : MessageBase {

        /// <summary>
        /// The inventory action.
        /// </summary>
        public InventoryAction Action;

        /// <summary>
        /// The related int to the <see cref="InventoryAction"/>
        /// </summary>
        public int RelatedInt;

        /// <summary>
        /// The inventory SyncBaseID this happened in. Only set by server.
        /// </summary>
        public int SyncBaseID;

        /// <summary>
        /// Creates an inventory action packet for sending.
        /// </summary>
        /// <param name="action">The action done.</param>
        /// <param name="relatedInt"></param>
        public InventoryActionPkt(InventoryAction action, int relatedInt) {
            Action = action;
            RelatedInt = relatedInt;
        }

        /// <summary>
        /// Creates an inventory action packet for deserializing.
        /// </summary>
        public InventoryActionPkt() {}

        /// <summary>
        /// Deserializes the <see cref="InventoryActionPkt"/>
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            Action = (InventoryAction) reader.ReadByte();
            RelatedInt = reader.ReadInt32();
            SyncBaseID = reader.ReadInt32();
        }
        
        /// <summary>
        /// Serializes the <see cref="InventoryActionPkt"/>
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write((byte) Action);
            writer.Write(RelatedInt);
            writer.Write(SyncBaseID);
        }
    }
}
