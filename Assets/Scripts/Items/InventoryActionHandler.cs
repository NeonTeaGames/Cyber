
using Cyber.Entities.SyncBases;
using Cyber.Networking.Clientside;
using Cyber.Networking.Messages;
using UnityEngine;

namespace Cyber.Items {

    public class InventoryActionHandler {

        private Inventory Inventory;
        private Character Character;

        public InventoryActionHandler(Inventory inventory, Character character) {
            Inventory = inventory;
            Character = character;
        }

        public InventoryActionPkt BuildUseItem(EquipSlot slot) {
            return new InventoryActionPkt(InventoryAction.Use, (int) slot);
        }

        public InventoryActionPkt BuildClearSlot(EquipSlot slot) {
            return new InventoryActionPkt(InventoryAction.Unequip, (int) slot);
        }

        public InventoryActionPkt BuildEquipItem(int itemID) {
            return new InventoryActionPkt(InventoryAction.Equip, itemID);
        }

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
