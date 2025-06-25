using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerExp : MonoBehaviour
{
    public static PlayerExp Instance;
    public int currentExp = 0;
    public int expToUpLevel = 100;
    public int currentLevel = 1;
    public List<SkillData> allSkills;

   // private int currentRerolls;

    private int pendingSkillChoices = 0;
    private bool isChoosingSkill = false;
    public int rerollsFromAdAmount = 1;
    private void Awake() // Thêm hàm Awake này
    {
        if (Instance == null)
        {
            Instance = this;
            //currentRerolls = PlayerPrefs.GetInt("RerollCount", 1);
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ có một PlayerExp
        }
    }
    public void gainXP(int amount)
    {
        Debug.Log($"GainXP called with amount: {amount}");
        currentExp += amount;
        while (currentExp >= expToUpLevel)
        {
            currentExp -= expToUpLevel;
            currentLevel++;
            PlayerHUD.instance.SetLevelTxt(currentLevel);
            expToUpLevel = GetExpToNextLevel(currentLevel);
            pendingSkillChoices++;
        }
        Debug.Log($"Current EXP: {currentExp}/{expToUpLevel}");
        ExpUI.instance?.updateUI(currentExp, expToUpLevel, currentLevel);
        if (!isChoosingSkill && pendingSkillChoices > 0)
        {
            StartCoroutine(HandleSkillChoices());
        }

    }

    private int GetExpToNextLevel(int level)
    {
        // Công thức tăng EXP mượt (hệ số 1.2 phù hợp đến cấp 50)
        return Mathf.RoundToInt(100 + 20 * Mathf.Pow(level, 1.15f));
    }
    IEnumerator HandleSkillChoices()
    {
        isChoosingSkill = true;

        while (pendingSkillChoices > 0)
        {
            ShowSkillSelection();
            Time.timeScale = 0f;

            // Đợi đến khi người chơi chọn xong (skill panel bị ẩn)
            while (SkillUI.instance.skillPanel.activeSelf)
            {
                yield return null;
            }
            yield return new WaitForSecondsRealtime(0.3f);
            pendingSkillChoices--;
        }
        Time.timeScale = 1f;
        isChoosingSkill = false;
    }
    public void RequestRerollSkills()
    {
        if(RerollManager.Instance == null)
        {
            Debug.LogError("RerollManager.Instance is null. Make sure RerollManager is in the scene.");
            return;
        }
        if (RerollManager.Instance.UseReroll())
        {
            Debug.Log($"Còn {RerollManager.Instance.GetCurrentRerolls()} lượt đổi."); // Lấy số lượt từ RerollManager
            ShowSkillSelection();
        }
        
    }
    public void GrantRerollsFromAd()
    {
        if (RerollManager.Instance == null)
        {
            Debug.LogError("RerollManager.Instance is null. Make sure RerollManager is in the scene.");
            return;
        }
        RerollManager.Instance.AddReroll(rerollsFromAdAmount);
       
        SkillUI.instance.UpdateRerollUI(RerollManager.Instance.GetCurrentRerolls());
        // Có thể ẩn nút quảng cáo sau khi nhận lượt đổi
       // SkillUI.instance.ShowAdButton(false);
        // Sau khi nhận reroll, nếu người chơi muốn, họ có thể nhấn nút reroll để đổi skill mới
        // hoặc bạn có thể tự động reroll nếu muốn (nhưng thường là để người chơi tự quyết)
    }
   

    void ShowSkillSelection()
    {
        List<SkillData> newlyAvailableAttackSkills = new List<SkillData>(); // Skill tấn công mới, còn slot
        List<SkillData> newlyAvailableSupportSkills = new List<SkillData>(); // Skill hỗ trợ mới, còn slot
        List<SkillData> upgradeableAttackSkills = new List<SkillData>();    // Skill tấn công đã học, chưa max level
        List<SkillData> upgradeableSupportSkills = new List<SkillData>();   // Skill hỗ trợ đã học, chưa max level
        List<SkillData> allEquippedSkills = new List<SkillData>();          // Tất cả skill đã học (dù max level hay chưa)

        foreach (var skill in allSkills)
        {
            SkillRuntimeData runtimeSkill = PlayerSkillManager.Instance.equippedSkills
                .Find(s => s.baseSkill.skillType == skill.skillType);

            if (runtimeSkill != null) // Skill đã học
            {
                allEquippedSkills.Add(skill); // Thêm vào danh sách tất cả skill đã học

                if (!runtimeSkill.IsMaxLevel())
                {
                    if (skill.skillCategory == SkillCategory.Attack)
                    {
                        upgradeableAttackSkills.Add(skill);
                    }
                    else if (skill.skillCategory == SkillCategory.Support)
                    {
                        upgradeableSupportSkills.Add(skill);
                    }
                }
            }
            else // Skill chưa học
            {
                if (skill.skillCategory == SkillCategory.Attack && PlayerSkillManager.Instance.HasAvailableSlot(SkillCategory.Attack))
                {
                    newlyAvailableAttackSkills.Add(skill);
                }
                else if (skill.skillCategory == SkillCategory.Support && PlayerSkillManager.Instance.HasAvailableSlot(SkillCategory.Support))
                {
                    newlyAvailableSupportSkills.Add(skill);
                }
            }
        }

        List<SkillData> finalSkillChoices = new List<SkillData>();
        int numChoices = 3;

        bool attackSlotsFull = !PlayerSkillManager.Instance.HasAvailableSlot(SkillCategory.Attack);
        bool supportSlotsFull = !PlayerSkillManager.Instance.HasAvailableSlot(SkillCategory.Support);
        bool allSlotsFull = attackSlotsFull && supportSlotsFull;

        // Định nghĩa các pool skill tiềm năng
        List<SkillData> poolOptionA = new List<SkillData>(); // Pool cho trường hợp chưa full slot nào
        List<SkillData> poolOptionB = new List<SkillData>(); // Pool cho trường hợp full Attack nhưng chưa full Support
        List<SkillData> poolOptionC = new List<SkillData>(); // Pool cho trường hợp full Support nhưng chưa full Attack
        List<SkillData> poolOptionD = new List<SkillData>(); // Pool cho trường hợp full cả 10 slot

        // Xây dựng các pool dựa trên trạng thái của game
        // Pool A: Khi còn slot của cả hai loại (chưa full loại nào)
        poolOptionA.AddRange(newlyAvailableAttackSkills);
        poolOptionA.AddRange(newlyAvailableSupportSkills);
        poolOptionA.AddRange(upgradeableAttackSkills);
        poolOptionA.AddRange(upgradeableSupportSkills);
        poolOptionA = poolOptionA.Distinct().ToList(); // Đảm bảo duy nhất

        // Pool B: Khi full Attack, còn Support (Skill Hỗ trợ mới + Skill đã học có thể nâng cấp)
        poolOptionB.AddRange(newlyAvailableSupportSkills);
        poolOptionB.AddRange(upgradeableAttackSkills);
        poolOptionB.AddRange(upgradeableSupportSkills);
        poolOptionB = poolOptionB.Distinct().ToList();

        // Pool C: Khi full Support, còn Attack (Skill Tấn công mới + Skill đã học có thể nâng cấp)
        poolOptionC.AddRange(newlyAvailableAttackSkills);
        poolOptionC.AddRange(upgradeableAttackSkills);
        poolOptionC.AddRange(upgradeableSupportSkills);
        poolOptionC = poolOptionC.Distinct().ToList();

        // Pool D: Khi full cả 10 slot (Chỉ skill đã học có thể nâng cấp)
        poolOptionD.AddRange(upgradeableAttackSkills);
        poolOptionD.AddRange(upgradeableSupportSkills);
        poolOptionD = poolOptionD.Distinct().ToList();


        // Bắt đầu logic lựa chọn chính
        if (allSlotsFull)
        {
            Debug.Log("Tất cả 10 slot đã đầy. Chỉ hiển thị skill đã học có thể nâng cấp.");
            finalSkillChoices = GetRandomSkills(poolOptionD, numChoices);

            // Nếu tất cả skill đã học đều max level (poolOptionD rỗng)
            if (finalSkillChoices.Count == 0)
            {
                Debug.LogWarning("Tất cả skill đã đạt cấp tối đa! Không còn skill nào để lựa chọn. Hiển thị skill đã học (dù max level).");
                // Hiển thị 3 skill bất kỳ từ số skill đã học để UI không bị trống
                finalSkillChoices = GetRandomSkills(allEquippedSkills, numChoices);
            }
        }
        else if (attackSlotsFull && !supportSlotsFull)
        {
            Debug.Log("Slot tấn công đã đầy, còn slot hỗ trợ. Ưu tiên skill hỗ trợ mới và skill nâng cấp.");
            finalSkillChoices = GetRandomSkills(poolOptionB, numChoices);
        }
        else if (supportSlotsFull && !attackSlotsFull)
        {
            Debug.Log("Slot hỗ trợ đã đầy, còn slot tấn công. Ưu tiên skill tấn công mới và skill nâng cấp.");
            finalSkillChoices = GetRandomSkills(poolOptionC, numChoices);
        }
        else // Chưa full loại nào cả (còn slot tấn công VÀ còn slot hỗ trợ)
        {
            Debug.Log("Chưa full slot nào. Random ngẫu nhiên trong tất cả các skill có thể học/nâng cấp.");
            finalSkillChoices = GetRandomSkills(poolOptionA, numChoices);
        }

        // Fallback chung: Nếu các pool trên không đủ 3 lựa chọn, bổ sung từ allSkills
        // Điều này đảm bảo luôn có 3 ô hiển thị, dù người chơi không thể chọn hết.
        while (finalSkillChoices.Count < numChoices && allSkills.Count > 0)
        {
            SkillData randomFallback = allSkills[Random.Range(0, allSkills.Count)];
            if (!finalSkillChoices.Contains(randomFallback))
            {
                finalSkillChoices.Add(randomFallback);
            }
        }

        // Đảm bảo không có skill trùng lặp (dù đã cố gắng Distinct ở các pool)
        finalSkillChoices = finalSkillChoices.Distinct().ToList();
        SkillUI.instance.DisplaySkills(finalSkillChoices);
        SkillUI.instance.UpdateRerollUI(RerollManager.Instance.GetCurrentRerolls());
    }
    public void ChooseSkillFromBoss()
    {
        if (!isChoosingSkill)
        {
            StartCoroutine(ChooseOneSkillFromBoss());
        }
    }

    private IEnumerator ChooseOneSkillFromBoss()
    {
        isChoosingSkill = true;
        Time.timeScale = 0f;

        // Hiển thị 1 lượt chọn skill (dùng lại logic cũ)
        ShowSkillSelection();

        while (SkillUI.instance.skillPanel.activeSelf)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;
        isChoosingSkill = false;
    }
    private List<SkillData> GetRandomSkills(List<SkillData> sourceList, int count)
    {
        List<SkillData> result = new List<SkillData>();
        List<SkillData> tempSource = new List<SkillData>(sourceList); // Tạo bản sao để không làm thay đổi danh sách gốc

        while (result.Count < count && tempSource.Count > 0)
        {
            int index = Random.Range(0, tempSource.Count);
            result.Add(tempSource[index]);
            tempSource.RemoveAt(index);
        }
        return result;
    }
}
