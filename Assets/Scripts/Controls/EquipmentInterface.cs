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
            public MeshFilter Mesh;
        }

        /// <summary>
        /// The inventory.
        /// </summary>
        public Inventory Inventory;

        /// <summary>
        /// How many times per second the visual equipment should be updated.
        /// </summary>
        public float UpdateFrequency = 1f;

        /// <summary>
        /// The slots where the equipped items go.
        /// </summary>
        public EquipmentMesh[] Slots;

        private float LastUpdateTime = 0;

        private void Update() {
            if (Time.time - LastUpdateTime >= 1f / UpdateFrequency) {
                Dictionary<EquipSlot, Item> Equips = Inventory.Equipped.GetEquippedDict();
                // Empty all slots
                for (int i = 0; i < Slots.Length; i++) {
                    Slots[i].Mesh.mesh = null;
                }
                // Equip all slots
                foreach (EquipSlot Slot in Equips.Keys) {
                    for (int i = 0; i < Slots.Length; i++) {
                        if (Slots[i].Slot == Slot) {
                            Slots[i].Mesh.mesh = MeshDB.GetMesh(Equips[Slots[i].Slot].ModelID);
                            break;
                        }
                    }
                }
                LastUpdateTime = Time.time;
            }
        }
    }
}