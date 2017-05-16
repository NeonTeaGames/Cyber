
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
        /// <param name="itemIdx">The item index to equip.</param>
        /// <returns></returns>
        public InventoryActionPkt BuildEquipItem(int itemIdx) {
            return new InventoryActionPkt(InventoryAction.Equip, itemIdx);
        }

        public InventoryActionPkt BuildSlotSwitch(int switchFrom, int switchTo) {
            return new InventoryActionPkt(InventoryAction.Switch, new int[]{ switchFrom, switchTo });
        }

        /// <summary>
        /// Handles an <see cref="InventoryActionPkt"/> to handle. Ran on server and client.
        /// </summary>
        /// <param name="action">The <see cref="InventoryAction"/> to run</param>
        /// <param name="relatedInt">The item related to the action.</param>
        /// <returns>Weather the action failed or not.</returns>
        public bool HandleAction(InventoryAction action, int[] intList) {
            switch (action) {
            case InventoryAction.Equip:
                Inventory.Drive.EquipItem(intList[0]);
                return true;
            case InventoryAction.Unequip:
                EquipSlot Slot = (EquipSlot) intList[0];
                Inventory.Drive.UnequipSlot(Slot);
                return true;
            case InventoryAction.Use:
                EquipSlot UseSlot = (EquipSlot) intList[0];
                Item UseItem = Inventory.Drive.GetSlot(UseSlot);
                if (UseItem != null && UseItem.Action != null && Character != null &&
                        (!Client.IsRunning() || Client.GetConnectedPlayer().Character != Character)) {
                    // Item exists, it has an action, and the character 
                    // isn't controlled by the client (no double-actions).
                    UseItem.Action(Character);
                    return true;
                }
                return false;
            case InventoryAction.Switch:
                Inventory.Drive.SwitchSlots(intList[0], intList[1]);
                return true;
            }
            return false;
        }

    }
}
