using UnityEngine;

namespace GraveSilence.Enemies
{
    /// <summary>
    /// Heavy zombie that cannot be stealth-killed. High health and damage.
    /// </summary>
    public class ZombieBrute : ZombieBase
    {
        protected override void Awake()
        {
            base.Awake();
            zombieType = ZombieType.Brute;
            maxHealth = 120f;
            health = maxHealth;
            walkSpeed = 1f;
            chaseSpeed = 3f;
            attackDamage = 30f;
            attackRange = 2.5f;
            sightRange = 10f;
            hearingRange = 6f;
        }
    }
}
