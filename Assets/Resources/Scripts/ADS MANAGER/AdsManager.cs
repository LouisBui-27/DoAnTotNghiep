using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager Instance;
    public static class PlayerPrefsKeys
    {
        public const string RemoveAds = "REMOVE_ADS";
    }
    [SerializeField] private string _androidGameId = "5865542"; 
    [SerializeField] private string _iOSGameId = "5865543";      
    private string _gameId; // ID game được chọn dựa trên nền tảng hiện tại
    [Header("Ad Unit IDs (Placement IDs)")]
    [SerializeField] private string _androidInterstitialAdUnitId = "Interstitial_Android"; // Thay thế bằng ID thực tế
    [SerializeField] private string _iOSInterstitialAdUnitId = "Interstitial_iOS";       // Thay thế bằng ID thực tế
    private string _interstitialAdUnitId;
    
    [SerializeField] private string _androidRewardedAdUnitId = "Rewarded_Android";       // Thay thế bằng ID thực tế
    [SerializeField] private string _iOSRewardedAdUnitId = "Rewarded_iOS";           // Thay thế bằng ID thực tế
    private string _rewardedAdUnitId;

    [Header("Settings")]
    [Tooltip("Bật chế độ Test Mode để xem quảng cáo thử nghiệm. Tắt khi build thật.")]
    [SerializeField] private bool _testMode = true; // Chế độ test để không ảnh hưởng đến doanh thu thật
    public static Action OnRewardedAdFinished;
    public static Action OnRewardedAdSkipped; 
    public static Action OnRewardedAdFailed;

    public static bool HasRemovedAds => PlayerPrefs.GetInt(PlayerPrefsKeys.RemoveAds, 0) == 1;

    private Action _onRewardedCallback;
    private bool isInterstitialReady = false;
    private bool isRewardedReady = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ GameObject này tồn tại giữa các Scene
            InitializeAds(); // Bắt đầu quá trình khởi tạo quảng cáo
        }
        else
        {
            Destroy(gameObject); // Nếu đã có Instance, hủy bỏ đối tượng mới
        }
    }
 
    void InitializeAds()
    {
#if UNITY_ANDROID
        _gameId = _androidGameId;
        _interstitialAdUnitId = _androidInterstitialAdUnitId;
        _rewardedAdUnitId = _androidRewardedAdUnitId;
#elif UNITY_IOS
    _gameId = _iOSGameId;
    _interstitialAdUnitId = _iOSInterstitialAdUnitId;
    _rewardedAdUnitId = _iOSRewardedAdUnitId;
#else
    Debug.LogWarning("Unsupported platform for Unity Ads");
#endif

        // Khởi tạo Unity Ads SDK với Game ID, chế độ test và listener hiện tại
        Advertisement.Initialize(_gameId, _testMode, this);
        Debug.Log("Initializing Unity Ads...");
    }
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Initialization Complete.");
        // Sau khi khởi tạo thành công, hãy tải ngay các quảng cáo để sẵn sàng hiển thị
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    public void LoadInterstitialAd()
    {
        Debug.Log("Loading Interstitial Ad: " + _interstitialAdUnitId);
        isInterstitialReady = false;
        Advertisement.Load(_interstitialAdUnitId, this); // 'this' là listener cho việc tải
    }

    // Tải quảng cáo có thưởng
    public void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded Ad: " + _rewardedAdUnitId);
        isRewardedReady = false;
        Advertisement.Load(_rewardedAdUnitId, this); // 'this' là listener cho việc tải
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Ad Loaded: " + placementId);
        if (placementId == _interstitialAdUnitId)
            isInterstitialReady = true;
        else if (placementId == _rewardedAdUnitId)
            isRewardedReady = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {placementId}: {error} - {message}");
        if (placementId == _interstitialAdUnitId)
            isInterstitialReady = false;
        else if (placementId == _rewardedAdUnitId)
            isRewardedReady = false;
    }

    public void ShowInterstitialAd()
    {
        if (HasRemovedAds)
        {
            Debug.Log("Ads removed. Interstitial will not be shown.");
            return;
        }
        // Kiểm tra xem quảng cáo đã sẵn sàng chưa
        if (isInterstitialReady)
        {
            Debug.Log("Showing Interstitial Ad: " + _interstitialAdUnitId);
            Advertisement.Show(_interstitialAdUnitId, this); // 'this' là listener cho việc hiển thị
        }
        else
        {
            Debug.LogWarning("Interstitial Ad is not ready yet. Trying to load it again.");
            LoadInterstitialAd(); // Nếu chưa sẵn sàng, thử tải lại
        }
    }

    // Hiển thị quảng cáo có thưởng
    public void ShowRewardedAd(Action onRewardedCallback = null) // Thêm tham số Action tùy chọn
    {
        if (isRewardedReady)
        {
            Debug.Log("Showing Rewarded Ad: " + _rewardedAdUnitId);
            _onRewardedCallback = onRewardedCallback; // Lưu trữ callback
            Advertisement.Show(_rewardedAdUnitId, this);
        }
        else
        {
            Debug.LogWarning("Rewarded Ad is not ready yet. Trying to load it again. No reward will be granted without a successful show.");
            _onRewardedCallback = null; // Đảm bảo callback được reset nếu không hiển thị
            LoadRewardedAd();
        }
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Ad Clicked: " + placementId);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Ad Show Complete: {placementId} - State: {showCompletionState}");

        // Tiếp tục game, âm thanh sau khi quảng cáo kết thúc
        Time.timeScale = 1f;
        AudioListener.pause = false;
        // AudioListener.volume = 1f; // Khôi phục âm lượng

        // Xử lý logic cấp thưởng cho quảng cáo có thưởng
        if (placementId == _rewardedAdUnitId)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Rewarded Ad successfully shown. Granting reward.");
                OnRewardedAdFinished?.Invoke(); // Gọi event để cấp thưởng.
                _onRewardedCallback?.Invoke();
            }
            else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
            {
                Debug.Log("Rewarded Ad skipped. No reward granted.");
                OnRewardedAdSkipped?.Invoke(); // Gọi event khi người dùng bỏ qua
            }
            else if (showCompletionState == UnityAdsShowCompletionState.UNKNOWN)
            {
                Debug.LogWarning("Rewarded Ad completion state is UNKNOWN. Consider not granting reward.");
            }
            _onRewardedCallback = null;
        }

        // Luôn tải lại quảng cáo sau khi nó đã được hiển thị để sẵn sàng cho lần tiếp theo
        if (placementId == _interstitialAdUnitId)
        {
            LoadInterstitialAd();
        }
        else if (placementId == _rewardedAdUnitId)
        {
            LoadRewardedAd();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
        // Thông báo lỗi cho người dùng nếu cần
        if (placementId == _rewardedAdUnitId)
        {
            OnRewardedAdFailed?.Invoke(); // Gọi event thông báo lỗi cho rewarded ad
        }
        // Thử tải lại quảng cáo sau khi hiển thị thất bại
        if (placementId == _interstitialAdUnitId)
        {
            LoadInterstitialAd();
        }
        else if (placementId == _rewardedAdUnitId)
        {
            LoadRewardedAd();
        }

        _onRewardedCallback = null;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Ad Show Start: " + placementId);
        if (placementId == _interstitialAdUnitId)
            isInterstitialReady = false;
        else if (placementId == _rewardedAdUnitId)
            isRewardedReady = false;
        // Tạm dừng game, âm thanh, v.v. khi quảng cáo bắt đầu hiển thị
        Time.timeScale = 0f; // Tạm dừng game
        AudioListener.pause = true; // Tạm dừng âm thanh
        // AudioListener.volume = 0f; // Hoặc giảm âm lượng
    }
}
