using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelUnlock
{
    private const string HIGHEST_LEVEL_COMPLETED_KEY = "HighestLevelCompleted";
    private const int DEFAULT_UNLOCKED_LEVEL = 1; // Level 1 luôn mở khóa

    public static void Initialize()
    {
        // Nếu chưa có key, đặt giá trị ban đầu là 0 (chưa hoàn thành level nào)
        if (!PlayerPrefs.HasKey(HIGHEST_LEVEL_COMPLETED_KEY))
        {
            PlayerPrefs.SetInt(HIGHEST_LEVEL_COMPLETED_KEY, 0);
            PlayerPrefs.Save();
        }
        Debug.Log($"LevelUnlockManager initialized. Highest level completed: {GetHighestLevelCompleted()}");
    }
    public static int GetHighestLevelCompleted()
    {
        return PlayerPrefs.GetInt(HIGHEST_LEVEL_COMPLETED_KEY, 0);
    }

    public static void MarkLevelCompleted(int levelID)
    {
        int currentHighest = GetHighestLevelCompleted();
        if (levelID > currentHighest)
        {
            PlayerPrefs.SetInt(HIGHEST_LEVEL_COMPLETED_KEY, levelID);
            PlayerPrefs.Save();
            Debug.Log($"Level {levelID} completed! New highest completed level: {levelID}");
        }
        else
        {
            Debug.Log($"Level {levelID} completed, but not a new highest. Highest remains {currentHighest}.");
        }
    }

    public static bool IsLevelUnlocked(int levelID)
    {
        if (levelID <= DEFAULT_UNLOCKED_LEVEL) // Level 1 luôn mở khóa
        {
            return true;
        }

        // Level N được mở khóa nếu Level N-1 đã hoàn thành
        return GetHighestLevelCompleted() >= (levelID - 1);
    }
    public static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt(HIGHEST_LEVEL_COMPLETED_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log("All level progress reset.");
    }
}
