using System;
using UnityEngine;
using UnityEngine.Networking;
using Cyber.Networking.Clientside;

namespace Cyber.Entities.SyncBases {
    
    /// <summary>
    /// A syncable component that all characters have. Controls the character's subsystems.
    /// </summary>
    public class Character : SyncBase {
        
        /// <summary>
        /// How fast the player should move in Unity's spatial units per second.
        /// </summary>
        public float MovementSpeed = 5.0f;

        /// <summary>
        /// The interaction distance of this player.
        /// </summary>
        public float InteractionDistance = 2.0f;

        /// <summary>
        /// The character controller, used to move the character. Handles collisions.
        /// </summary>
        public CharacterController CharacterController;

        /// <summary>
        /// The head transform for looking around.
        /// </summary>
        public Transform Head;

        private Vector3 MovementDirection = new Vector3();
        private Vector3 ServerPosition = new Vector3();
        private bool ServerPositionShouldLerpSync = false;

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
        /// Sets the position of this character.
        /// </summary>
        /// <param name="Position">Position.</param>
        public void SetPosition(Vector3 Position) {
            transform.position = Position;
        }

        /// <summary>
        /// Whether the player is moving or not.
        /// </summary>
        public bool Moving() {
            return MovementDirection.sqrMagnitude != 0;
        }

        /// <summary>
        /// Returns the current movement direction;
        /// </summary>
        /// <returns>The Movement Direction Vector3</returns>
        public Vector3 GetMovementDirection() {
            return MovementDirection;
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
        /// Returns the position of this character.
        /// </summary>
        /// <returns>The position.</returns>
        public Vector3 GetPosition() {
            return transform.position;
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
            ServerPosition = reader.ReadVector3();
            Vector3 ServerMovementDirection = reader.ReadVector3();
            Vector3 ServerRotation = reader.ReadVector3();

            float Drift = (ServerPosition - GetPosition()).magnitude;

            // Update position if this is the local player
            Character LocalCharacter = Client.GetConnectedPlayer().Character;
            if (Drift > MovementSpeed * 0.5f && LocalCharacter.Equals(this)) {
                SetPosition(ServerPosition);
                MovementDirection = ServerMovementDirection;
            }

            // Update position more often (with lerping) if this is not the local player
            if (Drift < 0.1) {
                ServerPositionShouldLerpSync = false;
            } else if (!LocalCharacter.Equals(this)) {
                ServerPositionShouldLerpSync = true;
            }
        }

        /// <summary>
        /// Serializes the character.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(NetworkWriter writer) {
            writer.Write(GetPosition());
            writer.Write(MovementDirection);
            writer.Write(GetRotation());
        }

        private void Update() {
            if (ServerPositionShouldLerpSync) {
                SetPosition(Vector3.Lerp(GetPosition(), ServerPosition, 10f * Time.deltaTime));
            }
        }

        private void FixedUpdate() {
            CharacterController.Move(MovementDirection * MovementSpeed * Time.fixedDeltaTime);
        }
    }
}
