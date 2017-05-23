using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {

    /// <summary>
    /// Lerps stuff.
    /// </summary>
    public class Lerper : MonoBehaviour {
        private struct PositionLerp {
            public Vector3 Target;
            public float Speed;

            public PositionLerp(Vector3 target, float speed) {
                Target = target;
                Speed = speed;
            }
        }

        private static Lerper Singleton;

        private Dictionary<Transform, PositionLerp> PositionLerps = new Dictionary<Transform, PositionLerp>();

        /// <summary>
        /// Sets the singleton.
        /// </summary>
        public Lerper() {
            Singleton = this;
        }

        /// <summary>
        /// Lerps the transform world position.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="to">To.</param>
        /// <param name="speed">Speed.</param>
        public static void LerpTransformPosition(Transform transform, Vector3 to, float speed) {
            if (Singleton != null) {
                Singleton.PositionLerps[transform] = new PositionLerp(to, speed);
            }
        }

        private void LateUpdate() {
            List<Transform> RemoveThese = new List<Transform>();
            foreach (Transform Transform in PositionLerps.Keys) {
                if (Transform == null) {
                    PositionLerps.Remove(Transform);
                    continue;
                }
                Transform.position = Vector3.Lerp(Transform.position, 
                    PositionLerps[Transform].Target, PositionLerps[Transform].Speed * Time.deltaTime);
                if ((Transform.position - PositionLerps[Transform].Target).magnitude < 0.001f) {
                    Transform.position = PositionLerps[Transform].Target;
                    RemoveThese.Add(Transform);
                }
            }
            foreach (Transform Transform in RemoveThese) {
                PositionLerps.Remove(Transform);
            }
        }
    }
}