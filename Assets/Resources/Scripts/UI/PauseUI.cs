using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject pausePanel;
    public PopupScaler popupScaler;
    public GameObject skillSlotPrefab;
    public Transform attackSkillContainer;
    public Transform supportSkillContainer;

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false); // ẩn panel lúc đầu
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        popupScaler.PlayScaleIn();
        Time.timeScale = 0f; // dừng thời gian
        ShowEquippedSkills();
        // AudioManager.Instance._sfxSource.Pause();
    }

    public void ResumeGame()
    {
        isPaused = false;
        popupScaler.HidePopup(() =>
        {
            pausePanel.SetActive(false); // Chỉ ẩn panel sau khi hiệu ứng đã hoàn tất
            Time.timeScale = 1f; // tiếp tục thời gian
            // AudioManager.Instance._sfxSource.UnPause(); // Bỏ comment nếu có AudioManager
        });
        //  AudioManager.Instance._sfxSource.UnPause();
    }

    public void GoToMainMenu()
    {
        SaveMatchHistory(false, "");
        Time.timeScale = 1f; // đảm bảo thời gian trở lại bình thường
        AdsManager.Instance.ShowInterstitialAd();
        SceneManager.LoadScene("MainMenu"); // thay bằng tên scene chính của bạn
    }
    private void ShowEquippedSkills()
    {
        // Clear trước
        foreach (Transform child in attackSkillContainer)
            Destroy(child.gameObject);

        foreach (Transform child in supportSkillContainer)
            Destroy(child.gameObject);

        // Duyệt danh sách skill
        var skills = PlayerSkillManager.Instance.equippedSkills;

        foreach (var skill in skills)
        {
            GameObject slot = Instantiate(skillSlotPrefab);
            slot.transform.Find("ImageIcon").GetComponent<UnityEngine.UI.Image>().sprite = skill.baseSkill.skillIcon;
            slot.transform.Find("LevelTxt").GetComponent<UnityEngine.UI.Text>().text = skill.currentLevel.ToString();

            if (skill.baseSkill.skillCategory == SkillCategory.Attack)
            {
                slot.transform.SetParent(attackSkillContainer, false);
            }
            else if (skill.baseSkill.skillCategory == SkillCategory.Support)
            {
                slot.transform.SetParent(supportSkillContainer, false);
            }
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
