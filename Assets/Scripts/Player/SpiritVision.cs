using UnityEngine;
using GraveSilence.Core;
using GraveSilence.Enemies;

namespace GraveSilence.Player
{
    /// <summary>
    /// Aragami-style spirit vision — briefly reveals zombie awareness and positions.
    /// Toggle with Tab (wired via InputManager Ability5 or a dedicated action).
    /// </summary>
    public class SpiritVision : MonoBehaviour
    {
        [SerializeField] private float visionDuration = 4f;
        [SerializeField] private float visionCooldown = 12f;
        [SerializeField] private float energyCost = 10f;
        [SerializeField] private Color unawareColor = new(0.2f, 0.8f, 0.3f, 0.6f);
        [SerializeField] private Color suspiciousColor = new(1f, 0.85f, 0.2f, 0.7f);
        [SerializeField] private Color alertColor = new(1f, 0.2f, 0.2f, 0.8f);

        private UmbralAbilities abilities;
        private float cooldownTimer;
        private float activeTimer;
        private bool isActive;

        public bool IsActive => isActive;
        public float CooldownRemaining => cooldownTimer;

        public event System.Action<bool> OnVisionToggled;

        private void Awake()
        {
            abilities = GetComponent<UmbralAbilities>();
        }

        private void Update()
        {
            cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.deltaTime);

            if (!isActive) return;

            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f)
                Deactivate();
        }

        public bool TryActivate()
        {
            if (isActive || cooldownTimer > 0f) return false;
            if (abilities != null && !abilities.TrySpendEnergy(energyCost)) return false;

            isActive = true;
            activeTimer = visionDuration;
            cooldownTimer = visionCooldown;
            OnVisionToggled?.Invoke(true);
            return true;
        }

        public void Deactivate()
        {
            if (!isActive) return;
            isActive = false;
            OnVisionToggled?.Invoke(false);
        }

        public Color GetAwarenessColor(float awareness)
        {
            if (awareness < 0.3f) return unawareColor;
            if (awareness < 0.7f) return suspiciousColor;
            return alertColor;
        }

        private void OnDrawGizmos()
        {
            if (!isActive) return;

            var zombies = FindObjectsByType<ZombieBase>(FindObjectsSortMode.None);
            foreach (var zombie in zombies)
            {
                if (zombie.CurrentState == ZombieState.Dead) continue;

                Gizmos.color = GetAwarenessColor(zombie.Awareness);
                Gizmos.DrawWireSphere(zombie.transform.position + Vector3.up * 2f, 0.5f);
                Gizmos.DrawLine(transform.position + Vector3.up, zombie.transform.position + Vector3.up);
            }
        }
    }
}
