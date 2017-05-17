
using System.Collections.Generic;

namespace Cyber.Items {

    /// <summary>
    /// A drive containing items, and has a limited capacity that cannot be changed.
    /// </summary>
    public class Drive {

        private Slot[] Slots = new Slot[0];

        /// <summary>
        /// The capacity of the drive, meaning how much stuff can this drive contain.
        /// </summary>
        public readonly float Capacity;

        /// <summary>
        /// Creates a drive with given capacity. Capacity cannot be changed after this.
        /// </summary>
        /// <param name="capacity">Capacity of the drive</param>
        public Drive(float capacity) {
            Capacity = capacity;
        }

        /// <summary>
        /// Current total weight of all the items combined.
        /// </summary>
        /// <returns>The totam weight</returns>
        public float TotalWeight() {
            float sum = 0;
            foreach (Slot slot in Slots) {
                if (slot.Item != null) {
                    sum += slot.Item.Weight;
                }
            }
            return sum;
        }

        /// <summary>
        /// Current free space in the drive. Literally is Capacity - TotalWeight()
        /// </summary>
        /// <returns>Current Free space.</returns>
        public float FreeSpace() {
            return Capacity - TotalWeight();
        }

        /// <summary>
        /// Clears the drive, completely wiping it empty. Mainly used for 
        /// <see cref="Inventory.Deserialize(NetworkReader)"/>
        /// </summary>
        public void Clear() {
            Slots = new Slot[0];
        }

        /// <summary>
        /// Adds the item in the drive. If the addition was not successful, returns false, true otherwise.
        /// </summary>
        /// <returns>Weather the drive had enough space.</returns>
        public bool AddItem(Item item) {
            if (item.Weight > FreeSpace()) {
                return false;
            }
            bool foundEmpty = false;
            for (int i = 0; i < Slots.Length; i++) {
                if (Slots[i].Item == null) {
                    Slots[i] = new Slot(item, false);
                    foundEmpty = true;
                }
            }
            if (!foundEmpty) {
                // Add space and set the item there
                IncreaseCapacity(1);
                Slots[Slots.Length - 1] = new Slot(item, false);
            }
            return true;
        }

        /// <summary>
        /// Tries to add an item to a specific slot. Returns false if slot already occupied.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="idx">Index in the inventory.</param>
        /// <returns>Weather the slot was empty or not.</returns>
        public bool AddItemToIndex(Item item, int idx) {
            if (idx < 0) {
                return false;
            }
            if (GetItemAt(idx) == null) {
                if (Slots.Length > idx) {
                    Slots[idx].Item = item;
                } else {
                    IncreaseCapacity(idx - Slots.Length + 1);
                    Slots[idx].Item = item;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Switches two slots at given indices, allowing moving items around the inventory.
        /// </summary>
        /// <param name="idx1">The first index.</param>
        /// <param name="idx2">The second index.</param>
        public void SwitchSlots(int idx1, int idx2) {
            if (idx1 == idx2) {
                return;
            }
            Slot Slot1 = GetSlotAt(idx1);
            Slot Slot2 = GetSlotAt(idx2);
            SetSlotAt(idx1, Slot2);
            SetSlotAt(idx2, Slot1);
        }

        private void SetSlotAt(int idx, Slot slot) {
            if (idx >= Slots.Length) {
                IncreaseCapacity(idx - Slots.Length + 1);
            }
            Slots[idx] = slot;
        }

        /// <summary>
        /// Gets the item at the given index, or null if there is nothing.
        /// </summary>
        /// <param name="idx">The index of the desired item</param>
        /// <returns>The item or null if nothing was found.</returns>
        public Item GetItemAt(int idx) {
            return GetSlotAt(idx).Item;
        }

        /// <summary>
        /// Returns the list of all items currently in the drive.
        /// </summary>
        /// <returns></returns>
        public List<Item> GetItems() {
            List<Item> Items = new List<Item>();
            foreach (Slot slot in Slots) {
                if (slot.Item != null) {
                    Items.Add(slot.Item);
                }
            }
            return Items;
        }

        /// <summary>
        /// Gets the item at the given slot. Loops through all items and checks if there is an item equipped at the given slot and returns it if there is.
        /// </summary>
        /// <param name="slot">The desired slot</param>
        /// <returns>The item at the slot, or null if no items</returns>
        public Item GetSlot(EquipSlot slot) {
            Item Item = null;
            foreach (Slot s in Slots) {
                if (s.Item != null && s.Item.Slot == slot && s.Equipped) {
                    Item = s.Item;
                    break;
                }
            }
            return Item;
        }

        /// <summary>
        /// Returns all the equipped items in a dictionary.
        /// </summary>
        /// <returns>The dictionary of items.</returns>
        public Dictionary<EquipSlot, Item> GetEquippedItems() {
            Dictionary<EquipSlot, Item> Items = new Dictionary<EquipSlot, Item>();
            foreach (Slot s in Slots) {
                if (s.Item != null && s.Equipped) {
                    Items.Add(s.Item.Slot, s.Item);
                }
            }
            return Items;
        }

        /// <summary>
        /// Attempt to equip the item at the given index.
        /// </summary>
        /// <param name="idx">Equip the iten at the given slot</param>
        /// <returns>Weather the item could be equipped or if there already exists something on that equip slot.</returns>
        public bool EquipItem(int idx) {
            Slot Slot = GetSlotAt(idx);
            if (Slot.Item != null && GetSlot(Slot.Item.Slot) == null) {
                Slots[idx].Equipped = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Unequips the item at the given inventory index.
        /// </summary>
        /// <param name="idx">The index to unequip.</param>
        public void UnequipItem(int idx) {
            Slot Slot = GetSlotAt(idx);
            if (Slot.Item != null) {
                Slots[idx].Equipped = false;
            }
        }

        /// <summary>
        /// Unequip all items that are in this slot.
        /// </summary>
        /// <param name="equipSlot">The slot of the desired item to be unequipped.</param>
        public void UnequipSlot(EquipSlot equipSlot) {
            for (int i = 0; i < Slots.Length; i++) {
                if (Slots[i].Item != null && Slots[i].Item.Slot == equipSlot) {
                    Slots[i].Equipped = false;
                }
            }
        }

        /// <summary>
        /// Simply returns the slots-array. The fastest way to get all of inventory if needed.
        /// </summary>
        /// <returns>The Slot-structs</returns>
        public Slot[] GetSlots() {
            return Slots;
        }

        private void IncreaseCapacity(int moreCapacity) {
            Slot[] NewSlots = new Slot[Slots.Length + moreCapacity];
            for (int i = 0; i < Slots.Length; i++) {
                NewSlots[i] = Slots[i];
            }
            Slots = NewSlots;
        }

        private Slot GetSlotAt(int idx) {
            if (idx < 0 || idx >= Slots.Length) {
                return new Slot(null, false);
            }
            return Slots[idx];
        }

    }
}
