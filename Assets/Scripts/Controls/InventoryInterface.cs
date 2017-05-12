using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Util;

namespace Cyber.Controls {

    /// <summary>
    /// Handles displaying and interacting with the inventory.
    /// </summary>
    public class InventoryInterface : MonoBehaviour {

        /// <summary>
        /// The camera that is displaying this inventory interface.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The hologram that acts as the root for the inventory.
        /// </summary>
        public Hologram Hologram;

        /// <summary>
        /// The text that contains the item list.
        /// </summary>
        public TextTextureApplier ItemListText;

        /// <summary>
        /// How many items can be shown on the screen at the same time.
        /// </summary>
        public int ItemsPerScreen;

        private CursorHandler CursorHandler;
        private bool InventoryOpen = false;
        private int TestingInventorySize = 20;
        private int ScrollingIndex = 0;
        private int SelectedIndex = -1;

        private void Start() {
            CursorHandler = GameObject.Find("/Systems/CursorHandler").GetComponent<CursorHandler>();
            RebuildItemList(-1);
        }

        private void Update() {
            if (Input.GetButtonDown("Inventory")) {
                InventoryOpen = !InventoryOpen;
                Hologram.Visible = InventoryOpen;
                CursorHandler.RequestLockState(!InventoryOpen);
            }

            RaycastHit LookedAt = CameraUtil.GetLookedAtHit(Camera, 1f, true);
            if (LookedAt.collider != null) {
                TextTextureApplier Text = LookedAt.collider.GetComponent<TextTextureApplier>();
                if (Text != null && Text == ItemListText) {
                    // Interacting with the item list
                    // Calculate the index
                    float ScaledY = (Text.transform.InverseTransformPoint(LookedAt.point).z * 0.1f) + 0.5f;
                    int CurrentIndex = ScrollingIndex + (int)(ScaledY * ItemsPerScreen);

                    // Update inputs
                    if (Input.GetAxis("Mouse ScrollWheel") > 0 && ScrollingIndex > 0) {
                        ScrollingIndex--;
                    }
                    if (Input.GetAxis("Mouse ScrollWheel") < 0 && ScrollingIndex < TestingInventorySize - 1) {
                        ScrollingIndex++;
                    }
                    if (Input.GetButtonDown("Activate")) {
                        SelectedIndex = CurrentIndex;
                    }

                    // Rebuild the list
                    RebuildItemList(CurrentIndex);
                }
            } else {
                // Outside of the inventory, clicking will unselect
                if (Input.GetButtonDown("Activate")) {
                    SelectedIndex = -1;
                    RebuildItemList(-1);
                }
            }
        }

        private void RebuildItemList(int focused) {
            string Inv = "";
            for (int i = ScrollingIndex; i < TestingInventorySize; i++) {
                if (i == focused) {
                    Inv += "<b>";
                }
                if (i == SelectedIndex) {
                    Inv += "·";
                }
                Inv += "Item #" + i + "\n";
                if (i == focused) {
                    Inv += "</b>";
                }
            }
            TextTextureProperties NewProps = ItemListText.TextProperties;
            NewProps.Text = Inv;
            ItemListText.SetTextProperties(NewProps);
        }
    }
}