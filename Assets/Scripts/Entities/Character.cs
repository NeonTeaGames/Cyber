using System;
using UnityEngine;
using UnityEngine.Networking;

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

        /// <summary>
        /// The head transform for looking around.
        /// </summary>
        public Transform Head;

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
        /// Sets the character's rotation.
        /// </summary>
        /// <param name="EulerAngles">Rotation in euler angles.</param>
        public void SetRotation(Vector3 EulerAngles) {
            Vector3 HeadRot = Head.localEulerAngles;
            HeadRot.x = EulerAngles.x;
            HeadRot.z = EulerAngles.z;
            Head.localEulerAngles = HeadRot;

            Vector3 BodyRot = transform.localEulerAngles;
            BodyRot.y = EulerAngles.y;
            transform.localEulerAngles = BodyRot;
        }

        /// <summary>
        /// Whether the player is moving or not.
        /// </summary>
        public bool Moving() {
            return MovementDirection.sqrMagnitude != 0;
        }

        /// <summary>
        /// The character's rotation. Intended to be given as an input to 
        /// <see cref="SetRotation"/>.
        /// </summary>
        /// <returns>The rotation.</returns>
        public Vector3 GetRotation() {
            return new Vector3(Head.localEulerAngles.x, 
                transform.localEulerAngles.y, Head.localEulerAngles.z);
        }

        /// <summary>
        /// Gets the Sync Handletype for Character, which doesn't require hash differences and syncs every tick.
        /// </summary>
        /// <returns></returns>
        public override SyncHandletype GetSyncHandletype() {
            return new SyncHandletype(false, 1);
        }

        /// <summary>
        /// Deserializes the character.
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(NetworkReader reader) {
            Vector3 ServerPosition = reader.ReadVector3();
            Vector3 ServerMovementDirection = reader.ReadVector3();
            Vector3 ServerRotation = reader.ReadVector3();

            float Drift = (ServerPosition - transform.position).magnitude;
            if (Drift > MovementSpeed * 0.5f) {
                transform.position = ServerPosition;
                MovementDirection = ServerMovementDirection;
            }
        }

        /// <summary>
        /// Serializes the character.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(transform.position);
            writer.Write(MovementDirection);
            writer.Write(GetRotation());
        }

        private void FixedUpdate() {
            CharacterController.Move(MovementDirection * MovementSpeed * Time.fixedDeltaTime);
        }
    }
}
