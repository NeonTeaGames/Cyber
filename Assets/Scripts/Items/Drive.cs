
using System.Collections.Generic;

namespace Cyber.Items {

    /// <summary>
    /// A drive containing items, and has a limited capacity that cannot be changed.
    /// </summary>
    public class Drive {

        private List<Item> Items = new List<Item>();

        /// <summary>
        /// The capacity of the drive, meaning how much stuff can this drive contain.
        /// </summary>
        public readonly float Capacity;

        /// <summary>
        /// The interface-class of this Drive.
        /// </summary>
        public DriveInterface Interface;

        /// <summary>
        /// Creates a drive with given capacity. Capacity cannot be changed after this.
        /// </summary>
        /// <param name="capacity">Capacity of the drive</param>
        public Drive(float capacity) {
            Capacity = capacity;
            Interface = new DriveInterface(this);
        }

        /// <summary>
        /// Current total weight of all the items combined.
        /// </summary>
        /// <returns>The totam weight</returns>
        public float TotalWeight() {
            float sum = 0;
            foreach (Item item in Items) {
                sum += item.Weight;
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
            Items.Clear();
        }

        /// <summary>
        /// Adds the item in the drive. If the addition was not successful, returns false, true otherwise.
        /// </summary>
        /// <returns>Weather the drive had enough space.</returns>
        public bool AddItem(Item item) {
            if (item.Weight > FreeSpace()) {
                return false;
            }
            Interface.AddNewItem(Items.Count);
            Items.Add(item);
            return true;
        }

        /// <summary>
        /// Gets the item at the given index, or null if there is nothing.
        /// </summary>
        /// <param name="idx">The index of the desired item</param>
        /// <returns>The item or null if nothing was found.</returns>
        public Item GetItem(int idx) {
            if (idx < 0 || idx >= Items.Count) {
                return null;
            }
            return Items[idx];
        }

        /// <summary>
        /// Returns the list of all items currently in the drive.
        /// </summary>
        /// <returns></returns>
        public List<Item> GetItems() {
            return Items;
        }

    }
}
