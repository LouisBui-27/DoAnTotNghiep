using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchHistoryUI : MonoBehaviour
{
    public GameObject matchItemPrefab;
    public Transform contentPanel;
    public List<CharacterData> allCharacters;
    public GameObject historyPanel;
    public GameObject skillSlotPrefab;
    public List<SkillData> allSkills;
    public Sprite winIcon;
    public Sprite loseIcon;


    //private void OnEnable()
    //{
    //    LoadHistoryUI();
    //}
    private void Awake()
    {
        if(historyPanel != null)
        {
            historyPanel.SetActive(false);
        }
    }
    public void LoadHistoryUI()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        var records = MatchHistoryManager.Instance.matchRecords;

        foreach (var record in records)
        {
            GameObject item = Instantiate(matchItemPrefab, contentPanel);

            var icon = allCharacters.Find(c => c.characterID == record.characterID)?.icon;

            item.transform.Find("CharacterIcon").GetComponent<Image>().sprite = icon;
            item.transform.Find("TextLevelPlay").GetComponent<Text>().text = record.levelName;
           // item.transform.Find("PlayTime").GetComponent<Text>().text = record.playTime;
            item.transform.Find("TextGoldEarned").GetComponent<Text>().text = record.goldEarned.ToString();
            item.transform.Find("Image/TextLevel").GetComponent<Text>().text = record.levelPlayer.ToString();
            item.transform.Find("TextDateTime").GetComponent<Text>().text = record.dateTime;
            item.transform.Find("ResultIcon").GetComponent<Image>().sprite = record.isWin ? winIcon :loseIcon;

            Transform attackContainer = item.transform.Find("AttackSkillsContainer");
            Transform supportContainer = item.transform.Find("SupportSkillsContainer");

            foreach (Transform child in attackContainer) Destroy(child.gameObject);
            foreach (Transform child in supportContainer) Destroy(child.gameObject);

            foreach (var skill in record.equippedSkills)
            {
                Transform parent = skill.skillCategory == SkillCategory.Attack ? attackContainer : supportContainer;

                GameObject skillObj = Instantiate(skillSlotPrefab, parent);

                // Lấy dữ liệu kỹ năng từ ScriptableObject
                SkillData skillData = GetSkillData(skill.skillName);

                // Gán icon nếu tìm được
                Image iconImg = skillObj.transform.Find("ImageIcon").GetComponent<Image>();
                iconImg.sprite = skillData != null ? skillData.skillIcon : null;

                // Gán level
                Text levelText = skillObj.transform.Find("LevelTxt").GetComponent<Text>();
                levelText.text = $"{skill.level}";
            }

        }
    }
    public void ShowHistory()
    {
        historyPanel.SetActive(true);
        LoadHistoryUI();
    }
    public void hideHistory()
    {
        historyPanel.SetActive(false);
    }
    private SkillData GetSkillData(string skillName)
    {
        return allSkills.Find(s => s.skillName == skillName);
    }
}
