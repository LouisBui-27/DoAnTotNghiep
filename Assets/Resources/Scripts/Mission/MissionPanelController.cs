using UnityEngine;
using UnityEngine.UI;

public class MissionPanelController : MonoBehaviour
{
    public Transform missionContainer;
    public GameObject missionPrefab;
    public Button dailyButton;
    public Button mainButton;

    public Sprite defaultToggle;
    public Sprite selectToggle;

    private bool isPanelEnabled = false;

    void OnEnable()
    {
        isPanelEnabled = true;
        MissionManager.OnMissionsLoaded += OnMissionsDataLoaded;
        MissionManager.OnMissionClaimed += RefreshMissionList;

        if (dailyButton != null) dailyButton.onClick.AddListener(() => SetCategoryFilter("Daily"));
        if (mainButton != null) mainButton.onClick.AddListener(() => SetCategoryFilter("Main"));
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.currentCategoryFilter = "Daily"; // Đảm bảo bộ lọc được reset
            UpdateUI("Daily");
            RefreshMissionList();
        }
    }
    //private void Awake()
    //{
    //    SetCategoryFilter("Daily");
    //}
    void OnMissionsDataLoaded()
    {
        if (isPanelEnabled && MissionManager.Instance != null)
        {
            RefreshMissionList();
        }
    }
    void OnDestroy()
    {
        MissionManager.OnMissionsLoaded -= OnMissionsDataLoaded;
        MissionManager.OnMissionClaimed -= RefreshMissionList;

        isPanelEnabled = false;
    }
    void SetCategoryFilter(string category)
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.currentCategoryFilter = category;
            RefreshMissionList();
            UpdateUI(category);
        }
    }
    void UpdateUI(string category)
    {
        if (category == "Daily")
        {
            dailyButton.image.sprite = selectToggle;
            mainButton.image.sprite = defaultToggle;
        }
        else if (category == "Main")
        {
            dailyButton.image.sprite = defaultToggle;
            mainButton.image.sprite = selectToggle;
        }
    }
    void RefreshMissionList()
    {
        if (MissionManager.Instance != null)
        {
            // Xóa UI hiện tại
            foreach (Transform child in missionContainer)
            {
                Destroy(child.gameObject);
            }

            // Lấy danh sách nhiệm vụ theo category hiện tại
            var missionsToDisplay = MissionManager.Instance.GetMissionsByCategory(MissionManager.Instance.currentCategoryFilter);

            // Tạo UI cho các nhiệm vụ
            foreach (var mission in missionsToDisplay)
            {
                if (mission.isClaimed) continue;
                var missionUI = Instantiate(missionPrefab, missionContainer);
                 MissionManager.Instance.SetupMissionUI(missionUI, mission); // Sử dụng hàm SetupMissionUI từ MissionManager (hoặc di chuyển nó vào đây)
            }
        }
    }
}