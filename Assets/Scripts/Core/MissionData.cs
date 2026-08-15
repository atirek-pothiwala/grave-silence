using UnityEngine;

namespace GraveSilence.Core
{
    [CreateAssetMenu(fileName = "NewMission", menuName = "Grave Silence/Mission Data")]
    public class MissionData : ScriptableObject
    {
        [Header("Mission Info")]
        public string missionId;
        public string missionTitle;
        [TextArea(3, 6)] public string briefing;

        [Header("Objectives")]
        public MissionObjective[] objectives;

        [Header("Scoring")]
        public int ghostBonus = 500;
        public int alertPenalty = 100;
        public float timeLimitSeconds;

        [Header("Scene")]
        public string sceneName;
    }

    [System.Serializable]
    public class MissionObjective
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public string targetTag;
        public int requiredCount = 1;
        public bool optional;
    }

    public enum ObjectiveType
    {
        ReachExtraction,
        EliminateTarget,
        RescueSurvivor,
        RetrieveItem,
        AvoidDetection,
        SilentTakedowns
    }
}
