using UnityEngine;

namespace GraveSilence.Environment
{
    /// <summary>
    /// Destructible or interactable object that emits noise when disturbed.
    /// Zombies will investigate the sound.
    /// </summary>
    public class NoiseTrap : MonoBehaviour
    {
        [SerializeField] private float noiseIntensity = 0.8f;
        [SerializeField] private Systems.NoiseType noiseType = Systems.NoiseType.ObjectBreak;
        [SerializeField] private bool oneShot = true;
        [SerializeField] private bool triggerOnCollision = true;

        private bool triggered;

        private void OnCollisionEnter(Collision collision)
        {
            if (!triggerOnCollision || triggered) return;
            if (collision.relativeVelocity.magnitude < 2f) return;

            TriggerNoise();
        }

        public void TriggerNoise()
        {
            if (oneShot && triggered) return;
            triggered = true;

            Systems.NoiseSystem.Instance?.EmitNoise(transform.position, noiseIntensity, noiseType);
        }
    }
}
