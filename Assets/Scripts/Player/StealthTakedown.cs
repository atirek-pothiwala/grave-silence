using UnityEngine;
using GraveSilence.Core;
using GraveSilence.Enemies;

namespace GraveSilence.Player
{
    /// <summary>
    /// Handles close-range stealth takedowns on unaware zombies.
    /// </summary>
    public class StealthTakedown : MonoBehaviour
    {
        [SerializeField] private float takedownRange = 2f;
        [SerializeField] private float takedownAngle = 60f;
        [SerializeField] private StealthController stealth;

        private void Awake()
        {
            stealth ??= GetComponent<StealthController>();
        }

        public bool TryTakedown()
        {
            ZombieBase target = FindTakedownTarget();
            if (target == null) return false;

            target.ExecuteStealthKill(transform);
            MissionScore.Instance?.RegisterSilentKill();
            return true;
        }

        private ZombieBase FindTakedownTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, takedownRange);
            ZombieBase best = null;
            float bestAngle = float.MaxValue;

            foreach (var hit in hits)
            {
                var zombie = hit.GetComponent<ZombieBase>();
                if (zombie == null || !zombie.CanBeStealthKilled) continue;

                Vector3 toZombie = (zombie.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, toZombie);

                if (angle < takedownAngle && angle < bestAngle)
                {
                    best = zombie;
                    bestAngle = angle;
                }
            }

            return best;
        }
    }
}
