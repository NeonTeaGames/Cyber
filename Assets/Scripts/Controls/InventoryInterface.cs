using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Util;
using Cyber.Console;

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

        private CursorHandler CursorHandler;
        private MeshDB MeshDB;
        private bool InventoryOpen = false;

        private List<Transform> ItemGridCells;
        private List<MeshFilter> ItemGridCellMeshes;
        private int ItemGridSelectedIndex;

        private Color IconInventoryColor;
        private Color IconStatusColor;
        private Color IconSocialColor;
        private Color IconMapColor;

        private void Start() {
            CursorHandler = GameObject.Find("/Systems/CursorHandler").GetComponent<CursorHandler>();
            MeshDB = GameObject.Find("/Systems/MeshDB").GetComponent<MeshDB>();

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
                    if (Input.GetButtonDown("Activate")) {
                        ItemGridSelectedIndex = CurrentIndex;
                    }
                } else if (Mesh != null) {
                    float InvBrightness = 1f;
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
            if (ItemGridSelectedIndex < 0) {
                SetPreviewMesh(null);
            }
            for (int i = 0; i < ItemGridDimensions.x * ItemGridDimensions.y; i++) {
                ItemGridCellMeshes[i].mesh = MeshDB.Meshes[i % MeshDB.Meshes.Length];
                float Scale = 0.08f;
                bool Spinning = false;
                if (focused == i || ItemGridSelectedIndex == i) {
                    Scale = 0.1f;
                    Spinning = true;
                }
                if (ItemGridSelectedIndex == i) {
                    SetPreviewMesh(ItemGridCellMeshes[i].mesh);
                    if ((ItemGridSelector.position - ItemGridCells[i].position).magnitude < 0.01f) {
                        ItemGridSelector.position = ItemGridCells[i].position;
                    } else {
                        ItemGridSelector.position = 
                            Vector3.Lerp(ItemGridSelector.position, 
                                ItemGridCells[i].position, 20f * Time.deltaTime);
                    }
                    ItemGridSelector.LookAt(Camera.transform);
                    Vector3 NewRot = ItemGridSelector.localEulerAngles;
                    NewRot.z = 0;
                    ItemGridSelector.localEulerAngles = NewRot;
                }
                if (!Spinning) {
                    ItemGridCellMeshes[i].transform.LookAt(Camera.transform);
                    Vector3 NewRot = ItemGridCellMeshes[i].transform.localEulerAngles;
                    NewRot.z = 0;
                    ItemGridCellMeshes[i].transform.localEulerAngles = NewRot;
                }
                ItemGridCells[i].GetComponent<Spinner>().Spinning = Spinning;
                FixMeshScaling(ItemGridCellMeshes[i], Scale);
            }
        }

        private void SetPreviewMesh(Mesh mesh) {
            ItemPreviewSpinner.Spinning = mesh != null;
            ItemPreviewMesh.mesh = mesh;

            if (mesh != null) {
                FixMeshScaling(ItemPreviewMesh, 0.175f);
            }
        }

        private void FixMeshScaling(MeshFilter toFix, float scale) {
            float HighestExtent = 0f;
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