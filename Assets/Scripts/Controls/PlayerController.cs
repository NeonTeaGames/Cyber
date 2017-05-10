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

        private CursorHandler CursorHandler;
        private Vector3 Rotation;

        private void Start() {
            CursorHandler = GameObject.Find("/CursorHandler").GetComponent<CursorHandler>();
        }

        private void Update() {
            if (!Term.IsVisible()) {
                // Handle inputs
                Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (Move.sqrMagnitude != 0) {
                    Character.Move(Character.transform.TransformDirection(Move));

                    Client.Send(PktType.MoveCreature, new MoveCreaturePkt(Character.transform.TransformDirection(Move), Character.ID));
                } else if (Character.Moving()) {
                    Character.Stop();

                    Client.Send(PktType.MoveCreature, new MoveCreaturePkt(new Vector3(), Character.ID));
                }

                Rotation.y += Input.GetAxis("Mouse X") * CursorHandler.MouseSensitivityX;
                Rotation.x = Mathf.Clamp(Rotation.x - Input.GetAxis("Mouse Y") * CursorHandler.MouseSensitivityY, -89, 89);
                Character.SetRotation(Rotation);
            } else if (Character.Moving()) {
                // The debug console is open, stop the player.
                Character.Stop();
            }
        }
    }
}