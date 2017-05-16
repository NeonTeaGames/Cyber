
using Cyber.Entities.SyncBases;
using Cyber.Networking.Clientside;
using Cyber.Networking.Messages;

namespace Cyber.Items {

    /// <summary>
    /// Handles InventoryActions, building them on the client when needed and running them.
    /// </summary>
    public class InventoryActionHandler {

        private Inventory Inventory;
        private Character Character;

        /// <summary>
        /// Creates an <see cref="InventoryActionHandler"/>
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="character"></param>
        public InventoryActionHandler(Inventory inventory, Character character) {
            Inventory = inventory;
            Character = character;
        }

        /// <summary>
        /// Builds a <see cref="InventoryAction.Use"/> packet.
        /// </summary>
        /// <param name="slot">The equipped slot to use.</param>
        /// <returns></returns>
        public InventoryActionPkt BuildUseItem(EquipSlot slot) {
            return new InventoryActionPkt(InventoryAction.Use, (int) slot);
        }

        /// <summary>
        /// Builds a <see cref="InventoryAction.Unequip"/> packet.
        /// </summary>
        /// <param name="slot">The equipped slot to clear.</param>
        /// <returns></returns>
        public InventoryActionPkt BuildClearSlot(EquipSlot slot) {
            return new InventoryActionPkt(InventoryAction.Unequip, (int) slot);
        }

        /// <summary>
        /// Builds a <see cref="InventoryAction.Equip"/> packet.
        /// </summary>
        /// <param name="itemID">The item ID to equip.</param>
        /// <returns></returns>
        public InventoryActionPkt BuildEquipItem(int itemID) {
            return new InventoryActionPkt(InventoryAction.Equip, itemID);
        }

        /// <summary>
        /// Handles an <see cref="InventoryActionPkt"/> to handle. Ran on server and client.
        /// </summary>
        /// <param name="action">The <see cref="InventoryAction"/> to run</param>
        /// <param name="relatedInt">The item related to the action.</param>
        /// <returns>Weather the action failed or not.</returns>
        public bool HandleAction(InventoryAction action, int relatedInt) {
            switch (action) {
            case InventoryAction.Equip:
                Item Item = ItemDB.Singleton.Get(relatedInt);
                Inventory.Equipped.SetSlot(Item.Slot, Item);
                return true;
            case InventoryAction.Unequip:
                EquipSlot Slot = (EquipSlot) relatedInt;
                Inventory.Equipped.ClearSlot(Slot);
                return true;
            case InventoryAction.Use:
                EquipSlot UseSlot = (EquipSlot) relatedInt;
                Item UseItem = Inventory.Equipped.GetItem(UseSlot);
                if (UseItem != null && UseItem.Action != null && Character != null &&
                        (!Client.IsRunning() || Client.GetConnectedPlayer().Character != Character)) {
                    // Item exists, it has an action, and the character 
                    // isn't controlled by the client (no double-actions).
                    UseItem.Action(Character);
                    return true;
                }
                return false;
            }
            return false;
        }

    }
}
