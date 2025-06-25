using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public static SkillUI instance;
    public GameObject skillPanel;
    public Button[] skillButtons;
    public Image[] skillIcons;
    public Text[] skillNames;
    public Text[] skillDiscriptions; 
    public Text[] skillLevelTexts;

    private List<SkillData> currentSkills;


    [Header("Reroll Settings")]
    public Button rerollButton;
    public Text   rerollTexts;

    // <--- CÁC BIẾN CẤU HÌNH HIỆU ỨNG CHO NÚT SKILL --->
    [Header("Button Animation Settings")]
    public float buttonPopUpDelay = 0.05f;        // Độ trễ giữa mỗi nút skill khi xuất hiện
    public float buttonPopUpDuration = 0.3f;      // Thời gian phóng to của nút
    public Ease buttonPopUpEase = Ease.OutBack;   // Kiểu easing cho hiệu ứng phóng to của nút

    // Thời gian cho hiệu ứng biến mất của nút khi chọn
    public float selectButtonScaleDuration = 0.15f;
    public float selectButtonFadeDuration = 0.15f;
    public Ease selectButtonEase = Ease.OutQuad;

    // Thời gian cho hiệu ứng biến mất của các nút còn lại
    public float otherButtonsFadeOutDuration = 0.1f;
    public Ease otherButtonsEase = Ease.InBack;

    [Header("Ad Reroll Settings")]
    public Button watchAdButton;
    public float adCooldown = 60f;
    private float lastAdShownTime = -999f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        skillPanel.SetActive(false);
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveAllListeners();
            rerollButton.onClick.AddListener(OnRerollButtonClicked);
            rerollButton.gameObject.SetActive(false); // Ban đầu ẩn nút đổi
        }
        if (watchAdButton != null)
        {
            watchAdButton.onClick.RemoveAllListeners();
            watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
            watchAdButton.gameObject.SetActive(false); // Ban đầu ẩn nút quảng cáo
        }
    }

    public void DisplaySkills(List<SkillData> skills)
    {
        currentSkills = skills;
        skillPanel.SetActive(true);

      
        // Phát âm thanh khi panel xuất hiện (nếu có)
       // AudioManager.Instance.PlaySkillPanelOpenSound(); // <--- THÊM ÂM THANH NÀY

        ResetAllSkillButtons();
        if (rerollButton != null)
        {
            rerollButton.gameObject.SetActive(true);
            UpdateRerollUI(RerollManager.Instance.GetCurrentRerolls());
        }
        ShowAdButton(RerollManager.Instance.GetCurrentRerolls() <= 0 && Time.timeSinceLevelLoad >= lastAdShownTime + adCooldown);
        
        // <--- HIỆU ỨNG CHO CÁC NÚT SKILL CON --->
        for (int i = 0; i < Mathf.Min(skills.Count, skillButtons.Length); i++)
        {
            SkillData skill = skills[i];
            int currentLevel = GetSkillCurrentLevel(skill);
            bool isMaxLevel = currentLevel >= skill.maxLevel;

            var btn = skillButtons[i];
            btn.gameObject.SetActive(true);
            btn.interactable = !isMaxLevel;

            // Đảm bảo scale ban đầu của nút là 0 hoặc nhỏ để hiệu ứng pop-up hoạt động
            btn.transform.localScale = Vector3.zero;

            CanvasGroup buttonCanvasGroup = btn.GetComponent<CanvasGroup>();
            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup = btn.gameObject.AddComponent<CanvasGroup>(); // Thêm CanvasGroup nếu chưa có
            }
            buttonCanvasGroup.alpha = 0f;

            skillIcons[i].sprite = skill.skillIcon;
            skillNames[i].text = $"{skill.skillName} Lv {currentLevel + 1}";
            skillDiscriptions[i].text = skill.description;
            //skillLevelTexts[i].text = $"Cấp {currentLevel}/{skill.maxLevel}"; // Bỏ comment nếu muốn dùng

            // Ngừng bất kỳ tween cũ nào đang chạy trên nút này
            DOTween.Kill(btn.transform);
            DOTween.Kill(buttonCanvasGroup);

            // Hiệu ứng phóng to của nút
            btn.transform.DOScale(1f, buttonPopUpDuration)
                .SetDelay(i * buttonPopUpDelay) // Độ trễ từng nút
                .SetEase(Ease.OutBack) // Hiệu ứng bật nảy đẹp mắt
                .SetUpdate(true); // Quan trọng
            buttonCanvasGroup.DOFade(1f, buttonPopUpDuration)
                .SetDelay(i * buttonPopUpDelay)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
            // Gán sự kiện click
            int index = i;
            btn.onClick.RemoveAllListeners(); // RẤT QUAN TRỌNG: Xóa listener cũ để tránh lỗi
            btn.onClick.AddListener(() => SelectSkill(index));
        }
    }
    private void ResetAllSkillButtons()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].gameObject.SetActive(false);
            skillButtons[i].onClick.RemoveAllListeners();
            // maxLevelIndicators[i].SetActive(false);
            skillButtons[i].transform.localScale = Vector3.one;
            CanvasGroup buttonCanvasGroup = skillButtons[i].GetComponent<CanvasGroup>();
            if (buttonCanvasGroup != null)
            {
                buttonCanvasGroup.alpha = 1f; // Reset alpha về 1
            }
        }
    }
    public void SelectSkill(int index)
    {
        if (index < 0 || index >= currentSkills.Count) return;

        SkillData selectedSkill = currentSkills[index];
        Debug.Log($"Đã chọn kỹ năng: {selectedSkill.skillName}");

        if (rerollButton != null)
        {
            rerollButton.interactable = false;
        }
        ShowAdButton(false);

        foreach (var btn in skillButtons)
        {
            btn.interactable = false;
        }

        // Nâng cấp skill
        PlayerSkillManager.Instance.EquipSkill(selectedSkill);

        // Phát âm thanh khi chọn skill
     //   AudioManager.Instance.PlaySkillSelectSound(); // <--- THÊM ÂM THANH NÀY

        // Hiệu ứng cho nút được chọn (phóng to và biến mất nhanh)
        Button selectedButton = skillButtons[index];
        CanvasGroup selectedButtonCanvasGroup = selectedButton.GetComponent<CanvasGroup>();
        if (selectedButtonCanvasGroup == null)
        {
            selectedButtonCanvasGroup = selectedButton.gameObject.AddComponent<CanvasGroup>();
        }

        selectedButton.transform.DOScale(1.2f, selectButtonScaleDuration).SetEase(selectButtonEase).SetUpdate(true);
        selectedButtonCanvasGroup.DOFade(0f, selectButtonFadeDuration).SetUpdate(true);
       

        // Ẩn các nút còn lại ngay lập tức hoặc làm mờ chúng
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i == index) continue; // Bỏ qua nút đã chọn

            // Hiệu ứng làm mờ và thu nhỏ các nút còn lại
            CanvasGroup otherButtonCanvasGroup = skillButtons[i].GetComponent<CanvasGroup>();
            if (otherButtonCanvasGroup == null)
            {
                otherButtonCanvasGroup = skillButtons[i].gameObject.AddComponent<CanvasGroup>();
            }

            DOTween.Kill(skillButtons[i].transform);
            DOTween.Kill(otherButtonCanvasGroup);

            skillButtons[i].transform.DOScale(0f, otherButtonsFadeOutDuration)
                .SetEase(otherButtonsEase)
                .SetUpdate(true);
            otherButtonCanvasGroup.DOFade(0f, otherButtonsFadeOutDuration)
                .SetEase(Ease.OutQuad) // Hoặc Ease.InQuad tùy hiệu ứng bạn muốn
                .SetUpdate(true);
        }
        float longestButtonEffectDuration = Mathf.Max(selectButtonScaleDuration, selectButtonFadeDuration, otherButtonsFadeOutDuration);
        DOVirtual.DelayedCall(longestButtonEffectDuration + 0.1f, ClosePanel, false).SetUpdate(true);
    }
    private void OnRerollButtonClicked()
    {
        if (PlayerExp.Instance != null)
        {
            PlayerExp.Instance.RequestRerollSkills();
            // Nút reroll sẽ bị disable tự động nếu không còn lượt đổi
            // Các nút skill con sẽ được làm mới thông qua DisplaySkills() được gọi từ RequestRerollSkills()
            ShowAdButton(RerollManager.Instance.GetCurrentRerolls() <= 0 && Time.timeSinceLevelLoad >= lastAdShownTime + adCooldown);
        }
    }

    // --- Hàm mới để cập nhật UI số lượt đổi ---
    public void UpdateRerollUI(int rerollsLeft)
    {
        if (rerollButton != null)
        {
            rerollButton.interactable = (rerollsLeft > 0); // Chỉ cho phép đổi nếu còn lượt
        }
        if (rerollTexts!=null)
        {
            rerollTexts.text = $"Còn lại: {rerollsLeft}";
        }
    }

    private int GetSkillCurrentLevel(SkillData data)
    {
        var runtime = PlayerSkillManager.Instance.equippedSkills
            .Find(s => s.baseSkill.skillType == data.skillType);
        return runtime != null ? runtime.currentLevel : 0;
    }
    public void ClosePanel()
    {
        skillPanel.SetActive(false);
        if (rerollButton != null)
        {
            rerollButton.gameObject.SetActive(false);
        }
        ShowAdButton(false);
    }
    private void OnWatchAdButtonClicked()
    {
        if (AdsManager.Instance != null && Time.timeSinceLevelLoad >= lastAdShownTime + adCooldown)
        {
            float previousTimeScale = Time.timeScale;
            Time.timeScale = 1f; // Tạm thời set về 1 để quảng cáo hoạt động đúng (nếu cần)

            AdsManager.Instance.ShowRewardedAd(() => // Truyền lambda expression (callback)
            {
                lastAdShownTime = Time.timeSinceLevelLoad;
                PlayerExp.Instance.GrantRerollsFromAd();
                ShowAdButton(false);

                Time.timeScale = previousTimeScale;
            });
        }
        else
        {
            Debug.LogWarning("AdsManager is not initialized. Cannot show rewarded ad.");
            // Trong môi trường debug, bạn có thể cấp thưởng ngay lập tức:
            // CurrencyManage.Instance.AddMoney(coinAmount);
            // Debug.Log($"[DEBUG] Cấp {coinAmount} xu ngay lập tức vì AdsManager không tồn tại.");
        }
    }
    public void ShowAdButton(bool show)
    {
        if (watchAdButton != null)
        {
            watchAdButton.gameObject.SetActive(show);
            if (show) // Nếu hiển thị, kiểm tra lại xem có tương tác được không (cooldown)
            {
                watchAdButton.interactable = (Time.timeSinceLevelLoad >= lastAdShownTime + adCooldown);
            }
        }
    }
}
