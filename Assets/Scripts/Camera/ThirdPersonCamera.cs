using UnityEngine;

namespace GraveSilence.Camera
{
    /// <summary>
    /// Third-person camera follow with collision avoidance.
    /// Works with Cinemachine or as a standalone follow camera.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 2f, -4f);
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float minDistance = 1.5f;
        [SerializeField] private float maxDistance = 6f;
        [SerializeField] private LayerMask collisionMask;

        private float currentYaw;
        private float currentPitch = 15f;

        private void LateUpdate()
        {
            if (target == null) return;

            HandleRotation();
            UpdatePosition();
        }

        private void HandleRotation()
        {
            if (GraveSilence.Core.InputManager.Instance == null) return;

            Vector2 look = GraveSilence.Core.InputManager.Instance.LookAction.ReadValue<Vector2>();
            currentYaw += look.x * rotationSpeed;
            currentPitch = Mathf.Clamp(currentPitch - look.y * rotationSpeed, -10f, 60f);
        }

        private void UpdatePosition()
        {
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 desiredPosition = target.position + rotation * offset;

            if (Physics.Raycast(target.position + Vector3.up, desiredPosition - target.position,
                    out RaycastHit hit, offset.magnitude, collisionMask))
            {
                desiredPosition = hit.point + hit.normal * 0.2f;
            }

            float distance = Vector3.Distance(target.position, desiredPosition);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }
}
