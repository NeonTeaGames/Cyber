using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Util {

    /// <summary>
    /// Static utility functions related to cameras.
    /// </summary>
    public class CameraUtil {

        /// <summary>
        /// Gets the first hit of a ray from the middle of the camera hits.
        /// </summary>
        /// <returns>The looked at game object.</returns>
        /// <param name="camera">Camera.</param>
        /// <param name="distance">Distance.</param>
        /// <param name="useMousePosition">Whether the raycast should use the
        /// mouse position on the screen (as opposed to the center).</param>
        public static RaycastHit GetLookedAtHit(Camera camera, 
            float distance, bool useMousePosition = false) {
            RaycastHit Hit;
            Ray Ray;
            if (useMousePosition) {
                Ray = camera.ScreenPointToRay(Input.mousePosition);
            } else {
                Ray = new Ray(camera.transform.position, camera.transform.forward);
            }
            Physics.Raycast(Ray, out Hit, distance);
            return Hit;
        }

        /// <summary>
        /// Gets the first game object a ray from the middle of the camera hits.
        /// </summary>
        /// <returns>The looked at game object.</returns>
        /// <param name="camera">Camera.</param>
        /// <param name="distance">Distance.</param>
        /// <param name="useMousePosition">Whether the raycast should use the
        /// mouse position on the screen (as opposed to the center).</param>
        public static GameObject GetLookedAtGameObject(Camera camera, 
                float distance, bool useMousePosition = false) {
            RaycastHit Result = GetLookedAtHit(camera, distance, useMousePosition);
            if (Result.collider != null) {
                return Result.collider.gameObject;
            } else {
                return null;
            }
        }
    }
}