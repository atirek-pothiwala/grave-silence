using System.Collections.Generic;
using UnityEngine;

namespace GraveSilence.Systems
{
    /// <summary>
    /// Global noise propagation system. Zombies react to sounds emitted here.
    /// </summary>
    public class NoiseSystem : MonoBehaviour
    {
        public static NoiseSystem Instance { get; private set; }

        [SerializeField] private float noiseLifetime = 3f;

        private readonly List<NoiseEvent> activeNoises = new();

        public event System.Action<NoiseEvent> OnNoiseEmitted;

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
            for (int i = activeNoises.Count - 1; i >= 0; i--)
            {
                if (Time.time - activeNoises[i].timestamp > noiseLifetime)
                    activeNoises.RemoveAt(i);
            }
        }

        public void EmitNoise(Vector3 position, float intensity, NoiseType type)
        {
            var noise = new NoiseEvent
            {
                position = position,
                intensity = intensity,
                type = type,
                timestamp = Time.time
            };

            activeNoises.Add(noise);
            OnNoiseEmitted?.Invoke(noise);
        }

        public NoiseEvent? GetLoudestNoiseNear(Vector3 position, float hearingRange)
        {
            NoiseEvent? loudest = null;
            float maxIntensity = 0f;

            foreach (var noise in activeNoises)
            {
                float distance = Vector3.Distance(position, noise.position);
                if (distance > hearingRange) continue;

                float falloff = 1f - (distance / hearingRange);
                float effectiveIntensity = noise.intensity * falloff;

                if (effectiveIntensity > maxIntensity)
                {
                    maxIntensity = effectiveIntensity;
                    loudest = noise;
                }
            }

            return loudest;
        }
    }

    public struct NoiseEvent
    {
        public Vector3 position;
        public float intensity;
        public NoiseType type;
        public float timestamp;
    }
}
