using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public List<MissionData> allMissions;
    public Transform missionContainer;
    public GameObject missionPrefab;

    public Sprite defaultButtonSprite;
    public Sprite completedButtonSprite;

    public delegate void MissionsLoadedAction();
    public static event MissionsLoadedAction OnMissionsLoaded;
    private bool missionsLoaded = false;
    public string currentCategoryFilter = "Daily";
    private string jsonFilePath = "missions";

    private const string LastResetTimeKey = "LastDailyMissionResetTime";
    private const int ResetHour = 5; // Giờ reset (5 sáng)
    public static event System.Action OnMissionClaimed;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadMissionsFromJSON(); // Load dữ liệu nhiệm vụ ngay khi Awake
        LoadMissionProgress();   // Load progress ngay sau khi load dữ liệu
        CheckAndResetDailyMissions();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm lại MissionBackground sau khi scene load
        var canvas = GameObject.Find("CanvasMission");
        if (canvas != null)
        {
            missionContainer = canvas.transform.Find("MissionBackground");
            if (missionContainer == null)
                Debug.LogWarning("Không tìm thấy MissionBackground trong Canvas.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Canvas.");
        }

        // Sau khi gán lại thì refresh UI nếu cần
        if (missionContainer != null)
        {
            RefreshMissionUI();
        }
    }
    private void LoadMissionsFromJSON()
    {
     //   Debug.Log("Đường dẫn file JSON: " + jsonFilePath);
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFilePath);
     //   Debug.Log("File JSON có tồn tại không? " + (jsonFile != null));

        
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            Debug.Log(jsonString);
            MissionListWrapper wrapper = JsonUtility.FromJson<MissionListWrapper>(jsonString);
            allMissions = wrapper.missions;

            missionsLoaded = true;
            OnMissionsLoaded?.Invoke(); // Báo hiệu đã load xong
           // Debug.Log("Đã tải " + allMissions.Count + " nhiệm vụ từ JSON");
        }
        else
        {
            missionsLoaded = true; // Vẫn báo là đã load (dù có lỗi) để tránh treo UI
            OnMissionsLoaded?.Invoke();
            Debug.LogError("Không tìm thấy file missions.json");
            allMissions = new List<MissionData>();
        }

        LoadMissionProgress();
    }

    private void LoadMissionProgress()
    {
        if (allMissions != null) // Đảm bảo allMissions đã được load
        {
            foreach (var mission in allMissions)
            {
                string key = "mission_" + mission.id;
                if (PlayerPrefs.HasKey(key + "_progress"))
                {
                    mission.currentProgress = PlayerPrefs.GetInt(key + "_progress");
                    mission.isCompleted = PlayerPrefs.GetInt(key + "_completed") == 1;
                    mission.isClaimed = PlayerPrefs.GetInt(key + "_claimed") == 1;
                }
            }
            Debug.Log("Đã load tiến trình nhiệm vụ từ PlayerPrefs.");
        }
        else
        {
            Debug.LogWarning("allMissions chưa được load khi cố gắng load progress.");
        }
    }

    public void SaveMissionProgress()
    {
        foreach (var mission in allMissions)
        {
            string key = "mission_" + mission.id;
            PlayerPrefs.SetInt(key + "_progress", mission.currentProgress);
            PlayerPrefs.SetInt(key + "_completed", mission.isCompleted ? 1 : 0);
            PlayerPrefs.SetInt(key + "_claimed", mission.isClaimed ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void RefreshMissionUI()
    {
        if (missionContainer == null) return; // Đảm bảo missionContainer đã được tìm thấy

        // Xóa UI hiện tại
        foreach (Transform child in missionContainer)
        {
            Destroy(child.gameObject);
        }

        // Lấy danh sách nhiệm vụ theo category hiện tại
        var missionsToDisplay = allMissions.Where(m => m.category == currentCategoryFilter).ToList();

        // Tạo UI cho các nhiệm vụ trong danh sách đã lọc
        foreach (var mission in missionsToDisplay)
        {
            var missionUI = Instantiate(missionPrefab, missionContainer);
            SetupMissionUI(missionUI, mission);
        }
    }

    public void SetupMissionUI(GameObject missionUI, MissionData mission)
    {
        var titleObj = missionUI.transform.Find("TxtDescription");
        var txtAmount = missionUI.transform.Find("TxtAmount");
        //var buttonObj = missionUI.transform.Find("Button (Legacy)");
        var iconRewardObj = missionUI.transform.Find("Image");


        //if (titleObj == null) Debug.LogError("Không tìm thấy TxtDescription");
      //  if (buttonObj == null) Debug.LogError("Không tìm thấy ButtonMission(Clone)");
        //if (imageObj == null) Debug.LogError("Không tìm thấy Image");

        Text titleText = titleObj?.GetComponent<Text>();
        Text amountText = txtAmount?.GetComponent<Text>();
        Button claimButton = missionUI.GetComponent<Button>();
        Image buttonImage = missionUI.GetComponent<Image>();
        Image iconRewardImage = iconRewardObj?.GetComponent<Image>();
        //Image buttonImage = claimButton?.GetComponent<Image>();
        // GameObject completedIcon = imageObj?.gameObject;

        // Nếu titleText hoặc claimButton là null, in ra tiếp
        if (titleText == null) Debug.LogError("Không có component Text trên TxtDescription");
        if (claimButton == null) Debug.LogError("Không có component Button trên ButtonMission");

        if (titleText != null)
            titleText.text = mission.title + $" [{mission.currentProgress}/{mission.requiredProgress}]";
        if (amountText != null && mission.rewards != null && mission.rewards.Count > 0)
        {
            //Debug.Log("Tìm thấy amountText component");
            //Debug.Log("Giá trị amount từ data: " + mission.rewards[0].amount);
            amountText.text = "+" + mission.rewards[0].amount;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy amountText component hoặc rewards rỗng.");
        }
     //   Debug.Log("Rewards cho mission " + mission.id + ": " + (mission.rewards != null ? mission.rewards.Count.ToString() : "null"));
        if (mission.rewards != null && mission.rewards.Count > 0)
        {
            //Debug.Log("Loại reward: " + mission.rewards[0].type + ", Amount: " + mission.rewards[0].amount);
            var firstReward = mission.rewards[0]; // Lấy phần thưởng đầu tiên để hiển thị

            if (amountText != null)
            {
                amountText.text = "+" + firstReward.amount;
            }

            if (iconRewardImage != null)
            {
                if (firstReward.type == "Gold")
                {
                    iconRewardImage.sprite = GetRewardIcon("Gold"); // Hàm này bạn sẽ viết
                }
                else if (firstReward.type == "Gem")
                {
                    iconRewardImage.sprite = GetRewardIcon("Gem"); // Hàm này bạn sẽ viết
                }
                // Thêm các loại phần thưởng khác (Item, Hero, ...)
                else if (firstReward.type == "Item")
                {
                    iconRewardImage.sprite = GetRewardIcon(firstReward.itemId); // Dùng itemId cho icon vật phẩm
                }
                else if (firstReward.type == "Hero")
                {
                    iconRewardImage.sprite = GetRewardIcon(firstReward.itemId); // Dùng itemId cho icon hero
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy icon cho loại phần thưởng: " + firstReward.type);
                    iconRewardImage.sprite = null; // Đặt sprite về null nếu không tìm thấy
                }
                iconRewardImage.enabled = iconRewardImage.sprite != null; // Chỉ hiển thị Image nếu có sprite
            }
        }
        else
        {
            // Ẩn amount và icon nếu không có phần thưởng
            if (amountText != null) amountText.text = "";
            if (iconRewardImage != null) iconRewardImage.enabled = false;
        }
        if (mission.isClaimed)
        {
            if (claimButton != null) claimButton.gameObject.SetActive(false);
            // if (completedIcon != null) completedIcon.SetActive(true);
            if (buttonImage != null && completedButtonSprite != null)
            {
                buttonImage.sprite = completedButtonSprite;
            }
        }
        else if (mission.isCompleted)
        {
            if (buttonImage != null && completedButtonSprite != null)
                buttonImage.sprite = completedButtonSprite;

            // Cho phép nhận thưởng khi ấn nút
            if (claimButton != null)
            {
                claimButton.onClick.RemoveAllListeners();
                claimButton.onClick.AddListener(() => ClaimReward(mission.id));
            }
        }
        else
        {
            if (buttonImage != null && defaultButtonSprite != null)
                buttonImage.sprite = defaultButtonSprite;

            // Không làm gì khi chưa hoàn thành
            if (claimButton != null)
            {
                claimButton.onClick.RemoveAllListeners();
                claimButton.onClick.AddListener(() =>
                {
                    Debug.Log("Nhiệm vụ chưa hoàn thành!");
                });
            }
        }
    }
    

    public void UpdateMissionProgress(string missionId, int currentProgress)
    {
        var mission = allMissions.Find(m => m.id == missionId);
        if (mission != null && !mission.isCompleted)
        {
            mission.currentProgress = currentProgress; // GÁN trực tiếp tiến trình hiện tại

            if (mission.currentProgress >= mission.requiredProgress)
            {
                Debug.Log($"Nhiệm vụ hoàn thành (MissionManager): {mission.title} ({mission.id}), Progress: {mission.currentProgress}/{mission.requiredProgress}");
                mission.isCompleted = true;
            }
            SaveMissionProgress();
            // RefreshMissionUI(); // Có thể refresh UI sau khi cập nhật tiến trình
        }
    }
    public List<MissionData> GetMissionsByCategory(string category)
    {
        return allMissions.Where(mission => mission.category == category).ToList();
    }
    public void ClaimReward(string missionId)
    {
        var mission = allMissions.Find(m => m.id == missionId);
        if (mission != null && mission.isCompleted && !mission.isClaimed)
        {
            foreach (var reward in mission.rewards)
            {
                GiveReward(reward);
            }

            mission.isClaimed = true;
            SaveMissionProgress();
            //RefreshMissionUI();
            Debug.Log("Đã nhận thưởng cho mission: " + mission.title);
            OnMissionClaimed?.Invoke();
        }
    }
    private Sprite GetRewardIcon(string iconName)
    {
        return Resources.Load<Sprite>("Icons/" + iconName); // Đặt icon của bạn trong thư mục "Resources/Icons"
    }
    private void GiveReward(RewardData reward)
    {
        switch (reward.type)
        {
            case "Gold":
                // PlayerData.Instance.AddGold(reward.amount);
               CurrencyManage.Instance.AddMoney(reward.amount);
                break;
            case "Gem":
                // PlayerData.Instance.AddGem(reward.amount);
                Debug.Log($"Nhận {reward.amount} Gem");
                GemManager.Instance.AddGems(reward.amount);
                break;
            case "Item":
                // InventoryManager.Instance.AddItem(reward.itemId, reward.amount);
                Debug.Log($"Nhận vật phẩm {reward.itemId} (x{reward.amount})");
                break;
            case "Hero":
                // HeroManager.Instance.UnlockHero(reward.itemId);
                Debug.Log($"Mở khóa hero: {reward.itemId}");
                break;
        }
    }
    public void OnLevelCompleted(int levelID)
    {
        // Cập nhật dữ liệu LevelUnlock
        LevelUnlock.MarkLevelCompleted(levelID);

        // Xử lý nhiệm vụ tương ứng
        switch (levelID)
        {
            case 1:
                UpdateMissionProgress("mission_4", 1); // vượt ải 1
                break;
            case 2:
                UpdateMissionProgress("mission_5", 1); // vượt ải 2
                break;
            case 3:
                UpdateMissionProgress("mission_8", 1); // vượt ải 2
                break;
                // ... thêm case nếu cần
        }
    }

    private void OnApplicationQuit()
    {
        SaveMissionProgress();
    }
    private void CheckAndResetDailyMissions()
    {
        string lastResetTimeString = PlayerPrefs.GetString(LastResetTimeKey, "");

        DateTime now = DateTime.Now;
        DateTime todayResetTime = new DateTime(now.Year, now.Month, now.Day, ResetHour, 0, 0);

        if (string.IsNullOrEmpty(lastResetTimeString) || !DateTime.TryParse(lastResetTimeString, out DateTime lastResetTime))
        {
            Debug.Log("Không có thời điểm reset trước đó hoặc lỗi parse. Reset nhiệm vụ.");
            ResetDailyMissions();
            SaveLastResetTime();
            return;
        }

        // Nếu thời điểm hiện tại đã qua giờ reset hôm nay
        // và thời điểm reset gần nhất nằm trước thời điểm reset hôm nay → thực hiện reset
        if (now >= todayResetTime && lastResetTime < todayResetTime)
        {
            ResetDailyMissions();
            SaveLastResetTime();
        }
    }


    private void ResetDailyMissions()
    {
        foreach (var mission in allMissions)
        {
            if (mission.category == "Daily")
            {
                mission.currentProgress = 0;
                mission.isCompleted = false;
                mission.isClaimed = false;
            }
        }
        SaveMissionProgress();
        if (missionContainer != null && currentCategoryFilter == "Daily")
        {
            RefreshMissionUI();
        }
        Debug.Log("Nhiệm vụ hàng ngày đã được reset.");
    }

    private void SaveLastResetTime()
    {
        PlayerPrefs.SetString(LastResetTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
        Debug.Log("Đã lưu thời điểm reset nhiệm vụ hàng ngày: " + DateTime.Now.ToString());
    }
}
