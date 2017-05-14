
namespace Cyber.Items {

    /// <summary>
    /// Represents an inventory action
    /// </summary>
    public enum InventoryAction : byte {

        /// <summary>
        /// Equip an item, given int is the ID of the item equipping
        /// </summary>
        Equip,

        /// <summary>
        /// Unequip the given slot, removing anything that was inside of it.
        /// </summary>
        Unequip
    }
}
