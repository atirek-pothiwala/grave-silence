using UnityEngine;
using UnityEngine.AI;
using GraveSilence.Player;
using GraveSilence.Systems;

namespace GraveSilence.Enemies
{
  public enum ZombieState
    {
        Idle,
        Patrol,
        Investigate,
        Chase,
        Attack,
        Stunned,
        Dead
    }

    public enum ZombieType
    {
        Shambler,   // Slow, low awareness — easy to sneak past
        Runner,     // Fast, moderate awareness
        Screamer,   // Alerts horde on detection
        Brute       // Heavy, hard to stealth kill
    }

    /// <summary>
    /// Base zombie AI with Aragami-style awareness states.
    /// Zombies patrol, investigate noise, chase, and attack.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieBase : MonoBehaviour
    {
        [Header("Type")]
        [SerializeField] protected ZombieType zombieType = ZombieType.Shambler;

        [Header("Stats")]
        [SerializeField] protected float maxHealth = 50f;
        [SerializeField] protected float walkSpeed = 1.5f;
        [SerializeField] protected float chaseSpeed = 4f;
        [SerializeField] protected float attackDamage = 15f;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float attackCooldown = 1.5f;

        [Header("Awareness")]
        [SerializeField] protected float sightRange = 12f;
        [SerializeField] protected float sightAngle = 90f;
        [SerializeField] protected float hearingRange = 8f;
        [SerializeField] protected float investigationDuration = 5f;
        [SerializeField] protected float awarenessDecayRate = 0.15f;
        [SerializeField] protected LayerMask sightObstacleMask;

        [Header("Patrol")]
        [SerializeField] protected Transform[] patrolPoints;
        [SerializeField] protected float patrolWaitTime = 2f;

        protected NavMeshAgent agent;
        protected ZombieDetection detection;
        protected Transform player;
        protected float health;
        protected float awareness;
        protected float attackTimer;
        protected float investigateTimer;
        protected float patrolWaitTimer;
        protected int patrolIndex;
        protected ZombieState currentState = ZombieState.Patrol;
        protected Vector3 lastKnownPlayerPosition;
        protected Vector3 investigateTarget;

        public ZombieState CurrentState => currentState;
        public float Awareness => awareness;
        public bool CanBeStealthKilled => currentState != ZombieState.Chase
                                          && currentState != ZombieState.Attack
                                          && currentState != ZombieState.Dead
                                          && zombieType != ZombieType.Brute
                                          && awareness < 0.3f;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            detection = GetComponent<ZombieDetection>();
            health = maxHealth;
            agent.speed = walkSpeed;
        }

        protected virtual void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;

            AlertSystem.Instance?.RegisterZombie(this);
        }

        protected virtual void OnDestroy()
        {
            AlertSystem.Instance?.UnregisterZombie(this);
        }

        protected virtual void Update()
        {
            if (currentState == ZombieState.Dead) return;

            attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime);
            UpdateAwareness();
            UpdateStateMachine();
        }

        protected virtual void UpdateAwareness()
        {
            if (player == null) return;

            float visibility = GetPlayerVisibility();
            float distance = Vector3.Distance(transform.position, player.position);

            if (CanSeePlayer(distance, visibility))
            {
                awareness = Mathf.Clamp01(awareness + visibility * Time.deltaTime * 2f);
                lastKnownPlayerPosition = player.position;

                if (awareness >= 1f && currentState != ZombieState.Chase)
                {
                    OnPlayerDetected();
                }
            }
            else
            {
                awareness = Mathf.Max(0f, awareness - awarenessDecayRate * Time.deltaTime);
            }

            CheckForNoise();
        }

        protected virtual bool CanSeePlayer(float distance, float visibility)
        {
            if (distance > sightRange || visibility < 0.05f) return false;

            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, direction);
            if (angle > sightAngle * 0.5f) return false;

            if (Physics.Raycast(transform.position + Vector3.up, direction, distance, sightObstacleMask))
                return false;

            return true;
        }

        protected virtual float GetPlayerVisibility()
        {
            if (player == null) return 0f;
            var stealth = player.GetComponent<StealthController>();
            return stealth != null ? stealth.Visibility : 1f;
        }

        protected virtual void CheckForNoise()
        {
            var noise = NoiseSystem.Instance?.GetLoudestNoiseNear(transform.position, hearingRange);
            if (noise == null) return;

            if (currentState == ZombieState.Idle || currentState == ZombieState.Patrol)
            {
                investigateTarget = noise.Value.position;
                SetState(ZombieState.Investigate);
                investigateTimer = investigationDuration;
            }
        }

        protected virtual void UpdateStateMachine()
        {
            switch (currentState)
            {
                case ZombieState.Patrol:
                    UpdatePatrol();
                    break;
                case ZombieState.Investigate:
                    UpdateInvestigate();
                    break;
                case ZombieState.Chase:
                    UpdateChase();
                    break;
                case ZombieState.Attack:
                    UpdateAttack();
                    break;
            }
        }

        protected virtual void UpdatePatrol()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                patrolWaitTimer -= Time.deltaTime;
                if (patrolWaitTimer <= 0f)
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[patrolIndex].position);
                    patrolWaitTimer = patrolWaitTime;
                }
            }
        }

        protected virtual void UpdateInvestigate()
        {
            agent.speed = walkSpeed;
            agent.SetDestination(investigateTarget);
            investigateTimer -= Time.deltaTime;

            if (investigateTimer <= 0f || Vector3.Distance(transform.position, investigateTarget) < 1f)
                SetState(ZombieState.Patrol);
        }

        protected virtual void UpdateChase()
        {
            agent.speed = chaseSpeed;
            if (player != null)
                agent.SetDestination(player.position);

            if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
                SetState(ZombieState.Attack);
        }

        protected virtual void UpdateAttack()
        {
            if (player == null) return;

            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            if (Vector3.Distance(transform.position, player.position) > attackRange * 1.5f)
            {
                SetState(ZombieState.Chase);
                return;
            }

            if (attackTimer <= 0f)
            {
                PerformAttack();
                attackTimer = attackCooldown;
            }
        }

        protected virtual void PerformAttack()
        {
            var playerHealth = player?.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(attackDamage);
        }

        protected virtual void OnPlayerDetected()
        {
            SetState(ZombieState.Chase);
            AlertSystem.Instance?.RaiseAlert(transform.position, 0.3f, this);

            if (zombieType == ZombieType.Screamer)
            {
                NoiseSystem.Instance?.EmitNoise(transform.position, 1f, NoiseType.Scream);
                AlertSystem.Instance?.RaiseAlert(transform.position, 0.5f, this);
            }
        }

        public virtual void ReceiveAlert(Vector3 origin, float amount)
        {
            awareness = Mathf.Clamp01(awareness + amount);
            investigateTarget = origin;
            if (currentState == ZombieState.Patrol || currentState == ZombieState.Idle)
            {
                SetState(ZombieState.Investigate);
                investigateTimer = investigationDuration;
            }
        }

        public virtual void ExecuteStealthKill(Transform killer)
        {
            if (!CanBeStealthKilled) return;
            Die();
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            awareness = 1f;
            if (health <= 0f)
                Die();
            else if (currentState != ZombieState.Chase)
                SetState(ZombieState.Chase);
        }

        protected virtual void Die()
        {
            currentState = ZombieState.Dead;
            agent.isStopped = true;
            agent.enabled = false;
            GetComponent<Collider>()?.gameObject.SetActive(false);
        }

        protected void SetState(ZombieState newState)
        {
            currentState = newState;
            agent.isStopped = newState == ZombieState.Dead;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, hearingRange);
        }
    }
}
