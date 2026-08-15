using UnityEngine;

namespace GraveSilence.Enemies
{
    /// <summary>
    /// Fast, aggressive zombie variant. Harder to outrun once alerted.
    /// </summary>
    public class ZombieRunner : ZombieBase
    {
        protected override void Awake()
        {
            base.Awake();
            zombieType = ZombieType.Runner;
            walkSpeed = 2.5f;
            chaseSpeed = 7f;
            sightRange = 15f;
            hearingRange = 10f;
            maxHealth = 35f;
        }
    }
}
