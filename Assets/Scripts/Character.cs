using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : SyncBase {
    public float MovementSpeed = 5.0f;
    public CharacterController CharacterController;

    private Vector3 MovementDirection = new Vector3();

    /// <summary>
    /// Moves the character in the wanted direction.
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

    public bool Moving() {
        return MovementDirection.sqrMagnitude != 0;
    }

    private void FixedUpdate() {
        CharacterController.Move(MovementDirection * MovementSpeed * Time.fixedDeltaTime);
    }
}
