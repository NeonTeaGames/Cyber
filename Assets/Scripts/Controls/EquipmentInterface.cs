using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Entities.SyncBases;
using Cyber.Util;
using Cyber.Items;

namespace Cyber.Controls {

    /// <summary>
    /// Interface that controls the visible equipment on the characters.
    /// </summary>
    public class EquipmentInterface : MonoBehaviour {

        /// <summary>
        /// Equipment mesh.
        /// </summary>
        [System.Serializable]
        public struct EquipmentMesh {
        
            /// <summary>
            /// The slot enum of the item.
            /// </summary>
            public EquipSlot Slot;

            /// <summary>
            /// The in-world slot of the item.
            /// </summary>
            public Transform Transform;
        }

        /// <summary>
        /// The inventory.
        /// </summary>
        public Inventory Inventory;

        /// <summary>
        /// The slots where the equipped items go.
        /// </summary>
        public EquipmentMesh[] Slots;

        private Dictionary<EquipSlot, Item> LastEquips = new Dictionary<EquipSlot, Item>();

        private void Update() {
            Dictionary<EquipSlot, Item> Equips = Inventory.Drive.GetEquippedItems();
            for (int i = 0; i < Slots.Length; i++) {
                bool Equipped = Equips.ContainsKey(Slots[i].Slot);
                bool LastEquipped = LastEquips.ContainsKey(Slots[i].Slot);
                // Check if this slot needs to be changed
                if (Equipped != LastEquipped || (Equipped && Equips[Slots[i].Slot] != LastEquips[Slots[i].Slot])) {
                    // Clear slot
                    for (int j = 0; j < Slots[i].Transform.childCount; j++) {
                        Destroy(Slots[i].Transform.GetChild(j).gameObject);
                    }
                    if (!Equipped) {
                        // Nothing is equipped
                        LastEquips.Remove(Slots[i].Slot);
                    } else {
                        // Something is equipped
                        Item Item = Equips[Slots[i].Slot];
                        PrefabDB.Create(Item.ModelID, Slots[i].Transform);
                        LastEquips[Slots[i].Slot] = Item;
                    }
                }
            }
        }
    }
}