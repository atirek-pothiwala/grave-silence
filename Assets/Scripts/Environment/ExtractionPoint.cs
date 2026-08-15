using UnityEngine;
using GraveSilence.Core;

namespace GraveSilence.Environment
{
    /// <summary>
    /// Mission extraction point. Completes the mission when the player enters.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ExtractionPoint : MonoBehaviour
    {
        [SerializeField] private bool requiresAllObjectives = true;
        [SerializeField] private GameObject activeIndicator;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (requiresAllObjectives && !AreObjectivesComplete())
                return;

            GameManager.Instance?.CompleteMission();
        }

        private bool AreObjectivesComplete()
        {
            // Extend with ObjectiveTracker when mission objectives are wired up.
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}
