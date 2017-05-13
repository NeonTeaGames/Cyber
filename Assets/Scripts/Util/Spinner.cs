using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {
    
    /// <summary>
    /// Spins the transform at a defined speed.
    /// </summary>
    public class Spinner : MonoBehaviour {

        /// <summary>
        /// The axis this spinner spins around.
        /// </summary>
        public Vector3 Axis = new Vector3(0, 1, 0);

        /// <summary>
        /// The rounds per minute of the spinning.
        /// </summary>
        public float RoundsPerMinute = 30;

        /// <summary>
        /// Whether the spinner is spinning currently.
        /// </summary>
        public bool Spinning = true;

        private float CurrentRelativeSpeed = 1f;

        private void Start() {
            CurrentRelativeSpeed = Spinning ? 1f : 0f;
        }

        private void Update() {
            CurrentRelativeSpeed = Mathf.Lerp(CurrentRelativeSpeed, 
                Spinning ? 1f : 0f, 20f * Time.deltaTime);
            transform.localEulerAngles = transform.localEulerAngles + 
                Axis * RoundsPerMinute * CurrentRelativeSpeed * Time.deltaTime;
        }
    }
}