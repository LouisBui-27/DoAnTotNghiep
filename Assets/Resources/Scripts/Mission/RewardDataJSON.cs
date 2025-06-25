using System;
using System.Collections.Generic;

[Serializable]
public class RewardDataJSON
{
    public string type;
    public string name;
    public int amount;
}
[Serializable]
public class MissionDataJSON
{
    public int id;
    public string description;
    public int targetProgress;
    public List<RewardDataJSON> rewards;
}

[Serializable]
public class MissionList
{
    public List<MissionDataJSON> missions;
}