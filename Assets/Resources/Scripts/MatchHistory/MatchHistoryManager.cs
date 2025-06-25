using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MatchHistoryManager : MonoBehaviour
{
    public static MatchHistoryManager Instance;

    private string saveFilePath;
    public List<MatchRecord> matchRecords = new List<MatchRecord>();
    private int maxHistory = 20;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "match_history.json");
            LoadHistory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddRecord(MatchRecord record)
    {
        matchRecords.Insert(0, record);
        if (matchRecords.Count > maxHistory)
            matchRecords.RemoveAt(matchRecords.Count - 1);

        SaveHistory();
    }

    public void SaveHistory()
    {
        MatchHistoryWrapper wrapper = new MatchHistoryWrapper { records = matchRecords };
        string json = JsonUtility.ToJson(wrapper, true); // `true` để định dạng dễ đọc

        File.WriteAllText(saveFilePath, json);
    }

    public void LoadHistory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            MatchHistoryWrapper wrapper = JsonUtility.FromJson<MatchHistoryWrapper>(json);
            matchRecords = wrapper.records ?? new List<MatchRecord>();
        }
        else
        {
            matchRecords = new List<MatchRecord>();
        }
    }

    [System.Serializable]
    private class MatchHistoryWrapper
    {
        public List<MatchRecord> records;
    }
}
