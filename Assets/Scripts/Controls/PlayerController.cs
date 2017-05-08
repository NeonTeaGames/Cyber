using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber.Entities;
using Cyber.Console;

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

        private void Update() {
            if (!Term.IsVisible()) {
                // Handle inputs
                Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (Move.sqrMagnitude != 0) {
                    Character.Move(transform.TransformDirection(Move));
                } else if (Character.Moving()) {
                    Character.Stop();
                }
            } else if (Character.Moving()) {
                // The debug console is open, stop the player.
                Character.Stop();
            }
        }
    }
}