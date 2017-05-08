using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Entities {
    
    /// <summary>
    /// A syncable component that all characters have. Controls the character's subsystems.
    /// </summary>
    public class Character : SyncBase {
        
        /// <summary>
        /// How fast the player should move in Unity's spatial units per second.
        /// </summary>
        public float MovementSpeed = 5.0f;

        /// <summary>
        /// The character controller, used to move the character. Handles collisions.
        /// </summary>
        public CharacterController CharacterController;

        private Vector3 MovementDirection = new Vector3();

        /// <summary>
        /// Moves the character in the given direction.
        /// </summary>
        /// <param name="Direction">Movement direction.</param>
        public void Move(Vector3 Direction) {
            if (!Direction.Equals(MovementDirection)) {
                MovementDirection = Direction.normalized;
            }
        }

        /// <summary>
        /// Stops the player from moving.
        /// </summary>
        public void Stop() {
            if (Moving()) {
                MovementDirection = new Vector3();
            }
        }

        /// <summary>
        /// Whether the player is moving or not.
        /// </summary>
        public bool Moving() {
            return MovementDirection.sqrMagnitude != 0;
        }

        private void FixedUpdate() {
            CharacterController.Move(MovementDirection * MovementSpeed * Time.fixedDeltaTime);
        }
    }
}
