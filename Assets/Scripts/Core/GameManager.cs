using UnityEngine;

namespace GraveSilence.Core
{
    /// <summary>
    /// Central game state manager. Handles mission flow, pause, and global events.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private MissionData currentMission;
        [SerializeField] private bool pauseOnStart;

        public MissionData CurrentMission => currentMission;
        public bool IsPaused { get; private set; }
        public bool MissionComplete { get; private set; }
        public bool MissionFailed { get; private set; }

        public event System.Action OnMissionStarted;
        public event System.Action OnMissionCompleted;
        public event System.Action OnMissionFailed;
        public event System.Action<bool> OnPauseChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (currentMission != null)
                StartMission(currentMission);

            if (pauseOnStart)
                SetPaused(true);
        }

        public void StartMission(MissionData mission)
        {
            currentMission = mission;
            MissionComplete = false;
            MissionFailed = false;
            OnMissionStarted?.Invoke();
        }

        public void CompleteMission()
        {
            if (MissionComplete || MissionFailed) return;
            MissionComplete = true;
            OnMissionCompleted?.Invoke();
        }

        public void FailMission(string reason = "")
        {
            if (MissionComplete || MissionFailed) return;
            MissionFailed = true;
            Debug.Log($"Mission failed: {reason}");
            OnMissionFailed?.Invoke();
        }

        public void SetPaused(bool paused)
        {
            if (IsPaused == paused) return;
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            OnPauseChanged?.Invoke(paused);
        }

        public void TogglePause() => SetPaused(!IsPaused);
    }
}
