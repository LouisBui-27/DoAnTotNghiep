using System.Collections.Generic;

[System.Serializable]
public class MatchRecord
{
    public int characterID;
    public string characterName;
    public string levelName;
    public string playTime;
    public int levelPlayer;
    public int goldEarned;
    public bool isWin;
    public string dateTime;
    public List<SkillInfo> equippedSkills;
}

[System.Serializable]
public class SkillInfo
{
    public string skillName;
    public int level;
    public SkillCategory skillCategory; // Attack / Support
}