using System.Collections.Generic;
using UnityEngine;

namespace GraveSilence.Core
{
    /// <summary>
    /// Tracks mission objective progress and notifies listeners on completion.
    /// </summary>
    public class ObjectiveTracker : MonoBehaviour
    {
        public static ObjectiveTracker Instance { get; private set; }

        private readonly Dictionary<string, int> progress = new();
        private MissionData mission;
        private bool allRequiredComplete;

        public bool AllRequiredObjectivesComplete => allRequiredComplete;
        public MissionData Mission => mission;

        public event System.Action<MissionObjective> OnObjectiveUpdated;
        public event System.Action OnAllRequiredComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnMissionStarted += HandleMissionStarted;
        }

        private void Start()
        {
            if (GameManager.Instance?.CurrentMission != null)
                Initialize(GameManager.Instance.CurrentMission);
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnMissionStarted -= HandleMissionStarted;
        }

        private void HandleMissionStarted()
        {
            if (GameManager.Instance?.CurrentMission != null)
                Initialize(GameManager.Instance.CurrentMission);
        }

        public void Initialize(MissionData missionData)
        {
            mission = missionData;
            progress.Clear();
            allRequiredComplete = false;

            if (mission?.objectives == null) return;

            foreach (var obj in mission.objectives)
                progress[obj.objectiveId] = 0;

            EvaluateCompletion();
        }

        public void ReportProgress(string objectiveId, int amount = 1)
        {
            if (!progress.ContainsKey(objectiveId)) return;

            progress[objectiveId] += amount;
            var objective = FindObjective(objectiveId);
            if (objective != null)
                OnObjectiveUpdated?.Invoke(objective);

            EvaluateCompletion();
        }

        public int GetProgress(string objectiveId)
        {
            return progress.TryGetValue(objectiveId, out int value) ? value : 0;
        }

        public bool IsObjectiveComplete(MissionObjective objective)
        {
            if (objective == null) return true;
            return GetProgress(objective.objectiveId) >= objective.requiredCount;
        }

        private void EvaluateCompletion()
        {
            if (mission?.objectives == null)
            {
                allRequiredComplete = true;
                return;
            }

            bool complete = true;
            foreach (var obj in mission.objectives)
            {
                if (obj.optional) continue;
                if (!IsObjectiveComplete(obj))
                {
                    complete = false;
                    break;
                }
            }

            if (complete && !allRequiredComplete)
            {
                allRequiredComplete = true;
                OnAllRequiredComplete?.Invoke();
            }
            else if (!complete)
            {
                allRequiredComplete = false;
            }
        }

        private MissionObjective FindObjective(string objectiveId)
        {
            if (mission?.objectives == null) return null;
            foreach (var obj in mission.objectives)
            {
                if (obj.objectiveId == objectiveId)
                    return obj;
            }
            return null;
        }
    }
}
