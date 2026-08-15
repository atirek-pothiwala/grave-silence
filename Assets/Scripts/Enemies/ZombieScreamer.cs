using UnityEngine;

namespace GraveSilence.Enemies
{
    /// <summary>
    /// Screams when detecting the player, alerting all nearby zombies.
    /// Priority target in stealth missions.
    /// </summary>
    public class ZombieScreamer : ZombieBase
    {
        [SerializeField] private float screamCooldown = 10f;
        private float screamTimer;

        protected override void Awake()
        {
            base.Awake();
            zombieType = ZombieType.Screamer;
            walkSpeed = 1.2f;
            chaseSpeed = 3.5f;
            sightRange = 14f;
            maxHealth = 30f;
        }

        protected override void Update()
        {
            base.Update();
            screamTimer = Mathf.Max(0f, screamTimer - Time.deltaTime);
        }

        protected override void OnPlayerDetected()
        {
            if (screamTimer <= 0f)
            {
                screamTimer = screamCooldown;
            }
            base.OnPlayerDetected();
        }
    }
}
