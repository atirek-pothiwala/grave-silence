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

        private bool playerInside;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnEnable()
        {
            if (ObjectiveTracker.Instance != null)
                ObjectiveTracker.Instance.OnAllRequiredComplete += ShowIndicator;
        }

        private void OnDisable()
        {
            if (ObjectiveTracker.Instance != null)
                ObjectiveTracker.Instance.OnAllRequiredComplete -= ShowIndicator;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameConstants.PlayerTag)) return;
            playerInside = true;
            TryExtract();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameConstants.PlayerTag))
                playerInside = false;
        }

        private void ShowIndicator()
        {
            if (activeIndicator != null)
                activeIndicator.SetActive(true);
        }

        private void TryExtract()
        {
            if (!playerInside) return;

            if (requiresAllObjectives && ObjectiveTracker.Instance != null
                && !ObjectiveTracker.Instance.AllRequiredObjectivesComplete)
                return;

            GameManager.Instance?.CompleteMission();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}
