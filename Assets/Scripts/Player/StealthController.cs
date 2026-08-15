using UnityEngine;
using GraveSilence.Systems;

namespace GraveSilence.Player
{
    /// <summary>
    /// Tracks player visibility, noise, and shadow state for stealth gameplay.
    /// Inspired by Aragami 2's shadow mechanics, adapted for a ruined city at night.
    /// </summary>
    public class StealthController : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float baseVisibility = 0.3f;
        [SerializeField] private float sprintVisibilityMultiplier = 2f;
        [SerializeField] private float crouchVisibilityMultiplier = 0.5f;
        [SerializeField] private LayerMask lightProbeMask;

        [Header("Shadow")]
        [SerializeField] private float shadowCheckRadius = 1.5f;
        [SerializeField] private float inShadowVisibilityMultiplier = 0.15f;

        [Header("Noise")]
        [SerializeField] private float walkNoise = 0.2f;
        [SerializeField] private float sprintNoise = 1f;
        [SerializeField] private float crouchNoise = 0.05f;

        private ThirdPersonController movement;
        private bool isInShadow;
        private bool isCloaked;
        private float currentVisibility;
        private float currentNoise;

        public float Visibility => isCloaked ? 0f : currentVisibility;
        public float Noise => currentNoise;
        public bool IsInShadow => isInShadow || isCloaked;
        public bool IsCloaked => isCloaked;

        public event System.Action<float> OnVisibilityChanged;
        public event System.Action<bool> OnShadowStateChanged;

        private void Awake()
        {
            movement = GetComponent<ThirdPersonController>();
        }

        private void Update()
        {
            UpdateShadowState();
            UpdateVisibility();
            DecayNoise();
        }

        public void SetCloaked(bool cloaked)
        {
            isCloaked = cloaked;
            OnShadowStateChanged?.Invoke(IsInShadow);
        }

        public void RegisterMovementNoise(float speed, bool crouching)
        {
            float noise = crouching ? crouchNoise : (speed > 5f ? sprintNoise : walkNoise);
            currentNoise = Mathf.Max(currentNoise, noise);
            NoiseSystem.Instance?.EmitNoise(transform.position, noise, NoiseType.Footstep);
        }

        public void EmitNoise(float amount, NoiseType type)
        {
            currentNoise = Mathf.Max(currentNoise, amount);
            NoiseSystem.Instance?.EmitNoise(transform.position, amount, type);
        }

        private void UpdateShadowState()
        {
            bool wasInShadow = isInShadow;

            // Darkness volumes tagged "ShadowZone" or areas with no direct light
            Collider[] shadows = Physics.OverlapSphere(transform.position, shadowCheckRadius);
            isInShadow = false;
            foreach (var col in shadows)
            {
                if (col.CompareTag("ShadowZone"))
                {
                    isInShadow = true;
                    break;
                }
            }

            if (isInShadow != wasInShadow)
                OnShadowStateChanged?.Invoke(IsInShadow);
        }

        private void UpdateVisibility()
        {
            float visibility = baseVisibility;

            if (movement != null)
            {
                if (movement.IsCrouching)
                    visibility *= crouchVisibilityMultiplier;
                else if (movement.IsSprinting)
                    visibility *= sprintVisibilityMultiplier;
            }

            if (isInShadow)
                visibility *= inShadowVisibilityMultiplier;

            currentVisibility = Mathf.Clamp01(visibility);
            OnVisibilityChanged?.Invoke(Visibility);
        }

        private void DecayNoise()
        {
            currentNoise = Mathf.Lerp(currentNoise, 0f, Time.deltaTime * 3f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isInShadow ? Color.black : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shadowCheckRadius);
        }
    }
}
