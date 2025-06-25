using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUpgrades : MonoBehaviour
{
    public static CharacterUpgrades Instance;

    [Header("References")]
    public PlayerController playerController;
    public PlayerHealth playerHealth;
    public PlayerDame PlayerDame; // Giả định bạn có script này
    public ItemCollector itemCollector; // Giả định bạn có script tăng tầm hút

    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        ApplyAllUpgrades(); // Áp dụng ngay khi game bắt đầu
    }

    public void ApplyAllUpgrades()
    {
        var upgrades = LoadUpgradeData();

        foreach (var upgrade in upgrades)
        {
            float value = GetCurrentValue(upgrade);
            ApplyUpgrade(upgrade.upgradeName, value);
        }
    }

    public void ApplyUpgrade(string upgradeName, float value)
    {
        switch (upgradeName)
        {
            case "Health":
                if (playerHealth != null)
                    playerHealth.setUpgradeMaxHealth(value);
                break;
            case "Speed":
                if (playerController != null)
                    playerController.upgradeSpeed(value);
                break;
            case "Attack":
                if (PlayerDame != null)
                    PlayerDame.IncreaseBaseDamage(value);
                break;
            case "Crit":
                if(PlayerSkillManager.Instance != null)
                {
                    value = Mathf.Clamp01(value); // Đảm bảo không vượt quá 1.0 (100%)
                    PlayerSkillManager.Instance.UpgradeCritChance(value);
                }
                break;
            case "Magnet":
                if (itemCollector != null)
                    itemCollector.increaseUpgradeRadius(value);
                break;
        }
    }

    private List<UpgradeData> LoadUpgradeData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("upgrade_data");
        if (jsonFile != null)
        {
            UpgradeDataList wrapper = JsonUtility.FromJson<UpgradeDataList>(jsonFile.text);
            foreach (var upgrade in wrapper.upgrades)
            {
                upgrade.level = PlayerPrefs.GetInt($"UpgradeLevel_{upgrade.upgradeName}", 0);
            }
            return wrapper.upgrades;
        }

        return new List<UpgradeData>();
    }

    public float GetCurrentValue(UpgradeData upgrade)
    {
        float totalValue = upgrade.baseValue;
        for (int i = 0; i < upgrade.level && i < upgrade.valuePerLevel.Count; i++)
        {
            totalValue += upgrade.valuePerLevel[i];
        }
        return totalValue;
    }
}
