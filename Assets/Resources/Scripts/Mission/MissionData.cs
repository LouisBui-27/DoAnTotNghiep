using System.Collections.Generic;
[System.Serializable]
public class MissionData
{
    public string id;
    public string title;
    public int currentProgress;
    public int requiredProgress;
    public bool isCompleted;
    public bool isClaimed;
    public string category; // <- thêm dòng này
    public List<RewardData> rewards;
}
