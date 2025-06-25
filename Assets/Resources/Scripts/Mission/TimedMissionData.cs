using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class TimedMissionData : MonoBehaviour
{
 [System.Serializable]
    public struct TimedMission
    {
        public string missionId;
        public float requiredTimeInSeconds;
        public float currentTime;
        public bool completed;
    }
    public List<TimedMission> timedMissions;
    private const string LastResetDateKey = "LastResetDate";
    private void Awake()
    {
        LoadMissionProgress();
        CheckAndResetDailyMissions();
    }
    private void Update()
    {
        if (MissionManager.Instance == null) return;
        for (int i = 0; i < timedMissions.Count; i++)
        {
            TimedMission currentMission = timedMissions[i];
            if (!currentMission.completed)
            {
                currentMission.currentTime += Time.deltaTime;
                // Cập nhật tiến trình (chỉ báo cáo thời gian đã trôi qua)
                int currentProgress = Mathf.FloorToInt(currentMission.currentTime / 60);
                MissionManager.Instance.UpdateMissionProgress(currentMission.missionId, currentProgress);
                if (currentProgress >= currentMission.requiredTimeInSeconds)
                {
                    Debug.Log(currentProgress);

                    currentMission.completed = true;

                    // Báo cáo tiến trình hoàn thành (có thể đặt bằng với requiredProgress trong JSON)

                    MissionManager.Instance.UpdateMissionProgress(currentMission.missionId, Mathf.FloorToInt(currentMission.requiredTimeInSeconds / 60));

                }
                timedMissions[i] = currentMission; // Gán bản sao đã sửa đổi trở lại list

            }

        }

    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveMissionProgress();
    }

    private void OnApplicationQuit()
    {
        SaveMissionProgress();
    }
    private void LoadMissionProgress()
    {
        for (int i = 0; i < timedMissions.Count; i++)
        {
            string key = $"TimedMission_{timedMissions[i].missionId}";
            if (PlayerPrefs.HasKey(key + "_Time"))
            {
                TimedMission mission = timedMissions[i];
                mission.currentTime = PlayerPrefs.GetFloat(key + "_Time");
                mission.completed = PlayerPrefs.GetInt(key + "_Completed") == 1;
                timedMissions[i] = mission;
            }
        }
    }
    private void ResetAllMissions()
    {
        for (int i = 0; i < timedMissions.Count; i++)
        {
            TimedMission mission = timedMissions[i];
            mission.currentTime = 0f;
            mission.completed = false;
            timedMissions[i] = mission;
        }
        SaveMissionProgress();
    }
    private void SaveMissionProgress()
    {
        foreach (var mission in timedMissions)
        {
            string key = $"TimedMission_{mission.missionId}";
            PlayerPrefs.SetFloat(key + "_Time", mission.currentTime);
            PlayerPrefs.SetInt(key + "_Completed", mission.completed ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
    private void CheckAndResetDailyMissions()
    {
        string lastResetDate = PlayerPrefs.GetString(LastResetDateKey, "");
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd");

        if (lastResetDate != currentDate)
        {
            ResetAllMissions();
            PlayerPrefs.SetString(LastResetDateKey, currentDate);
            PlayerPrefs.Save();
        }
    }
}