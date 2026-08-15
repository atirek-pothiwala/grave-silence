using UnityEngine;

namespace GraveSilence.Environment
{
    /// <summary>
    /// Marks areas where the player can use umbral abilities and gain stealth bonuses.
    /// Place on trigger volumes in dark alleys, ruins, and underground areas.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ShadowZone : MonoBehaviour
    {
        [SerializeField] private bool allowsUmbralStep = true;
        [SerializeField] private bool allowsUmbralCloak = true;
        [SerializeField] private float energyRegenBonus = 2f;

        public bool AllowsUmbralStep => allowsUmbralStep;
        public bool AllowsUmbralCloak => allowsUmbralCloak;
        public float EnergyRegenBonus => energyRegenBonus;

        private void Awake()
        {
            gameObject.tag = "ShadowZone";
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.1f, 0.05f, 0.2f, 0.3f);
            var col = GetComponent<Collider>();
            if (col is BoxCollider box)
                Gizmos.DrawCube(transform.position + box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
        }
    }
}
