using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Button backButton;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    // THAY ĐỔI TỪ TOGGLE SANG BUTTON CHO NHẠC VÀ SFX
    public Button musicButton;       // Kéo ButtonMusic (có component Button) vào đây
    public Button sfxButton;         // Kéo ButtonSFX (có component Button) vào đây

    [Header("Music Button Sprites")]
    public Sprite musicOnSprite;     // Kéo sprite khi nhạc BẬT (loa có tiếng) vào đây
    public Sprite musicOffSprite;    // Kéo sprite khi nhạc TẮT (loa có dấu X hoặc gạch ngang) vào đây

    [Header("SFX Button Sprites")]
    public Sprite sfxOnSprite;       // Kéo sprite khi SFX BẬT vào đây
    public Sprite sfxOffSprite;      // Kéo sprite khi SFX TẮT vào đây

    private float previousMusicVolume = 1f;
    private float previousSFXVolume = 1f;

    [SerializeField] private PopupScaler popupScaler;
    void Awake()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    void OnEnable()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("SettingsPanelManager: AudioManager.Instance is null. Make sure AudioManager is present in the scene and persistent.");
            return;
        }

        // 1. Cập nhật giá trị Slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // 2. Cập nhật trạng thái Button và đăng ký sự kiện
        // Music Button
        if (musicButton != null)
        {
            musicButton.onClick.AddListener(ToggleMusicMute); // Đăng ký sự kiện click
            UpdateMusicButtonSprite(); // Cập nhật hình ảnh ban đầu
        }
        // SFX Button
        if (sfxButton != null)
        {
            sfxButton.onClick.AddListener(ToggleSFXMute); // Đăng ký sự kiện click
            UpdateSFXButtonSprite(); // Cập nhật hình ảnh ban đầu
        }

        // 3. Đăng ký sự kiện click cho nút Back
        if (backButton != null)
        {
            backButton.onClick.AddListener(HideSettingsPanel);
        }
    }

    void OnDisable()
    {
        // Hủy đăng ký các sự kiện để tránh lỗi và rò rỉ bộ nhớ
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        }
        if (musicButton != null)
        {
            musicButton.onClick.RemoveListener(ToggleMusicMute);
        }
        if (sfxButton != null)
        {
            sfxButton.onClick.RemoveListener(ToggleSFXMute);
        }
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(HideSettingsPanel);
        }
    }

    // --- Các phương thức được gọi bởi UI ---

    public void SetMusicVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
            UpdateMusicButtonSprite(); // Cập nhật sprite khi âm lượng thay đổi (dùng nếu bạn muốn nút mute tự chuyển trạng thái)
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
            UpdateSFXButtonSprite(); // Cập nhật sprite khi âm lượng thay đổi
        }
    }

    // Bật/Tắt nhạc nền (gọi bởi Button Music)
    public void ToggleMusicMute()
    {
        if (AudioManager.Instance != null && AudioManager.Instance._musicSource != null)
        {
            bool isMuted = !AudioManager.Instance._musicSource.mute;
            AudioManager.Instance.ToggleMusic(isMuted);

            // Đồng bộ slider (nếu muốn slider về 0 khi mute)
            if (musicVolumeSlider != null)
            {
                if (isMuted)
                {
                    previousMusicVolume = AudioManager.Instance.GetMusicVolume();
                    musicVolumeSlider.value = 0f;
                    AudioManager.Instance.SetMusicVolume(0f);
                }
                else
                {
                    musicVolumeSlider.value = previousMusicVolume;
                    AudioManager.Instance.SetMusicVolume(previousMusicVolume);
                }

            }

            UpdateMusicButtonSprite();
        }
    }

    // Bật/Tắt hiệu ứng âm thanh (gọi bởi Button SFX)
    public void ToggleSFXMute()
    {
        if (AudioManager.Instance != null && AudioManager.Instance._sfxSource != null)
        {
            // Đảo ngược trạng thái mute hiện tại
            bool isMuted = !AudioManager.Instance._sfxSource.mute;
            AudioManager.Instance.ToggleSFX(isMuted); // Gọi ToggleSFX với trạng thái mới
            if (sfxVolumeSlider != null)
            {
                if (isMuted)
                {
                    previousSFXVolume = AudioManager.Instance.GetSFXVolume();
                    sfxVolumeSlider.value = 0f;
                    AudioManager.Instance.SetSFXVolume(0f);
                }
                else
                {
                    sfxVolumeSlider.value = previousSFXVolume;
                    AudioManager.Instance.SetSFXVolume(previousSFXVolume);
                }

            }
            UpdateSFXButtonSprite(); // Cập nhật hình ảnh nút
        }
    }

    // --- Các phương thức hỗ trợ cập nhật Sprite ---

    private void UpdateMusicButtonSprite()
    {
        if (musicButton != null && musicButton.image != null && musicOnSprite != null && musicOffSprite != null)
        {
            // Kiểm tra trạng thái mute của AudioSource hoặc nếu âm lượng bằng 0
            if (AudioManager.Instance != null)
            {
                if (AudioManager.Instance._musicSource.mute || AudioManager.Instance.GetMusicVolume() <= 0.01f) // Kiểm tra cả mute và âm lượng
                {
                    musicButton.image.sprite = musicOffSprite;
                }
                else
                {
                    musicButton.image.sprite = musicOnSprite;
                }
            }
        }
    }

    private void UpdateSFXButtonSprite()
    {
        if (sfxButton != null && sfxButton.image != null && sfxOnSprite != null && sfxOffSprite != null)
        {
            // Kiểm tra trạng thái mute của AudioSource hoặc nếu âm lượng bằng 0
            if (AudioManager.Instance != null)
            {
                if (AudioManager.Instance._sfxSource.mute || AudioManager.Instance.GetSFXVolume() <= 0.01f) // Kiểm tra cả mute và âm lượng
                {
                    sfxButton.image.sprite = sfxOffSprite;
                }
                else
                {
                    sfxButton.image.sprite = sfxOnSprite;
                }
            }
        }
    }


    // Hiển thị panel cài đặt
    public void ShowSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            popupScaler.PlayScaleIn();
            // Cập nhật trạng thái sprite mỗi khi panel được mở
            UpdateMusicButtonSprite();
            UpdateSFXButtonSprite();
            // Có thể thêm hiệu ứng fade-in cho panel ở đây bằng DOTween
        }
    }

    // Ẩn panel cài đặt
    public void HideSettingsPanel()
    {
        if (settingsPanel != null)
        {
            popupScaler.HidePopup(() =>
            {
                settingsPanel.SetActive(false);
            });
       
            // Có thể thêm hiệu ứng fade-out cho panel ở đây bằng DOTween
        }
    }

}
