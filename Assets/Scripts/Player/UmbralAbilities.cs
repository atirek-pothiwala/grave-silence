using UnityEngine;
using GraveSilence.Core;
using GraveSilence.Enemies;
using GraveSilence.Environment;
using GraveSilence.Systems;

namespace GraveSilence.Player
{
    /// <summary>
    /// Umbral powers — the Grave Silence equivalent of Aragami 2's shadow arts.
    /// Teleport through darkness, cloak in shadows, lure zombies, and execute silent kills.
    /// </summary>
    public class UmbralAbilities : MonoBehaviour
    {
        [Header("Umbral Step (Teleport)")]
        [SerializeField] private float stepRange = 12f;
        [SerializeField] private float stepCooldown = 3f;
        [SerializeField] private LayerMask aimMask = ~0;
        [SerializeField] private GameObject stepVfxPrefab;

        [Header("Umbral Cloak (Invisibility)")]
        [SerializeField] private float cloakDuration = 5f;
        [SerializeField] private float cloakCooldown = 15f;

        [Header("Umbral Lure (Shadow Decoy)")]
        [SerializeField] private float lureRange = 15f;
        [SerializeField] private float lureDuration = 8f;
        [SerializeField] private float lureCooldown = 10f;
        [SerializeField] private GameObject lureDecoyPrefab;

        [Header("Umbral Strike (Shadow Assassination)")]
        [SerializeField] private float strikeRange = 2.5f;
        [SerializeField] private float strikeCooldown = 1f;

        [Header("Resource")]
        [SerializeField] private float maxUmbralEnergy = 100f;
        [SerializeField] private float energyRegenRate = 5f;
        [SerializeField] private float stepEnergyCost = 20f;
        [SerializeField] private float cloakEnergyCost = 35f;
        [SerializeField] private float lureEnergyCost = 25f;
        [SerializeField] private float strikeEnergyCost = 15f;

        private StealthController stealth;
        private CharacterController controller;
        private UnityEngine.Camera aimCamera;
        private float umbralEnergy;
        private float stepTimer;
        private float cloakTimer;
        private float lureTimer;
        private float strikeTimer;
        private float cloakEndTime;
        private GameObject activeLure;

        public float UmbralEnergy => umbralEnergy;
        public float UmbralEnergyNormalized => umbralEnergy / maxUmbralEnergy;
        public bool CanUseAbilities => umbralEnergy > 0f;
        public float StepCooldownRemaining => stepTimer;
        public float CloakCooldownRemaining => cloakTimer;
        public float LureCooldownRemaining => lureTimer;
        public float StrikeCooldownRemaining => strikeTimer;

        public event System.Action<float> OnEnergyChanged;
        public event System.Action<string> OnAbilityUsed;

        private void Awake()
        {
            stealth = GetComponent<StealthController>();
            controller = GetComponent<CharacterController>();
            aimCamera = UnityEngine.Camera.main;
            umbralEnergy = maxUmbralEnergy;

            if (stealth != null)
                stealth.OnCloakBroken += HandleCloakBroken;
        }

        private void OnDestroy()
        {
            if (stealth != null)
                stealth.OnCloakBroken -= HandleCloakBroken;
        }

        private void Update()
        {
            stepTimer = Mathf.Max(0f, stepTimer - Time.deltaTime);
            cloakTimer = Mathf.Max(0f, cloakTimer - Time.deltaTime);
            lureTimer = Mathf.Max(0f, lureTimer - Time.deltaTime);
            strikeTimer = Mathf.Max(0f, strikeTimer - Time.deltaTime);

            if (stealth != null && stealth.IsCloaked && Time.time >= cloakEndTime)
                stealth.SetCloaked(false);

            RegenerateEnergy();
        }

        public bool TryUmbralStep()
        {
            if (stepTimer > 0f || umbralEnergy < stepEnergyCost) return false;

            if (!AimHelper.TryGetAimHit(aimCamera, stepRange, aimMask, out RaycastHit hit))
                return false;

            bool validTarget = hit.collider.CompareTag(GameConstants.ShadowZoneTag)
                                 || hit.collider.CompareTag(GameConstants.GroundTag)
                                 || hit.collider.GetComponent<ShadowZone>() != null;

            if (!validTarget) return false;

            if (stepVfxPrefab != null)
                Instantiate(stepVfxPrefab, transform.position, Quaternion.identity);

            controller.enabled = false;
            transform.position = hit.point;
            controller.enabled = true;

            SpendEnergy(stepEnergyCost);
            stepTimer = stepCooldown;
            OnAbilityUsed?.Invoke("Umbral Step");
            return true;
        }

        public bool TryUmbralCloak()
        {
            if (cloakTimer > 0f || umbralEnergy < cloakEnergyCost) return false;
            if (stealth == null || !stealth.IsInShadow) return false;

            stealth.SetCloaked(true);
            cloakEndTime = Time.time + cloakDuration;
            SpendEnergy(cloakEnergyCost);
            cloakTimer = cloakCooldown;
            OnAbilityUsed?.Invoke("Umbral Cloak");
            return true;
        }

        public bool TryUmbralLure()
        {
            if (lureTimer > 0f || umbralEnergy < lureEnergyCost) return false;

            Vector3 lurePoint = AimHelper.TryGetAimPoint(aimCamera, lureRange, aimMask, out Vector3 point)
                ? point
                : transform.position + transform.forward * 5f;

            if (activeLure != null) Destroy(activeLure);

            if (lureDecoyPrefab != null)
            {
                activeLure = Instantiate(lureDecoyPrefab, lurePoint, Quaternion.identity);
                Destroy(activeLure, lureDuration);
            }

            NoiseSystem.Instance?.EmitNoise(lurePoint, 1f, NoiseType.Lure);
            SpendEnergy(lureEnergyCost);
            lureTimer = lureCooldown;
            OnAbilityUsed?.Invoke("Umbral Lure");
            return true;
        }

        public bool TryUmbralStrike()
        {
            if (strikeTimer > 0f || umbralEnergy < strikeEnergyCost) return false;
            if (stealth == null || (!stealth.IsInShadow && !stealth.IsCloaked)) return false;

            ZombieBase target = FindStrikeTarget();
            if (target == null) return false;

            target.ExecuteStealthKill(transform);
            MissionScore.Instance?.RegisterSilentKill();
            SpendEnergy(strikeEnergyCost);
            strikeTimer = strikeCooldown;
            OnAbilityUsed?.Invoke("Umbral Strike");
            return true;
        }

        private ZombieBase FindStrikeTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, strikeRange);
            ZombieBase closest = null;
            float closestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var zombie = hit.GetComponent<ZombieBase>();
                if (zombie == null || !zombie.CanBeStealthKilled) continue;

                float dist = Vector3.Distance(transform.position, zombie.transform.position);
                if (dist < closestDist)
                {
                    closest = zombie;
                    closestDist = dist;
                }
            }

            return closest;
        }

        private void HandleCloakBroken()
        {
            cloakEndTime = 0f;
        }

        public bool TrySpendEnergy(float amount)
        {
            if (umbralEnergy < amount) return false;
            SpendEnergy(amount);
            return true;
        }

        private void SpendEnergy(float amount)
        {
            umbralEnergy = Mathf.Max(0f, umbralEnergy - amount);
            OnEnergyChanged?.Invoke(UmbralEnergyNormalized);
        }

        private void RegenerateEnergy()
        {
            if (umbralEnergy >= maxUmbralEnergy) return;

            float regenMultiplier = stealth != null && stealth.IsInShadow ? 2f : 1f;
            umbralEnergy = Mathf.Min(maxUmbralEnergy,
                umbralEnergy + energyRegenRate * regenMultiplier * Time.deltaTime);
            OnEnergyChanged?.Invoke(UmbralEnergyNormalized);
        }
    }
}
