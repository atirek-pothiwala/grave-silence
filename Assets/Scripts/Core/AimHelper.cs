using UnityEngine;
using UnityEngine.InputSystem;

namespace GraveSilence.Core
{
    /// <summary>
    /// Screen-to-world raycasting via the new Input System.
    /// </summary>
    public static class AimHelper
    {
        public static bool TryGetAimPoint(Camera camera, float maxDistance, LayerMask mask, out Vector3 point)
        {
            point = Vector3.zero;
            if (camera == null) return false;

            Vector2 screenPos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            Ray ray = camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask))
            {
                point = hit.point;
                return true;
            }

            return false;
        }

        public static bool TryGetAimHit(Camera camera, float maxDistance, LayerMask mask, out RaycastHit hit)
        {
            hit = default;
            if (camera == null) return false;

            Vector2 screenPos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            Ray ray = camera.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out hit, maxDistance, mask);
        }
    }
}
