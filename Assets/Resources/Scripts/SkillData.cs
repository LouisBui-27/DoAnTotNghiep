using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill System/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public string description;
    public SkillType skillType;
    public SkillCategory skillCategory;
    //[HideInInspector] public int currentLevel = 0;
    public int maxLevel = 5;
}
[Serializable]
public enum SkillCategory
{
    Attack,
    Support
}
public enum SkillType
{
    IncreaseBulletCount,
    IncreaseHealth,
    Boomerang,
    ArrowShoot,
    RocketShoot,
    Heal,
    Lightning,
    Pet, 
    Aura, 
    shootBullet,
    OrbitingWeapon,
    SpeedBoost,
    Whip,
    IncreasePickupRange,
    IncreaseCritChance,
    Tornado
}
