using UnityEngine;

[CreateAssetMenu(menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int characterID; // thêm ID
    public string characterName;
    public Sprite icon;
    public GameObject characterPrefab;
    public SkillType startingSkill; // Kỹ năng khởi đầu
    public int startingSkillLevel = 1; // Cấp độ khởi đầu
}
