using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SkillRuntimeData
{
    public SkillData baseSkill;
    public int currentLevel;

    public SkillRuntimeData(SkillData data)
    {
        baseSkill = data;
        currentLevel = 1;
    }

    public bool IsMaxLevel()
    {
        return currentLevel >= baseSkill.maxLevel;
    }
}
    