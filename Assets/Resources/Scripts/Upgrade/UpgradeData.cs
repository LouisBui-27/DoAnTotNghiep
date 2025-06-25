using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public int level;
    public int maxLevel;
    public float baseValue;
    public List<float> valuePerLevel;
    public List<int> levelCosts;
    //public string iconName;
    //[System.NonSerialized] public Sprite icon;
}