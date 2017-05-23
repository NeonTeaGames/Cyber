using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Util;
using Cyber.Console;
using Cyber.Entities.SyncBases;
using Cyber.Items;
using Cyber.Networking.Clientside;
using Cyber.Networking;
using Cyber.Networking.Messages;

namespace Cyber.Controls {

    /// <summary>
    /// Handles displaying and interacting with the inventory.
    /// </summary>
    public class InventoryInterface : MonoBehaviour {

        /// <summary>
        /// The inventory of the player, will be displayed here.
        /// </summary>
        public Inventory Inventory;

        /// <summary>
        /// The camera that is displaying this inventory interface.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The hologram that acts as the root for the inventory.
        /// </summary>
        public Hologram Hologram;

        /// <summary>
        /// The text texture that shows the selected item's name.
        /// </summary>
        public TextTextureApplier ItemNameText;

        /// <summary>
        /// The text texture that shows the selected item's description.
        /// </summary>
        public TextTextureApplier ItemDescriptionText;

        /// <summary>
        /// The item preview parent.
        /// </summary>
        public Transform ItemPreview;

        /// <summary>
        /// The item preview spinner.
        /// </summary>
        public Spinner ItemPreviewSpinner;

        /// <summary>
        /// The icon for the inventory.
        /// </summary>
        public MeshRenderer IconInventory;

        /// <summary>
        /// The icon for the status.
        /// </summary>
        public MeshRenderer IconStatus;

        /// <summary>
        /// The icon for the social.
        /// </summary>
        public MeshRenderer IconSocial;

        /// <summary>
        /// The icon for the map.
        /// </summary>
        public MeshRenderer IconMap;

        /// <summary>
        /// The text that describes the selected icon.
        /// </summary>
        public TextTextureApplier IconExplainerText;

        /// <summary>
        /// The selector mesh. It'll move to the position of the selected item
        /// in the grid.
        /// </summary>
        public Transform ItemGridSelector;

        /// <summary>
        /// The parent of all the gameobjects that create the grid.
        /// </summary>
        public Transform ItemGridParent;

        /// <summary>
        /// The item grid dimensions.
        /// </summary>
        public Vector2 ItemGridDimensions;

        /// <summary>
        /// The material of the items in the item grid.
        /// </summary>
        public Material ItemGridHologramMaterial;

        /// <summary>
        /// The material of the item in the item preview.
        /// </summary>
        public Material ItemPreviewHologramMaterial;

        private bool InventoryOpen = false;
        private float PreviewVisibility = 0f;
        private int PreviewModelID = -1;

        private List<Transform> ItemGridCells;
        private Dictionary<int, Item> ItemGridContainedItems = new Dictionary<int, Item>();
        private int ItemGridSelectedIndex;
        private Transform GrabbedItem;
        private int GrabbedItemIndex = -1;

        private Color IconInventoryColor;
        private Color IconStatusColor;
        private Color IconSocialColor;
        private Color IconMapColor;

        /// <summary>
        /// Whether the inventory is currently open and interactible.
        /// </summary>
        /// <returns><c>true</c> if this instance is open; otherwise, <c>false</c>.</returns>
        public bool IsOpen() {
            return InventoryOpen;
        }

        private void Start() {
            int ItemGridSize = (int) ItemGridDimensions.x * (int) ItemGridDimensions.y;
            ItemGridCells = new List<Transform>(ItemGridSize);
            for (int i = 0; i < ItemGridSize; i++) {
                Transform Cell = ItemGridParent.GetChild(i).transform;
                ItemGridCells.Add(Cell);
            }

            IconInventoryColor = IconInventory.material.GetColor("_EmissionColor");
            IconStatusColor = IconStatus.material.GetColor("_EmissionColor");
            IconSocialColor = IconSocial.material.GetColor("_EmissionColor");
            IconMapColor = IconMap.material.GetColor("_EmissionColor");
        }

        private void Update() {
            if (Term.IsVisible()) {
                return;
            }

            if (Input.GetButtonDown("Inventory")) {
                InventoryOpen = !InventoryOpen;
                Hologram.Visible = InventoryOpen;
                CursorHandler.RequestLockState(!InventoryOpen);
            }

            int CurrentIndex = -1;
            RaycastHit LookedAt = CameraUtil.GetLookedAtHit(Camera, 1f, true);
            if (LookedAt.collider != null) {
                MeshRenderer Mesh = LookedAt.collider.GetComponent<MeshRenderer>();
                if (ItemGridCells.Contains(LookedAt.collider.transform)) {
                    // Interacting with the item list
                    CurrentIndex = int.Parse(LookedAt.collider.name.Split(' ')[1]);
                    if (GrabbedItem == null) {
                        // Nothing is currently being dragged, continue as normal
                        if (Input.GetButton("Activate") && 
                                (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && 
                                ItemGridSelectedIndex == CurrentIndex) {
                            // This item has been selected for at least a frame, 
                            // and the mouse is moving, this counts as dragging
                            GrabbedItem = LookedAt.collider.transform;
                            GrabbedItemIndex = CurrentIndex;
                        }
                        if (Input.GetButtonDown("Activate") || Input.GetButtonDown("Equip")) {
                            // Select things
                            ItemGridSelectedIndex = CurrentIndex;
                        }
                        if (Input.GetButtonDown("Equip")) {
                            // Equip things
                            Item SelectedItem = Inventory.Drive.GetItemAt(ItemGridSelectedIndex);
                            if (SelectedItem != null) {
                                Item Equipped = Inventory.Drive.GetSlot(SelectedItem.Slot);
                                if (Equipped != null && Equipped.ID == SelectedItem.ID) {
                                    Inventory.Drive.UnequipSlot(SelectedItem.Slot);
                                    Client.Send(PktType.InventoryAction, Inventory.ActionHandler.BuildClearSlot(SelectedItem.Slot));
                                } else {
                                    Inventory.Drive.EquipItem(ItemGridSelectedIndex);
                                    Client.Send(PktType.InventoryAction, Inventory.ActionHandler.BuildEquipItem(ItemGridSelectedIndex));
                                }
                            }
                        }
                    } else {
                        // Something is grabbed, make things react to this
                        ItemGridSelectedIndex = CurrentIndex;
                        if (Input.GetButtonUp("Activate")) {
                            // Grab was released, drop item here
                            if (GrabbedItemIndex == CurrentIndex) {
                                // The item was dropped in place, reset the position
                                if (ItemGridCells[GrabbedItemIndex].childCount > 0) {
                                    ItemGridCells[GrabbedItemIndex].GetChild(0).localPosition = new Vector3();
                                }
                            } else {
                                // Lerp things (if the models are set)
                                Vector3 ReturnToPos = ItemGridCells[GrabbedItemIndex].position;
                                ItemGridCells[GrabbedItemIndex].position = ItemGridCells[CurrentIndex].position;
                                Lerper.LerpTransformPosition(ItemGridCells[GrabbedItemIndex], ReturnToPos, 20f);

                                // Switch items
                                Inventory.Drive.SwitchSlots(GrabbedItemIndex, CurrentIndex);
                                Client.Send(PktType.InventoryAction, Inventory.ActionHandler.BuildSlotSwitch(GrabbedItemIndex, CurrentIndex));
                            }

                            // Reset grabbing
                            GrabbedItem = null;
                            GrabbedItemIndex = -1;
                        } else {
                            if (ItemGridCells[GrabbedItemIndex].childCount > 0) {
                                ItemGridCells[GrabbedItemIndex].GetChild(0).position = LookedAt.point;
                            }
                        }
                    }
                } else if (Mesh != null) {
                    float InvBrightness = 1.1f;
                    float StsBrightness = 1f;
                    float SclBrightness = 1f;
                    float MapBrightness = 1f;
                    string SelectedIcon = "";

                    if (Mesh == IconInventory) {
                        InvBrightness = 1.2f;
                        SelectedIcon = "Storage";
                    } else if (Mesh == IconStatus) {
                        StsBrightness = 1.2f;
                        SelectedIcon = "Status";
                    } else if (Mesh == IconSocial) {
                        SclBrightness = 1.2f;
                        SelectedIcon = "Social";
                    } else if (Mesh == IconMap) {
                        MapBrightness = 1.2f;
                        SelectedIcon = "Map";
                    }

                    TextTextureProperties Props = IconExplainerText.TextProperties;
                    Props.Text = SelectedIcon;
                    IconExplainerText.SetTextProperties(Props);


                    IconInventory.material.SetColor("_EmissionColor", new Color(IconInventoryColor.r * InvBrightness,
                        IconInventoryColor.g * InvBrightness, IconInventoryColor.b * InvBrightness));
                    IconStatus.material.SetColor("_EmissionColor", new Color(IconStatusColor.r * StsBrightness,
                        IconStatusColor.g * StsBrightness, IconStatusColor.b * StsBrightness));
                    IconSocial.material.SetColor("_EmissionColor", new Color(IconSocialColor.r * SclBrightness,
                        IconSocialColor.g * SclBrightness, IconSocialColor.b * SclBrightness));
                    IconMap.material.SetColor("_EmissionColor", new Color(IconMapColor.r * MapBrightness,
                        IconMapColor.g * MapBrightness, IconMapColor.b * MapBrightness));
                }
            } else {
                // Outside of the inventory, clicking will unselect
                if (Input.GetButtonDown("Activate")) {
                    ItemGridSelectedIndex = -1;
                }
            }
            RebuildItemGrid(CurrentIndex);
        }

        private void RebuildItemGrid(int focused) {
            bool ItemFound = false;
            for (int y = 0; y < ItemGridDimensions.y; y++) {
                for (int x = 0; x < ItemGridDimensions.x; x++) {
                    // Find the item and mesh
                    int i = x + y * (int) ItemGridDimensions.x;
                    Item Item = Inventory.Drive.GetItemAt(x + y * (int) ItemGridDimensions.x);
                    if (Item != null && (!ItemGridContainedItems.ContainsKey(i) || ItemGridContainedItems[i] != Item)) {
                        // Clear cell
                        for (int j = 0; j < ItemGridCells[i].childCount; j++) {
                            DestroyImmediate(ItemGridCells[i].GetChild(j).gameObject);
                        }

                        // Add new item
                        GameObject ItemObject = PrefabDB.Create(Item.ModelID, ItemGridCells[i]);
                        MeshUtil.SetMaterial(ItemObject, ItemGridHologramMaterial);
                        ItemGridContainedItems[i] = Item;
                    } else if (Item == null && ItemGridContainedItems.ContainsKey(i)) {
                        for (int j = 0; j < ItemGridCells[i].childCount; j++) {
                            Destroy(ItemGridCells[i].GetChild(j).gameObject);
                        }
                        ItemGridContainedItems.Remove(i);
                    }

                    // Set the base scale and spin status
                    float Scale = 0.13f;
                    bool Spinning = false;

                    if (focused == i || ItemGridSelectedIndex == i) {
                        // Item is selected or hovered, animate
                        Scale = 0.16f;
                        Spinning = true;
                    }

                    if (ItemGridSelectedIndex == i) {
                        // Set preview information
                        if (ItemGridCells[i].childCount > 0) {
                            SetPreviewItem(Item);
                        }
                        TextTextureProperties NameProps = ItemNameText.TextProperties;
                        TextTextureProperties DescriptionProps = ItemDescriptionText.TextProperties;
                        if (Item != null) {
                            NameProps.Text = Item.Name;
                            DescriptionProps.Text = Item.Description;
                            ItemFound = true;
                        } else {
                            NameProps.Text = "";
                            DescriptionProps.Text = "";
                        }
                        ItemNameText.SetTextProperties(NameProps);
                        ItemDescriptionText.SetTextProperties(DescriptionProps);

                        // Move selector
                        if ((ItemGridSelector.position - ItemGridCells[i].position).magnitude < 0.01f) {
                            ItemGridSelector.position = ItemGridCells[i].position;
                        } else {
                            ItemGridSelector.position =
                                Vector3.Lerp(ItemGridSelector.position,
                                    ItemGridCells[i].position, 10f * Time.deltaTime);
                        }
                        Vector3 NewRot = ItemGridSelector.localEulerAngles;
                        NewRot.z = 0;
                        ItemGridSelector.localEulerAngles = NewRot;
                    }

                    if (!Spinning) {
                        // Not selected, reset rotation
                        if (ItemGridCells[i].childCount > 0) {
                            Transform ItemTransform = ItemGridCells[i].GetChild(0);
                            ItemTransform.LookAt(Camera.transform);
                            Vector3 NewRot = ItemTransform.localEulerAngles;
                            NewRot.z = 0;
                            NewRot.y += 90;
                            ItemTransform.localEulerAngles = NewRot;
                        }
                    }

                    // Update spinning status and scaling
                    ItemGridCells[i].GetComponent<Spinner>().Spinning = Spinning;
                    if (ItemGridCells[i].childCount > 0) {
                        FixMeshScaling(ItemGridCells[i].GetChild(0), Scale);
                    }
                }
            }

            // Hide the selector if nothing is selected
            ItemGridSelector.gameObject.SetActive(ItemGridSelectedIndex >= 0);

            // Clean up the preview screen if there was no item
            if (!ItemFound) {
                SetPreviewItem(null);
                TextTextureProperties NameProps = ItemNameText.TextProperties;
                TextTextureProperties DescriptionProps = ItemDescriptionText.TextProperties;
                NameProps.Text = "";
                DescriptionProps.Text = "";
                ItemNameText.SetTextProperties(NameProps);
                ItemDescriptionText.SetTextProperties(DescriptionProps);
                PreviewVisibility = Mathf.Lerp(PreviewVisibility, 0f, 10f * Time.deltaTime);
            } else {
                PreviewVisibility = Mathf.Lerp(PreviewVisibility, 1f, 10f * Time.deltaTime);
            }
        }

        private void SetPreviewItem(Item item) {
            ItemPreviewSpinner.Spinning = item != null;
            if (item != null && PreviewModelID != item.ModelID) {
                for (int j = 0; j < ItemPreview.childCount; j++) {
                    DestroyImmediate(ItemPreview.GetChild(j).gameObject);
                }
                GameObject ItemObject = PrefabDB.Create(item.ModelID, ItemPreview);
                MeshUtil.SetMaterial(ItemObject, ItemPreviewHologramMaterial);
                PreviewModelID = item.ModelID;
            }
            FixMeshScaling(ItemPreview, 0.35f * PreviewVisibility);
        }

        private void FixMeshScaling(Transform toFix, float scale) {
            Vector3 Extents = MeshUtil.GetMeshBounds(toFix.gameObject).extents;
            float HighestExtent = 0.1f;

            if (Extents.x > HighestExtent) {
                HighestExtent = Extents.x;
            }
            if (Extents.y > HighestExtent) {
                HighestExtent = Extents.y;
            }
            if (Extents.z > HighestExtent) {
                HighestExtent = Extents.z;
            }

            float Scale = scale * 0.5f / HighestExtent;
            toFix.localScale = new Vector3(Scale, Scale, Scale);
        }
    }
}
