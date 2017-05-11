using UnityEngine;
using Cyber.Entities.SyncBases;
using Cyber.Console;
using Cyber.Networking.Clientside;
using Cyber.Networking;
using Cyber.Networking.Messages;

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

        private CursorHandler CursorHandler;
        private Vector3 Rotation;

        private void Start() {
            CursorHandler = GameObject.Find("/Systems/CursorHandler").GetComponent<CursorHandler>();
        }

        private void Update() {
            if (!Term.IsVisible()) {
                // Don't do any "gameplay stuff" if the debug console is up

                // Handle inputs
                // Movement
                Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (Move.sqrMagnitude != 0) {
                    Character.Move(Character.transform.TransformDirection(Move));
                    Client.Send(PktType.MoveCreature, new MoveCreaturePkt(Character.transform.TransformDirection(Move), Character.ID));
                } else if (Character.Moving()) {
                    Character.Stop();
                    Client.Send(PktType.MoveCreature, new MoveCreaturePkt(new Vector3(), Character.ID));
                }

                // Rotation
                Rotation.y += Input.GetAxis("Mouse X") * CursorHandler.MouseSensitivityX;
                Rotation.x = Mathf.Clamp(Rotation.x - Input.GetAxis("Mouse Y") * CursorHandler.MouseSensitivityY, -89, 89);
                Character.SetRotation(Rotation);
            
                // Interactions
                if (Input.GetButtonDown("Activate")) {
                    GameObject LookedAtObject = GetLookedAtGameObject();
                    if (LookedAtObject != null) {
                        Interactable LookingAt = LookedAtObject.GetComponent<Interactable>();
                        if (LookingAt != null && (LookingAt.transform.position - Character.GetPosition()).magnitude < Character.InteractionDistance) {
                            LookingAt.Interact(Character);
                            if (LookingAt.GetInteractableSyncdata().PublicInteractions) {
                                Client.Send(PktType.InteractPkt, new InteractionPkt(LookingAt.ID));
                            }
                        }
                    }
                }
            } else if (Character.Moving()) {
                // The debug console is open, stop the player.
                Character.Stop();
            }
        }

        private GameObject GetLookedAtGameObject() {
            RaycastHit Hit;
            Physics.Raycast(Camera.transform.position, Camera.transform.forward, out Hit, Character.InteractionDistance);
            if (Hit.collider != null) {
                return Hit.collider.gameObject;
            } else {
                return null;
            }
        }
    }
}