using UnityEngine;
using GraveSilence.Core;
using GraveSilence.Enemies;

namespace GraveSilence.Environment
{
    /// <summary>
    /// Reports objective progress when attached zombies are eliminated.
    /// </summary>
    public class ObjectiveKillZone : MonoBehaviour
    {
        [SerializeField] private string objectiveId;
        [SerializeField] private ZombieBase[] linkedZombies;

        private int eliminated;

        private void OnEnable()
        {
            if (linkedZombies == null) return;
            foreach (var zombie in linkedZombies)
            {
                if (zombie != null)
                    zombie.OnDeath += HandleZombieDeath;
            }
        }

        private void OnDisable()
        {
            if (linkedZombies == null) return;
            foreach (var zombie in linkedZombies)
            {
                if (zombie != null)
                    zombie.OnDeath -= HandleZombieDeath;
            }
        }

        private void HandleZombieDeath(ZombieBase zombie)
        {
            eliminated++;
            ObjectiveTracker.Instance?.ReportProgress(objectiveId);
        }
    }
}
