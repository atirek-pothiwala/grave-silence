using System.Collections.Generic;
using UnityEngine;
using GraveSilence.Enemies;

namespace GraveSilence.Systems
{
    /// <summary>
    /// Manages horde alert levels. When zombies detect the player, nearby zombies are alerted.
    /// </summary>
    public class AlertSystem : MonoBehaviour
    {
        public static AlertSystem Instance { get; private set; }

        [SerializeField] private float alertPropagationRadius = 20f;
        [SerializeField] private float alertDecayRate = 0.1f;

        private float globalAlertLevel;
        private readonly List<ZombieBase> registeredZombies = new();

        public float GlobalAlertLevel => globalAlertLevel;
        public bool IsHordeAlerted => globalAlertLevel > 0.7f;

        public event System.Action<float> OnAlertLevelChanged;
        public event System.Action OnHordeAlerted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            globalAlertLevel = Mathf.Max(0f, globalAlertLevel - alertDecayRate * Time.deltaTime);
            OnAlertLevelChanged?.Invoke(globalAlertLevel);
        }

        public void RegisterZombie(ZombieBase zombie)
        {
            if (!registeredZombies.Contains(zombie))
                registeredZombies.Add(zombie);
        }

        public void UnregisterZombie(ZombieBase zombie)
        {
            registeredZombies.Remove(zombie);
        }

        public void RaiseAlert(Vector3 origin, float amount, ZombieBase source = null)
        {
            globalAlertLevel = Mathf.Clamp01(globalAlertLevel + amount);
            OnAlertLevelChanged?.Invoke(globalAlertLevel);

            if (globalAlertLevel > 0.7f)
                OnHordeAlerted?.Invoke();

            PropagateAlert(origin, amount, source);
        }

        private void PropagateAlert(Vector3 origin, float amount, ZombieBase source)
        {
            foreach (var zombie in registeredZombies)
            {
                if (zombie == source || zombie == null) continue;

                float distance = Vector3.Distance(origin, zombie.transform.position);
                if (distance <= alertPropagationRadius)
                {
                    float falloff = 1f - (distance / alertPropagationRadius);
                    zombie.ReceiveAlert(origin, amount * falloff);
                }
            }
        }
    }
}
