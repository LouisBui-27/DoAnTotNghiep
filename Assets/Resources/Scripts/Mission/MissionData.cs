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
    public string category; // <- th�m d�ng n�y
    public List<RewardData> rewards;
}
