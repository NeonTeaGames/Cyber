
using System.Collections.Generic;

namespace Cyber.Items {

    /// <summary>
    /// Represents the equipped items at given slots.
    /// </summary>
    public class Equipped {

        Dictionary<EquipSlot, Item> EquippedItems = new Dictionary<EquipSlot, Item>();

        /// <summary>
        /// Inserts an item here, marking it as 'equipped'.
        /// </summary>
        /// <param name="slot">The slot to equip the item to.</param>
        /// <param name="item">The item to equip.</param>
        public void SetSlot(EquipSlot slot, Item item) {
            EquippedItems[slot] = item;
        }

        /// <summary>
        /// Empties the desired slot of any items.
        /// </summary>
        /// <param name="slot">The slot to empty.</param>
        public void ClearSlot(EquipSlot slot) {
            EquippedItems.Remove(slot);
        }

        /// <summary>
        /// Returns the item at the given slot, or null if no item at the slot was found.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public Item GetItem(EquipSlot slot) {
            if (EquippedItems.ContainsKey(slot)) {
                return EquippedItems[slot];
            }
            return null;
        }

        /// <summary>
        /// Returns a dictionary of all equipped items.
        /// </summary>
        /// <returns>Dictionary of equipped items.</returns>
        public Dictionary<EquipSlot, Item> GetEquippedDict() {
            return EquippedItems;
        }

        /// <summary>
        /// Returns a list of all items that are generally equipped.
        /// </summary>
        /// <returns>List of equipped items.</returns>
        public List<Item> GetEquippedList() {
            return new List<Item>(EquippedItems.Values);
        }

        /// <summary>
        /// Clears all equipped items, removing them from their slots.
        /// </summary>
        public void ClearAllEquipped() {
            EquippedItems.Clear();
        }

    }
}
