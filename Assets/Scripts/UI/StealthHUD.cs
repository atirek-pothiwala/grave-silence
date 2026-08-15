using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GraveSilence.Player;
using GraveSilence.Systems;

namespace GraveSilence.UI
{
    /// <summary>
    /// HUD showing stealth state, umbral energy, health, and alert level.
    /// </summary>
    public class StealthHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StealthController stealth;
        [SerializeField] private UmbralAbilities abilities;
        [SerializeField] private PlayerHealth health;

        [Header("UI Elements")]
        [SerializeField] private Slider visibilityBar;
        [SerializeField] private Slider energyBar;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider alertBar;
        [SerializeField] private Image shadowIndicator;
        [SerializeField] private TextMeshProUGUI abilityFeedbackText;
        [SerializeField] private float feedbackDuration = 2f;

        private float feedbackTimer;

        private void Start()
        {
            if (stealth != null)
                stealth.OnVisibilityChanged += UpdateVisibility;

            if (abilities != null)
                abilities.OnEnergyChanged += UpdateEnergy;

            if (abilities != null)
                abilities.OnAbilityUsed += ShowAbilityFeedback;

            if (health != null)
                health.OnHealthChanged += UpdateHealth;

            if (AlertSystem.Instance != null)
                AlertSystem.Instance.OnAlertLevelChanged += UpdateAlert;
        }

        private void OnDestroy()
        {
            if (stealth != null)
                stealth.OnVisibilityChanged -= UpdateVisibility;

            if (abilities != null)
            {
                abilities.OnEnergyChanged -= UpdateEnergy;
                abilities.OnAbilityUsed -= ShowAbilityFeedback;
            }

            if (health != null)
                health.OnHealthChanged -= UpdateHealth;

            if (AlertSystem.Instance != null)
                AlertSystem.Instance.OnAlertLevelChanged -= UpdateAlert;
        }

        private void Update()
        {
            if (shadowIndicator != null && stealth != null)
                shadowIndicator.color = stealth.IsInShadow
                    ? new Color(0.1f, 0.1f, 0.3f, 0.8f)
                    : new Color(0.8f, 0.8f, 0.2f, 0.3f);

            if (feedbackTimer > 0f)
            {
                feedbackTimer -= Time.deltaTime;
                if (feedbackTimer <= 0f && abilityFeedbackText != null)
                    abilityFeedbackText.text = "";
            }
        }

        private void UpdateVisibility(float visibility)
        {
            if (visibilityBar != null)
                visibilityBar.value = visibility;
        }

        private void UpdateEnergy(float normalized)
        {
            if (energyBar != null)
                energyBar.value = normalized;
        }

        private void UpdateHealth(float normalized)
        {
            if (healthBar != null)
                healthBar.value = normalized;
        }

        private void UpdateAlert(float alertLevel)
        {
            if (alertBar != null)
                alertBar.value = alertLevel;
        }

        private void ShowAbilityFeedback(string abilityName)
        {
            if (abilityFeedbackText != null)
                abilityFeedbackText.text = abilityName;

            feedbackTimer = feedbackDuration;
        }
    }
}
