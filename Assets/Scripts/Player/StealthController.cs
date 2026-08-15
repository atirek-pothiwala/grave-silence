using UnityEngine;
using GraveSilence.Environment;
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

        [Header("Shadow")]
        [SerializeField] private float shadowCheckRadius = 1.5f;
        [SerializeField] private float inShadowVisibilityMultiplier = 0.15f;

        [Header("Noise")]
        [SerializeField] private float walkNoise = 0.2f;
        [SerializeField] private float sprintNoise = 1f;
        [SerializeField] private float crouchNoise = 0.05f;
        [SerializeField] private float noiseEmitInterval = 0.4f;

        private ThirdPersonController movement;
        private int shadowZoneCount;
        private bool isCloaked;
        private float currentVisibility;
        private float currentNoise;
        private float lastNoiseEmitTime;
        private float lastReportedVisibility = -1f;

        public float Visibility => isCloaked ? 0f : currentVisibility;
        public float Noise => currentNoise;
        public bool IsInShadow => shadowZoneCount > 0 || isCloaked;
        public bool IsCloaked => isCloaked;

        public event System.Action<float> OnVisibilityChanged;
        public event System.Action<bool> OnShadowStateChanged;
        public event System.Action OnCloakBroken;

        private void Awake()
        {
            movement = GetComponent<ThirdPersonController>();
        }

        private void Update()
        {
            if (isCloaked && movement != null && movement.IsMoving)
                BreakCloak();

            UpdateVisibility();
            DecayNoise();
        }

        public void SetCloaked(bool cloaked)
        {
            if (isCloaked == cloaked) return;
            isCloaked = cloaked;
            OnShadowStateChanged?.Invoke(IsInShadow);
            UpdateVisibility();
        }

        public void RegisterMovementNoise(float speed, bool crouching)
        {
            float noise = crouching ? crouchNoise : (speed > 5f ? sprintNoise : walkNoise);
            currentNoise = Mathf.Max(currentNoise, noise);

            if (Time.time - lastNoiseEmitTime < noiseEmitInterval) return;
            lastNoiseEmitTime = Time.time;
            NoiseSystem.Instance?.EmitNoise(transform.position, noise, NoiseType.Footstep);
        }

        public void EmitNoise(float amount, NoiseType type)
        {
            currentNoise = Mathf.Max(currentNoise, amount);
            NoiseSystem.Instance?.EmitNoise(transform.position, amount, type);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ShadowZone>() != null)
            {
                shadowZoneCount++;
                OnShadowStateChanged?.Invoke(IsInShadow);
                UpdateVisibility();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<ShadowZone>() != null)
            {
                shadowZoneCount = Mathf.Max(0, shadowZoneCount - 1);
                if (isCloaked && shadowZoneCount == 0)
                    BreakCloak();

                OnShadowStateChanged?.Invoke(IsInShadow);
                UpdateVisibility();
            }
        }

        private void BreakCloak()
        {
            isCloaked = false;
            OnCloakBroken?.Invoke();
            OnShadowStateChanged?.Invoke(IsInShadow);
            UpdateVisibility();
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

            if (IsInShadow)
                visibility *= inShadowVisibilityMultiplier;

            currentVisibility = Mathf.Clamp01(visibility);

            if (!Mathf.Approximately(currentVisibility, lastReportedVisibility))
            {
                lastReportedVisibility = currentVisibility;
                OnVisibilityChanged?.Invoke(Visibility);
            }
        }

        private void DecayNoise()
        {
            currentNoise = Mathf.Lerp(currentNoise, 0f, Time.deltaTime * 3f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsInShadow ? Color.black : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shadowCheckRadius);
        }
    }
}
