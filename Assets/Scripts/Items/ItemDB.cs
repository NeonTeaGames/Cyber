using System.Collections.Generic;

namespace Cyber.Items {

    /// <summary>
    /// ItemDB containing 'templates' for all items.
    /// </summary>
    public class ItemDB {

        private Dictionary<int, Item> Items = new Dictionary<int, Item>();
        private int Counter = 0;

        /// <summary>
        /// 
        /// </summary>
        public static ItemDB Singleton = new ItemDB();

        /// <summary>
        /// Creates the ItemDB. Should not be externally called. See <see cref="Singleton"/>.
        /// </summary>
        public ItemDB() {
            AddItem(new Item(Counter++, 0, "Very Long Item Name", 1.5f, EquipSlot.Hat, "This item is a rare piece of the \"way too long of a name\" technology, invented by space goblins in ancient times."));
            AddItem(new Item(Counter++, 1, "Outworldly spherical tube", .5f, EquipSlot.RightHand, "It's so spherical and smooth that it seems like it's not even from this world!"));
        }

        /// <summary>
        /// If there is an item at the given ID, return a clone of it. Otherwise return null.
        /// </summary>
        /// <param name="itemId">The id of the desired item.</param>
        /// <returns></returns>
        public Item Get(int itemId) {
            if (Items.ContainsKey(itemId)) {
                return Items[itemId].Clone();
            }
            return null;
        }

        private void AddItem(Item item) {
            Items.Add(item.ID, item);
        }

    }
}
