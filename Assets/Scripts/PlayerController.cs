using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Character Character;

    void Update() {
        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Move.sqrMagnitude != 0) {
            Character.Move(transform.TransformDirection(Move));
        } else if (Character.Moving()) {
            Character.Stop();
        }
    }
}
