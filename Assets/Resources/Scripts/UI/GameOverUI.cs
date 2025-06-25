using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI instance;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button backToHomeButton;
    [SerializeField] Sprite winImage;
    [SerializeField] Sprite loseImage;
    [SerializeField] private Image image;
    [SerializeField] Text yourResultTxt;


    [Header("Skill UI")]
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private Transform attackSkillContainer;
    [SerializeField] private Transform supportSkillContainer;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        resultPanel.SetActive(false);
        backToHomeButton.onClick.AddListener(BackToHome);
    }
    public void ShowResult(bool isWin)
    {
        resultPanel.SetActive(true);

        Time.timeScale = 0f;
        float playTime = FindObjectOfType<PlayTimeUI>().GetPlayTime();
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        yourResultTxt.text = $"{minutes:00}:{seconds:00}s";

        image.sprite = isWin ? winImage : loseImage;
        // Bạn có thể đổi màu text/icon nếu muốn:
        // titleText.color = isWin ? Color.green : Color.red;

        ShowEquippedSkills();
        SaveMatchHistory(isWin, yourResultTxt.text);
    }
    private void BackToHome()
    {
        Time.timeScale = 1f; // khôi phục thời gian
        CurrencyManage.Instance.CommitSessionMoney();
        SceneManager.LoadScene("MainMenu"); // thay "Home" bằng tên scene chính của bạn
    }
    private void ShowEquippedSkills()
    {
        // Clear cũ
        foreach (Transform child in attackSkillContainer)
            Destroy(child.gameObject);
        foreach (Transform child in supportSkillContainer)
            Destroy(child.gameObject);

        // Duyệt kỹ năng đã trang bị
        var skills = PlayerSkillManager.Instance.equippedSkills;

        foreach (var skill in skills)
        {
            GameObject slot = Instantiate(skillSlotPrefab);
            slot.transform.Find("ImageIcon").GetComponent<Image>().sprite = skill.baseSkill.skillIcon;
            slot.transform.Find("LevelTxt").GetComponent<Text>().text = skill.currentLevel.ToString();

            Transform parent = skill.baseSkill.skillCategory == SkillCategory.Attack
                ? attackSkillContainer
                : supportSkillContainer;

            slot.transform.SetParent(parent, false);
        }
    }
    private void SaveMatchHistory(bool isWin, string timeSurvived)
    {
        var record = new MatchRecord
        {
            characterID = PlayerDataManager.Instance.SelectedCharacter.characterID,
            characterName = PlayerDataManager.Instance.SelectedCharacter.characterName,
            levelName = SceneManager.GetActiveScene().name,
            playTime = timeSurvived,
            goldEarned = CurrencyManage.Instance.GetSessionMoney(),
            levelPlayer = PlayerExp.Instance.currentLevel,
            isWin = isWin,
            dateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            equippedSkills = new List<SkillInfo>()
        };

        foreach (var skill in PlayerSkillManager.Instance.equippedSkills)
        {
            record.equippedSkills.Add(new SkillInfo
            {
                skillName = skill.baseSkill.skillName,
                level = skill.currentLevel,
                skillCategory = skill.baseSkill.skillCategory
            });
        }

        MatchHistoryManager.Instance.AddRecord(record);
    }
}
