using Cyber.Entities.SyncBases;

namespace Cyber.Items {
    
    /// <summary>
    /// An item, containing itemmy information.
    /// </summary>
    public class Item {
        public delegate void ItemAction(Character host);

        /// <summary>
        /// ID of the item, used in <see cref="ItemDB"/>.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// Model ID of this item.
        /// </summary>
        public int ModelID;

        /// <summary>
        /// Name of the item.
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the item.
        /// </summary>
        public string Description;

        /// <summary>
        /// The weight of the item (in kg).
        /// </summary>
        public float Weight;

        /// <summary>
        /// The slot in which the item can be equipped to.
        /// </summary>
        public EquipSlot Slot;

        /// <summary>
        /// The function that is ran when the item is used.
        /// </summary>
        public ItemAction Action;

        /// <summary>
        /// Creates an item. This should technically be only called by ItemDB, but it's public because of "reasons".
        /// </summary>
        /// <param name="id">ID of the item.</param>
        /// <param name="modelId">ModelID of the item, see ModelDB.</param>
        /// <param name="name">The name if the item.</param>
        /// <param name="weight">The Weight of the item.</param>
        /// <param name="slot">The equip slot of the item.</param>
        /// <param name="description">The description of the item.</param>
        public Item(int id, int modelId, string name, float weight, EquipSlot slot, string description, ItemAction action = null) {
            ID = id;
            ModelID = modelId;
            Name = name;
            Weight = weight;
            Slot = slot;
            Description = description;
            Action = action;
        }
        
        /// <summary>
        /// Clones the item, used mostly for <see cref="ItemDB.Get(int)"/>.
        /// </summary>
        /// <returns></returns>
        public Item Clone() {
            return new Item(ID, ModelID, Name, Weight, Slot, Description, Action);
        }
    }
}
