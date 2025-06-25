using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text descriptionText;
    public Text priceText;
    public Text currentLevelText;
    public Button upgradeButton;
    public List<Button> upgradeButtons;
    public Image upgradeIconDisplay;
    public Image upgradeBorderDisplay;


    private List<UpgradeData> upgradesData;
    private UpgradeData selectedUpgrade;



    private void Start()
    {
        LoadUpgradeData();
        LoadSavedUpgrades();
        SetupUpgradeButtons();
        // Hiển thị thông tin của nâng cấp đầu tiên khi panel được mở
        if (upgradesData != null && upgradesData.Count > 0)
        {
            SelectUpgrade(upgradesData[0].upgradeName);
        }

        // Đăng ký sự kiện khi tiền thay đổi để cập nhật trạng thái nút nâng cấp
        CurrencyManage.Instance.OnMoneyChanged += UpdateUpgradeButtonInteractability;
        UpdateUpgradeButtonInteractability();
    }
    private void OnDestroy()
    {
        if (CurrencyManage.Instance != null)
        {
            CurrencyManage.Instance.OnMoneyChanged -= UpdateUpgradeButtonInteractability;
        }
    }
    private void LoadUpgradeData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("upgrade_data"); 
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            UpgradeDataList wrapper = JsonUtility.FromJson<UpgradeDataList>(jsonString);
            upgradesData = wrapper.upgrades;
        }
        else
        {
            Debug.LogError("Không tìm thấy file upgrade_data.json trong thư mục Resources.");
            upgradesData = new List<UpgradeData>();
        }
    }

    private void SetupUpgradeButtons()
    {
        foreach (var button in upgradeButtons)
        {
            string upgradeName = "";
        
            // Dựa vào tên GameObject của button để xác định loại nâng cấp
            if (button.gameObject.name.Contains("Health", System.StringComparison.OrdinalIgnoreCase))
            {
                upgradeName = "Health";
                //icon = button.transform.Find("ImageHealth").GetComponent<Image>().sprite;
                //upgradeIconDisplay.sprite = icon;
            }
            else if (button.gameObject.name.Contains("Attack", System.StringComparison.OrdinalIgnoreCase))
            {
                upgradeName = "Attack";
                //icon = button.transform.Find("ImageAttack").GetComponent<Image>().sprite;
                //upgradeIconDisplay.sprite = icon;

            }
            else if (button.gameObject.name.Contains("Speed", System.StringComparison.OrdinalIgnoreCase))
            {
                upgradeName = "Speed";
                //icon = button.transform.Find("ImageSpeed").GetComponent<Image>().sprite;
                //upgradeIconDisplay.sprite = icon;

            }
            else if (button.gameObject.name.Contains("Crit", System.StringComparison.OrdinalIgnoreCase))
            {
                upgradeName = "Crit";
                //icon = button.transform.Find("ImageCrit").GetComponent<Image>().sprite;
                //upgradeIconDisplay.sprite = icon;

            }
            else if (button.gameObject.name.Contains("MagnetRange", System.StringComparison.OrdinalIgnoreCase))
            {
                upgradeName = "Magnet";
                //icon = button.transform.Find("ImageMagnet").GetComponent<Image>().sprite;
                //upgradeIconDisplay.sprite = icon;

            }

            if (!string.IsNullOrEmpty(upgradeName))
            {
                string capturedUpgradeName = upgradeName; // Capture biến cho lambda
                button.onClick.AddListener(() => SelectUpgrade(capturedUpgradeName));
            }
            else
            {
                Debug.LogWarning("Không thể xác định loại nâng cấp cho button: " + button.gameObject.name);
            }
        }

        // Thêm listener cho nút Lên cấp
        upgradeButton.onClick.AddListener(AttemptUpgrade);
    }

    private void SelectUpgrade(string upgradeName)
    {
        selectedUpgrade = upgradesData.Find(upgrade => upgrade.upgradeName == upgradeName);
        if (selectedUpgrade != null)
        {
            foreach (var button in upgradeButtons)
            {
                if (button.gameObject.name.Contains(upgradeName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Image iconImage = button.GetComponentInChildren<Image>();
                    if (iconImage != null)
                    {
                        upgradeBorderDisplay.sprite = iconImage.sprite;
                        upgradeBorderDisplay.enabled = true;
                    }
                    // 🔽 Tìm Image con của button để lấy sprite
                    Image[] childImages = button.GetComponentsInChildren<Image>();
                    foreach (var image in childImages)
                    {
                        // Bỏ qua chính image của button (viền xám)
                        if (image.gameObject != button.gameObject)
                        {
                            upgradeIconDisplay.sprite = image.sprite;
                            upgradeIconDisplay.enabled = true;
                            break;
                        }
                    }
                    break;
                }
            }
            UpdateUpgradeInfoUI();
        }
        else
        {
            Debug.LogError("Không tìm thấy dữ liệu nâng cấp cho: " + upgradeName);
        }
    }

    private void UpdateUpgradeInfoUI()
    {
        if (selectedUpgrade != null)
        {
           
            string description = "";
            float currentValue = GetNextLevelValue(selectedUpgrade);

            switch (selectedUpgrade.upgradeName)
            {
                case "Health":
                    description = $"Máu tối đa: +{currentValue}";
                    break;
                case "Attack":
                    description = $"Sát thương: +{currentValue}";
                    break;
                case "Speed":
                    description = $"Tốc độ di chuyển: +{currentValue * 100:F0}%";
                    break;
                case "Crit":
                    description = $"Tỉ lệ chí mạng: +{currentValue * 100:F0}%"; // Hiển thị dạng phần trăm
                    break;
                case "Magnet":
                    description = $"Tầm hút: +{currentValue}";
                    break;
                default:
                    description = $"Nâng cấp {selectedUpgrade.upgradeName}: +{currentValue}";
                    break;
            }

            if (selectedUpgrade.level < selectedUpgrade.maxLevel)
            {
                priceText.text = selectedUpgrade.levelCosts[selectedUpgrade.level].ToString("N0") ;
                upgradeButton.interactable = CurrencyManage.Instance.GetCurrentMoney() >= selectedUpgrade.levelCosts[selectedUpgrade.level];
            }
            else
            {
                priceText.text = "MAX";
                upgradeButton.interactable = false;
            }

            descriptionText.text = description;
            currentLevelText.text = $"Cấp {selectedUpgrade.level}";
        }
        else
        {
            descriptionText.text = "";
            priceText.text = "";
            currentLevelText.text = "";
            upgradeButton.interactable = false;
        }
    }

    private float GetCurrentValue(UpgradeData upgrade)
    {
        if (upgrade.level == 0)
        {
            return upgrade.baseValue;
        }
        else if (upgrade.level > 0 && upgrade.level <= upgrade.valuePerLevel.Count)
        {
            return upgrade.baseValue + upgrade.valuePerLevel[upgrade.level - 1];
        }
        else
        {
            return upgrade.baseValue; // Hoặc xử lý lỗi nếu level vượt quá mảng
        }
    }

    private float GetNextLevelValue(UpgradeData upgrade)
    {
        if (upgrade.level < upgrade.maxLevel && upgrade.level < upgrade.valuePerLevel.Count)
        {
            return upgrade.baseValue + upgrade.valuePerLevel[upgrade.level];
        }
        else
        {
            return GetCurrentValue(upgrade); // Hoặc giá trị khi đạt max level
        }
    }

    private void AttemptUpgrade()
    {
        if (selectedUpgrade != null && selectedUpgrade.level < selectedUpgrade.maxLevel)
        {
            int cost = selectedUpgrade.levelCosts[selectedUpgrade.level];
            if (CurrencyManage.Instance.SpendMoney(cost))
            {
                selectedUpgrade.level++;
                SaveUpgradeLevel(selectedUpgrade); // Lưu level đã nâng cấp
                UpdateUpgradeInfoUI();
                // Gọi hàm để áp dụng hiệu ứng nâng cấp (cần viết hàm này ở một script khác, ví dụ: CharacterUpgrades)
                CharacterUpgrades.Instance?.ApplyUpgrade(selectedUpgrade.upgradeName, GetCurrentValue(selectedUpgrade));
            }
            else
            {
                Debug.Log("Không đủ tiền để nâng cấp.");
            }
        }
        else if (selectedUpgrade != null && selectedUpgrade.level >= selectedUpgrade.maxLevel)
        {
            Debug.Log("Nâng cấp đã đạt cấp tối đa.");
        }
        else
        {
            Debug.LogWarning("Không có nâng cấp nào được chọn.");
        }
    }

    private void UpdateUpgradeButtonInteractability()
    {
        if (selectedUpgrade != null && selectedUpgrade.level < selectedUpgrade.maxLevel)
        {
            upgradeButton.interactable = CurrencyManage.Instance.GetCurrentMoney() >= selectedUpgrade.levelCosts[selectedUpgrade.level];
        }
    }

    private void SaveUpgradeLevel(UpgradeData upgrade)
    {
        PlayerPrefs.SetInt($"UpgradeLevel_{upgrade.upgradeName}", upgrade.level);
        PlayerPrefs.Save();
    }

    private void LoadSavedUpgrades()
    {
        if (upgradesData != null)
        {
            foreach (var upgrade in upgradesData)
            {
                upgrade.level = PlayerPrefs.GetInt($"UpgradeLevel_{upgrade.upgradeName}", 0);
            }
        }
    }

    private void OnEnable()
    {
        LoadSavedUpgrades(); // Load lại level khi panel được mở
        if (upgradesData != null && selectedUpgrade != null)
        {
            SelectUpgrade(selectedUpgrade.upgradeName); // Cập nhật lại UI khi panel được mở lại
        }
        else if (upgradesData != null && upgradesData.Count > 0)
        {
            SelectUpgrade(upgradesData[0].upgradeName);
        }
        UpdateUpgradeButtonInteractability();
    }
}
