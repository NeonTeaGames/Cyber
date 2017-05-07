using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Character Character;

    void FixedUpdate() {
        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Move = transform.TransformDirection(Move);
        Character.Move(Move);
    }
}
