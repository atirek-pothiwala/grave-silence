using UnityEngine;
using GraveSilence.Core;

namespace GraveSilence.Player
{
    /// <summary>
    /// Player health and death handling. Mission fails on death.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float invincibilityDuration = 0.5f;

        private float health;
        private float invincibilityTimer;

        public float Health => health;
        public float HealthNormalized => health / maxHealth;

        public event System.Action<float> OnHealthChanged;
        public event System.Action OnPlayerDied;

        private void Awake()
        {
            health = maxHealth;
        }

        private void Update()
        {
            invincibilityTimer = Mathf.Max(0f, invincibilityTimer - Time.deltaTime);
        }

        public void TakeDamage(float damage)
        {
            if (invincibilityTimer > 0f || health <= 0f) return;

            health = Mathf.Max(0f, health - damage);
            invincibilityTimer = invincibilityDuration;
            OnHealthChanged?.Invoke(HealthNormalized);

            if (health <= 0f)
            {
                OnPlayerDied?.Invoke();
                GameManager.Instance?.FailMission("Player was killed");
            }
        }

        public void Heal(float amount)
        {
            health = Mathf.Min(maxHealth, health + amount);
            OnHealthChanged?.Invoke(HealthNormalized);
        }
    }
}
