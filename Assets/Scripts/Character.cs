using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : SyncBase {
    public float MovementSpeed = 5.0f;
    public CharacterController CharacterController;

    /// <summary>
    /// Moves the character in the wanted direction. Should be called on the physics tick. (FixedUpdate)
    /// </summary>
    /// <param name="Direction">Movement direction.</param>
    public void Move(Vector3 Direction) {
        CharacterController.Move(Direction.normalized * MovementSpeed * Time.fixedDeltaTime);
    }
}
