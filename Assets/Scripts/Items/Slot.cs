
namespace Cyber.Items {

    /// <summary>
    /// Represents a slot which contains an item and a bool weather the item is equipped or not.
    /// </summary>
    public struct Slot {

        /// <summary>
        /// The item that this slot holds. If this is null, the slot is empty.
        /// </summary>
        public Item Item;

        /// <summary>
        /// Weather this item is equipped or not.
        /// </summary>
        public bool Equipped;

        /// <summary>
        /// Creates a slot.
        /// </summary>
        /// <param name="item">Item in the slot, or null if the slot is empty.</param>
        /// <param name="equipped">Weather this slot is equipped or not.</param>
        public Slot(Item item, bool equipped) {
            Item = item;
            Equipped = equipped;
        }

    }
}
