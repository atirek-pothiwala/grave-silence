using UnityEngine;

namespace GraveSilence.Enemies
{
    /// <summary>
    /// Visual feedback component for zombie awareness (eye glow, question marks, etc.).
    /// Attach alongside ZombieBase and drive UI indicators from awareness level.
    /// </summary>
    public class ZombieDetection : MonoBehaviour
    {
        [SerializeField] private ZombieBase zombie;
        [SerializeField] private Renderer eyeRenderer;
        [SerializeField] private Color calmColor = new(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color suspiciousColor = Color.yellow;
        [SerializeField] private Color alertColor = Color.red;
        [SerializeField] private GameObject investigateIcon;
        [SerializeField] private GameObject alertIcon;

        private Material eyeMaterial;

        private void Awake()
        {
            zombie ??= GetComponent<ZombieBase>();
            if (eyeRenderer != null)
            {
                eyeMaterial = eyeRenderer.material;
            }
        }

        private void Update()
        {
            if (zombie == null) return;

            float awareness = zombie.Awareness;
            UpdateEyeColor(awareness);
            UpdateIcons(zombie.CurrentState, awareness);
        }

        private void UpdateEyeColor(float awareness)
        {
            if (eyeMaterial == null) return;

            Color target = awareness < 0.3f ? calmColor
                : awareness < 0.7f ? suspiciousColor
                : alertColor;

            eyeMaterial.SetColor("_EmissionColor", target * awareness);
        }

        private void UpdateIcons(ZombieState state, float awareness)
        {
            if (investigateIcon != null)
                investigateIcon.SetActive(state == ZombieState.Investigate);

            if (alertIcon != null)
                alertIcon.SetActive(awareness > 0.7f || state == ZombieState.Chase);
        }
    }
}
