using UnityEngine;
using Cyber.Entities.SyncBases;
using Cyber.Console;
using Cyber.Networking.Clientside;
using Cyber.Networking;
using Cyber.Networking.Messages;
using Cyber.Entities;
using Cyber.Util;
using Cyber.Items;

namespace Cyber.Controls {
    
    /// <summary>
    /// Controls the player character. Shouldn't exist on the server, and only one
    /// should exist per client (the character that client is controlling).
    /// </summary>
    public class PlayerController : MonoBehaviour {
        
        /// <summary>
        /// The character this controller should control.
        /// </summary>
        public Character Character;

        /// <summary>
        /// The camera the player is seeing the world through.
        /// </summary>
        public Camera Camera;

        private Vector3 Rotation;
        private GameObject LastLookedAt;

        private void Update() {
            if (!Term.IsVisible()) {
                // Don't do any "gameplay stuff" if the debug console is up

                // Handle inputs
                // Movement
                Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (Move.sqrMagnitude != 0) {
                    Character.Move(Character.transform.TransformDirection(Move));
                } else if (Character.Moving()) {
                    Character.Stop();
                }

                // Rotation (only when cursor is locked
                if (CursorHandler.Locked()) {
                    Rotation.y += Input.GetAxis("Mouse X") * CursorHandler.GetMouseSensitivityX();
                    Rotation.x = Mathf.Clamp(Rotation.x - Input.GetAxis("Mouse Y") * CursorHandler.GetMouseSensitivityY(), -89, 89);
                    Character.SetRotation(Rotation);
                }
            
                // Interactions
                bool Interacted = false;
                GameObject LookedAtObject = CameraUtil.GetLookedAtGameObject(Camera, Character.InteractionDistance);
                if (LookedAtObject != null) {
                    Interactable LookingAt = LookedAtObject.GetComponent<Interactable>();
                    if (LookingAt != null && (LookingAt.transform.position - Character.GetPosition()).magnitude < Character.InteractionDistance) {
                        if (Input.GetButtonDown("Activate")) {
                            InteractWith(LookingAt, InteractionType.Activate);
                            Interacted = true;
                        }
                        if (Input.GetButtonUp("Activate")) {
                            InteractWith(LookingAt, InteractionType.Deactivate);
                            Interacted = true;
                        }
                        if (LookedAtObject != LastLookedAt) {
                            InteractWith(LookingAt, InteractionType.Enter);
                            if (LastLookedAt != null) {
                                InteractWith(LastLookedAt.GetComponent<Interactable>(), InteractionType.Exit);
                            }
                        }
                        LastLookedAt = LookedAtObject;
                    }
                } else if (LastLookedAt != null) {
                    InteractWith(LastLookedAt.GetComponent<Interactable>(), InteractionType.Exit);
                    LastLookedAt = null;
                }

                // Equipment actions
                if (!Interacted) {
                    // Don't use equipment if you're interacting with something
                    // (ie. don't shoot at the buttons)
                    if (Input.GetButtonDown("Use Item (R)")) {
                        Character.UseItemInSlot(EquipSlot.RightHand);
                        Client.Send(PktType.InventoryAction, new InventoryActionPkt(InventoryAction.Use, (int) EquipSlot.RightHand));
                    }
                    if (Input.GetButtonDown("Use Item (L)")) {
                        Character.UseItemInSlot(EquipSlot.LeftHand);
                        Client.Send(PktType.InventoryAction, new InventoryActionPkt(InventoryAction.Use, (int) EquipSlot.LeftHand));
                    }
                }
            } else if (Character.Moving()) {
                // The debug console is open, stop the player.
                Character.Stop();
            }
        }

        private void InteractWith(Interactable interactable, InteractionType type) {
            interactable.Interact(Character, type);
            if (interactable.GetInteractableSyncdata().PublicInteractions) {
                Client.Send(PktType.Interact, new InteractionPkt(interactable.ID, type));
            }
        }
    }
}