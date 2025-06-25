using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject selectLevelPanel;
    public GameObject characterSelectPanel;
    public Button backButton;           
    public Button playSelectedLevelButton; 
    public Button Play;
    public Button btnChon;
    public Button Setting;
    public Button History;

    [Header("Level Buttons")]
    public Button level1Button; // Kéo BtnLevel1 vào đây
    public Button level2Button; // Kéo BtnLevel2 vào đây
    public Button level3Button; // Kéo BtnLevel3 vào đây

    [Header("Level Button Images (để hiển thị icon level)")]
    public Image level1ImageDisplay; // Kéo Image con của BtnLevel1 (hiển thị icon màn)
    public Image level2ImageDisplay; // Kéo Image con của BtnLevel2
    public Image level3ImageDisplay; // Kéo Image con của BtnLevel3

    [Header("Selected Level Display (trên ButtonPlay của CanvasDefault)")]
    public Image mainPlayButtonImage; // Kéo Image component của ButtonPlay trong CanvasDefault vào đây

    [Header("Level Icons (Sprites của từng màn chơi)")]
    public Sprite level1Icon; // Kéo sprite icon cho Level 1
    public Sprite level2Icon; // Kéo sprite icon cho Level 2
    public Sprite level3Icon; // Kéo sprite icon cho Level 3

    private int selectedLevelID = -1; // ID của level đang được chọn
    [SerializeField] SlideInOutAnimator slideInOutAnimator;

    void Awake()
    {
        if (selectLevelPanel != null)
        {
            selectLevelPanel.SetActive(false);
        }
        if (characterSelectPanel != null)
        {
            characterSelectPanel.SetActive(false); // Đảm bảo panel chọn nhân vật cũng ẩn khi bắt đầu
        }
    }

    void OnEnable()
    {
        // Đăng ký sự kiện click cho các nút level
        if (level1Button != null) level1Button.onClick.AddListener(() => SelectLevel(1));
        if (level2Button != null) level2Button.onClick.AddListener(() => SelectLevel(2));
        if (level3Button != null) level3Button.onClick.AddListener(() => SelectLevel(3));

        // Đăng ký sự kiện click cho nút Back và nút Play/Chọn
        if (backButton != null) backButton.onClick.AddListener(HideLevelSelectPanel);
        if (playSelectedLevelButton != null) playSelectedLevelButton.onClick.AddListener(ShowLevelSelectPanel);
        if (Play != null) Play.onClick.AddListener(OpenCharacterSelectPanel);
        if(btnChon != null) btnChon.onClick.AddListener(HideLevelSelectPanel);

        // Cập nhật trạng thái các nút level khi panel được bật
        UpdateLevelButtonsState();
        UpdatePlayButtonState(); // Cập nhật trạng thái nút Play/Chọn
        // Chọn level 1 mặc định khi mở panel nếu chưa có level nào được chọn
        if (selectedLevelID == -1)
        {
            SelectLevel(1);
        }
    }

    void OnDisable()
    {
        // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ
        if (level1Button != null) level1Button.onClick.RemoveAllListeners();
        if (level2Button != null) level2Button.onClick.RemoveAllListeners();
        if (level3Button != null) level3Button.onClick.RemoveAllListeners();

        if (backButton != null) backButton.onClick.RemoveAllListeners();
        if (playSelectedLevelButton != null) playSelectedLevelButton.onClick.RemoveAllListeners();
    }

    void SelectLevel(int levelID)
    {
        if (LevelUnlock.IsLevelUnlocked(levelID))
        {
            selectedLevelID = levelID;
            PlayerPrefs.SetInt("SelectedLevel", levelID);
            Debug.Log($"Selected Level: {levelID}");

            // Cập nhật hình ảnh trên nút Play chính ở CanvasDefault
            UpdateMainPlayButtonImage(levelID);

            // TODO: Thêm hiệu ứng hình ảnh cho nút level được chọn trong panel SelectLevel
            // Ví dụ: Highlight viền, scale nhẹ nút được chọn
            HighlightSelectedLevelButton(levelID);

            UpdatePlayButtonState(); // Cập nhật trạng thái nút Play/Chọn
        }
        else
        {
            Debug.LogWarning($"Level {levelID} is locked!");
            // TODO: Hiển thị thông báo "Level bị khóa" cho người chơi
            selectedLevelID = -1; // Đảm bảo không chọn level bị khóa
            UpdatePlayButtonState(); // Vô hiệu hóa nút Play/Chọn
        }
    }

    void HighlightSelectedLevelButton(int selectedID)
    {
        // Reset tất cả các nút về trạng thái bình thường
        ResetLevelButtonHighlight(level1Button);
        ResetLevelButtonHighlight(level2Button);
        ResetLevelButtonHighlight(level3Button);

        // Highlight nút được chọn
        switch (selectedID)
        {
            case 1: ApplyHighlight(level1Button); break;
            case 2: ApplyHighlight(level2Button); break;
            case 3: ApplyHighlight(level3Button); break;
        }
    }

    void ApplyHighlight(Button btn)
    {
        if (btn != null)
        {
            // Ví dụ: Thay đổi màu sắc hoặc scale
            // btn.image.color = Color.yellow;
            // btn.transform.DOScale(1.1f, 0.1f);
            // Hoặc sử dụng một Sprite highlight riêng
        }
    }

    void ResetLevelButtonHighlight(Button btn)
    {
        if (btn != null)
        {
            // btn.image.color = Color.white;
            // btn.transform.DOScale(1f, 0.1f);
        }
    }


    void LoadSelectedLevel()
    {
        if (selectedLevelID != -1 && LevelUnlock.IsLevelUnlocked(selectedLevelID))
        {
            // Gọi Loading script để tải scene
            Loading.LoadGameScene("Level" + selectedLevelID); // Ví dụ: "Level1", "Level2"
        }
        else
        {
            Debug.LogWarning("Chưa chọn level hoặc level được chọn bị khóa!");
            // TODO: Hiển thị thông báo cho người chơi
        }
    }

    void UpdateLevelButtonsState()
    {
        // Level 1
        if (level1Button != null)
        {
            bool unlocked = LevelUnlock.IsLevelUnlocked(1);
            level1Button.interactable = unlocked;
            
            // Cập nhật icon hiển thị bên trong nút level
            //if (level1ImageDisplay != null) level1ImageDisplay.sprite = level1Icon;
            // Level 1 không có overlay khóa
        }

        // Level 2
        if (level2Button != null)
        {
            bool unlocked = LevelUnlock.IsLevelUnlocked(2);
            level2Button.interactable = unlocked;
           
            //if (level2ImageDisplay != null) level2ImageDisplay.sprite = level2Icon;
        }

        // Level 3
        if (level3Button != null)
        {
            bool unlocked = LevelUnlock.IsLevelUnlocked(3);
            level3Button.interactable = unlocked;
           
           // if (level3ImageDisplay != null) level3ImageDisplay.sprite = level3Icon;
        }
    }

    void UpdatePlayButtonState()
    {
        if (playSelectedLevelButton != null)
        {
            // Nút "Chọn" chỉ hoạt động nếu một level hợp lệ đã được chọn
            playSelectedLevelButton.interactable = (selectedLevelID != -1 && LevelUnlock.IsLevelUnlocked(selectedLevelID));
        }
    }

    // Cập nhật hình ảnh trên nút Play chính của CanvasDefault
    void UpdateMainPlayButtonImage(int levelID)
    {
        if (mainPlayButtonImage != null)
        {
            switch (levelID)
            {
                case 1: mainPlayButtonImage.sprite = level1Icon; break;
                case 2: mainPlayButtonImage.sprite = level2Icon; break;
                case 3: mainPlayButtonImage.sprite = level3Icon; break;
                default: mainPlayButtonImage.sprite = null; break; // Hoặc một sprite mặc định
            }
        }
    }

    public void ShowLevelSelectPanel()
    {
        if (selectLevelPanel != null)
        {
            selectLevelPanel.SetActive(true);
            Setting.gameObject.SetActive(false);
            History.gameObject.SetActive(false);
            // selectedLevelID = -1; // Không reset nếu bạn muốn giữ lựa chọn trước đó
            UpdateLevelButtonsState(); // Cập nhật trạng thái các nút (khóa/mở)
            UpdatePlayButtonState(); // Cập nhật trạng thái nút Play/Chọn (interactable)
            // Đảm bảo hình ảnh nút Play chính được cập nhật khi mở panel
            UpdateMainPlayButtonImage(selectedLevelID == -1 ? 1 : selectedLevelID); // Mặc định hiển thị level 1 nếu chưa chọn
        }
    }
    void OpenCharacterSelectPanel()
    {
        if (selectedLevelID != -1 && LevelUnlock.IsLevelUnlocked(selectedLevelID))
        {
            if (characterSelectPanel != null)
                characterSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Chưa chọn level hoặc level bị khóa.");
        }
    }
    public void HideLevelSelectPanel()
    {
        if (selectLevelPanel != null)
        {
            slideInOutAnimator.PlaySlideOut(() =>
            {
                selectLevelPanel.SetActive(false);
                Setting.gameObject.SetActive(true);
                History.gameObject.SetActive(true );
            });
            
        }
    }
}
