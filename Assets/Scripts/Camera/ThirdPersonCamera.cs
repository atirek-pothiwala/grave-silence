using UnityEngine;

namespace GraveSilence.Camera
{
    /// <summary>
    /// Third-person camera follow with collision avoidance.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 2f, -4f);
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 0.15f;
        [SerializeField] private float minDistance = 1.5f;
        [SerializeField] private float maxDistance = 6f;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private bool drivePlayerRotation = true;

        private float currentYaw;
        private float currentPitch = 15f;
        private GraveSilence.Player.ThirdPersonController playerMovement;

        private void Start()
        {
            if (target != null)
                playerMovement = target.GetComponent<GraveSilence.Player.ThirdPersonController>();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleRotation();
            UpdatePosition();

            if (drivePlayerRotation && playerMovement != null)
                playerMovement.SetCameraTarget(transform);
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
            Vector3 pivot = target.position + Vector3.up * 1.5f;
            Vector3 desiredOffset = rotation * offset;
            float targetDistance = Mathf.Clamp(desiredOffset.magnitude, minDistance, maxDistance);
            Vector3 direction = desiredOffset.normalized;
            Vector3 desiredPosition = pivot + direction * targetDistance;

            if (Physics.Raycast(pivot, direction, out RaycastHit hit, targetDistance, collisionMask))
                desiredPosition = hit.point + hit.normal * 0.2f;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.LookAt(pivot);
        }
    }
}
