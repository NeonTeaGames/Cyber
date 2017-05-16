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
        /// The item preview mesh.
        /// </summary>
        public MeshFilter ItemPreviewMesh;

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

        private bool InventoryOpen = false;
        private float PreviewVisibility = 0f;

        private List<Transform> ItemGridCells;
        private List<MeshFilter> ItemGridCellMeshes;
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
            ItemGridCellMeshes = new List<MeshFilter>(ItemGridSize);
            for (int i = 0; i < ItemGridSize; i++) {
                Transform Cell = ItemGridParent.GetChild(i).transform;
                ItemGridCells.Add(Cell);
                ItemGridCellMeshes.Add(Cell.GetComponentInChildren<MeshFilter>());
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
                        if (Input.GetButtonUp("Activate")) {
                            // Grab was released, drop item here
                            // Lerp things
                            ItemGridCellMeshes[GrabbedItemIndex].transform.position = ItemGridCells[CurrentIndex].position;
                            ItemGridCellMeshes[CurrentIndex].transform.position = ItemGridCells[GrabbedItemIndex].position;
                            Lerper.LerpTransformPosition(ItemGridCellMeshes[GrabbedItemIndex].transform, new Vector3(), 10f);
                            Lerper.LerpTransformPosition(ItemGridCellMeshes[CurrentIndex].transform, new Vector3(), 10f);

                            // Switch items
                            Client.Send(PktType.InventoryAction, Inventory.ActionHandler.BuildSlotSwitch(GrabbedItemIndex, CurrentIndex));

                            // Reset grabbing
                            GrabbedItem = null;
                            GrabbedItemIndex = -1;
                            ItemGridSelectedIndex = CurrentIndex;
                        } else {
                            ItemGridCellMeshes[GrabbedItemIndex].transform.position = LookedAt.point;
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
                    Mesh Mesh = null;
                    if (Item != null) {
                        Mesh = MeshDB.GetMesh(Item.ModelID);
                    }
                    ItemGridCellMeshes[i].mesh = Mesh;

                    // Set the base scale and spin status
                    float Scale = 0.08f;
                    bool Spinning = false;

                    if (focused == i || ItemGridSelectedIndex == i) {
                        // Item is selected or hovered, animate
                        Scale = 0.1f;
                        Spinning = true;
                    }

                    if (ItemGridSelectedIndex == i) {
                        // Set preview information
                        SetPreviewMesh(Mesh);
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
                                    ItemGridCells[i].position, 20f * Time.deltaTime);
                        }
                        Vector3 NewRot = ItemGridSelector.localEulerAngles;
                        NewRot.z = 0;
                        ItemGridSelector.localEulerAngles = NewRot;
                    }

                    if (!Spinning) {
                        // Not selected, reset rotation
                        ItemGridCellMeshes[i].transform.LookAt(Camera.transform);
                        Vector3 NewRot = ItemGridCellMeshes[i].transform.localEulerAngles;
                        NewRot.z = 0;
                        ItemGridCellMeshes[i].transform.localEulerAngles = NewRot;
                    }

                    // Update spinning status and scaling
                    ItemGridCells[i].GetComponent<Spinner>().Spinning = Spinning;
                    FixMeshScaling(ItemGridCellMeshes[i], Scale);
                }
            }

            // Hide the selector if nothing is selected
            ItemGridSelector.gameObject.SetActive(ItemGridSelectedIndex >= 0);

            // Clean up the preview screen if there was no item
            if (!ItemFound) {
                SetPreviewMesh(null);
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

        private void SetPreviewMesh(Mesh mesh) {
            ItemPreviewSpinner.Spinning = mesh != null;
            if (mesh != null) {
                ItemPreviewMesh.mesh = mesh;
            }
            FixMeshScaling(ItemPreviewMesh, 0.175f * PreviewVisibility);
        }

        private void FixMeshScaling(MeshFilter toFix, float scale) {
            if (toFix.mesh == null) {
                return;
            }

            float HighestExtent = 0.1f;
            float Height = toFix.mesh.bounds.extents.y * 2f;
            float Width = Mathf.Sqrt(Mathf.Pow(toFix.mesh.bounds.extents.x * 2f, 2) +
                Mathf.Pow(toFix.mesh.bounds.extents.y * 2f, 2));
            float Depth = Mathf.Sqrt(Mathf.Pow(toFix.mesh.bounds.extents.z * 2f, 2) +
                Mathf.Pow(toFix.mesh.bounds.extents.y * 2f, 2));

            if (Height > HighestExtent) {
                HighestExtent = Height;
            }
            if (Width > HighestExtent) {
                HighestExtent = Width;
            }
            if (Depth > HighestExtent) {
                HighestExtent = Depth;
            }

            float Scale = scale * 1f / HighestExtent;
            toFix.transform.localScale = new Vector3(Scale, Scale, Scale);
        }
    }
}
