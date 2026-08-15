using UnityEngine;

namespace GraveSilence.Core
{
    /// <summary>
    /// Tracks Aragami-style end-of-mission medals: Ghost, Silent, and Speed.
    /// </summary>
    public class MissionScore : MonoBehaviour
    {
        public static MissionScore Instance { get; private set; }

        private int silentKills;
        private int alertedEnemies;
        private int totalKills;
        private float missionStartTime;
        private bool anyEnemyFullyAlerted;

        public int SilentKills => silentKills;
        public int TotalKills => totalKills;
        public bool IsGhostRun => !anyEnemyFullyAlerted && alertedEnemies == 0;
        public bool IsSilentRun => totalKills == 0 || silentKills == totalKills;

        public event System.Action OnScoreChanged;

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
            {
                GameManager.Instance.OnMissionStarted += ResetScore;
                GameManager.Instance.OnMissionCompleted += LogFinalScore;
            }

            if (Systems.AlertSystem.Instance != null)
                Systems.AlertSystem.Instance.OnHordeAlerted += HandleHordeAlerted;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMissionStarted -= ResetScore;
                GameManager.Instance.OnMissionCompleted -= LogFinalScore;
            }

            if (Systems.AlertSystem.Instance != null)
                Systems.AlertSystem.Instance.OnHordeAlerted -= HandleHordeAlerted;
        }

        public void RegisterSilentKill()
        {
            silentKills++;
            totalKills++;
            OnScoreChanged?.Invoke();
        }

        public void RegisterKill(bool wasSilent)
        {
            totalKills++;
            if (wasSilent) silentKills++;
            OnScoreChanged?.Invoke();
        }

        public void RegisterEnemyAlerted()
        {
            alertedEnemies++;
            OnScoreChanged?.Invoke();
        }

        public float GetElapsedTime() => Time.time - missionStartTime;

        public bool IsUnderParTime()
        {
            var mission = GameManager.Instance?.CurrentMission;
            if (mission == null || mission.timeLimitSeconds <= 0f) return true;
            return GetElapsedTime() <= mission.timeLimitSeconds;
        }

        private void ResetScore()
        {
            silentKills = 0;
            alertedEnemies = 0;
            totalKills = 0;
            anyEnemyFullyAlerted = false;
            missionStartTime = Time.time;
            OnScoreChanged?.Invoke();
        }

        private void HandleHordeAlerted() => anyEnemyFullyAlerted = true;

        private void LogFinalScore()
        {
            Debug.Log($"[Mission Score] Ghost: {IsGhostRun} | Silent: {IsSilentRun} | " +
                      $"Time: {GetElapsedTime():F1}s | Kills: {totalKills} ({silentKills} silent)");
        }
    }
}
