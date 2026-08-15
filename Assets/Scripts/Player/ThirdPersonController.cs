using UnityEngine;

namespace GraveSilence.Player
{
    /// <summary>
    /// Third-person character controller with Aragami-style fluid movement.
    /// Supports walk, sprint, crouch, and rotation toward camera direction.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float sprintSpeed = 6.5f;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float standingHeight = 1.8f;
        [SerializeField] private float crouchHeight = 1.2f;

        [Header("References")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private StealthController stealthController;

        private CharacterController controller;
        private Vector3 velocity;
        private float rotationVelocity;
        private bool isSprinting;
        private bool isCrouching;
        private Vector3 lastMoveDirection;

        public bool IsMoving => lastMoveDirection.sqrMagnitude > 0.01f;
        public bool IsSprinting => isSprinting && IsMoving;
        public bool IsCrouching => isCrouching;
        public float CurrentSpeed => controller.velocity.magnitude;
        public Transform CameraTarget => cameraTarget;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            stealthController ??= GetComponent<StealthController>();
        }

        private void Update()
        {
            ApplyGravity();
        }

        public void SetCameraTarget(Transform target) => cameraTarget = target;

        public void SetSprinting(bool sprinting)
        {
            isSprinting = sprinting && !isCrouching;
        }

        public void SetCrouching(bool crouching)
        {
            isCrouching = crouching;
            if (isCrouching) isSprinting = false;

            float targetHeight = isCrouching ? crouchHeight : standingHeight;
            controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
            controller.center = new Vector3(0f, controller.height * 0.5f, 0f);
        }

        public void Move(Vector2 input)
        {
            if (input.sqrMagnitude < 0.01f)
            {
                lastMoveDirection = Vector3.zero;
                return;
            }

            float targetSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
            Vector3 inputDirection = new Vector3(input.x, 0f, input.y).normalized;

            if (cameraTarget != null)
            {
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                                    + cameraTarget.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                    ref rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                lastMoveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(lastMoveDirection * (targetSpeed * Time.deltaTime));
            }
            else
            {
                lastMoveDirection = transform.TransformDirection(inputDirection);
                controller.Move(lastMoveDirection * (targetSpeed * Time.deltaTime));
            }

            stealthController?.RegisterMovementNoise(targetSpeed, isCrouching);
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0f)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
