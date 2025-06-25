using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AdsManager;

public class ShopManager : MonoBehaviour
{
    [Header("Các Panel Shop")]
    public GameObject materialPanel;
    public GameObject HerosPanel;

    [Header("Các nut chuyển tab")]
    public Button materialButton;   
    public Button HerosButton;

    public Sprite tabDefault;
    public Sprite tabToggle;

    [Header("Giá tiền")]
    [SerializeField] private int coin1 = 90;
    [SerializeField] private int coin2 = 150;
    [SerializeField] private int coin3 = 250;

    [Header("Số lượng Coin nhận được")]
    [SerializeField] private int coinPack1Amount = 1000;
    [SerializeField] private int coinPack2Amount = 5000;
    [SerializeField] private int coinPack3Amount = 10000;
    
    [Header("Số lượng Gem nhận được")]
    [SerializeField] private int gemPack1Amount = 10;
    [SerializeField] private int gemPack2Amount = 5000;
    [SerializeField] private int gemPack3Amount = 10000;

    // ------ Hero////
    [Header("Nút mua hero")]
    public Button buyHero1Button;
    public Button buyHero2Button;
    public Button buyHero3Button;
    public Button buyHero4Button;
    public Button buyHero5Button;
   

    [Header("Giá tiền hero")]
    [SerializeField] private int hero1 = 1000;
    [SerializeField] private int hero2 = 2000;
    [SerializeField] private int hero3 = 3000; 
    
    [Header("Giá tiền hero gem")]
    [SerializeField] private int hero1gem = 300;
    [SerializeField] private int hero2gem = 500;


    //----reroll----///
    [Header("Gía tiền reroll")]
    [SerializeField] private int rerollCoin = 250;
    [SerializeField] private int rerollGem = 100;
    [Header("Số lượng reroll nhận được")]
    [SerializeField] private int rerollGemAmount = 2;
    [SerializeField] private int rerollCoinAmount = 1;
    [Header("Button cho nut reroll")]
    [SerializeField] private Button rerollGemButton;
    [SerializeField] private Button rerollCoinButton;

    [Header("No Ads")]
    [SerializeField] private Button removeAdsButton;
    [SerializeField] int price = 200;
    [Header("Watch Ads")]
    [SerializeField] Button adCoinButton;
    [SerializeField] Button adGemButton;
    private float adCooldown = 60f;

    [Header("UI mua")]
    [SerializeField] private GameObject popupBuy;
    [SerializeField] private Text txtBuy;
    [SerializeField] private float messageDuration = 2f;
    [SerializeField] private string messageSuccess = "Mua thành công !!";
    [SerializeField] private string messagefail = "Mua thất bại !!";
    private Coroutine currentMessageCoroutine;


    private void Start()
    {
        ShowMaterialPanel();
        SetupTabButtons();
        UpdateHeroBuyButtons();
        updateNoadsButton();
        if (rerollGemButton != null)
        {
            rerollGemButton.onClick.AddListener(BuyRerollWithGem);
        }
        if(rerollCoinButton != null)
        {
            rerollCoinButton.onClick.AddListener(BuyRerollWithCoin);
        }
        if(removeAdsButton != null)
        {
            removeAdsButton.onClick.AddListener(OnRemoveAdsButton);
        }

    }
    private void Update()
    {
        var now = System.DateTime.UtcNow;
        var coinRemaining = (GetAdCooldown("CoinAdCooldown") - now).TotalSeconds;
        var gemRemaining = (GetAdCooldown("GemAdCooldown") - now).TotalSeconds;

        if (adCoinButton != null)
        {
            adCoinButton.interactable = coinRemaining <= 0;
        }

        if (adGemButton != null)
        {
            adGemButton.interactable = gemRemaining <= 0;
        }
    }
    private void SetupTabButtons()
    {
        if (materialButton != null)
            materialButton.onClick.AddListener(ShowMaterialPanel);
        else
            Debug.LogError("Chưa gán nút Material");

        if (HerosButton != null)
            HerosButton.onClick.AddListener(ShowHerosPanel);
        else
            Debug.LogError("Chưa gán nút Heros");
    }

    public void ShowMaterialPanel()
    {
        if (materialPanel != null)
        {
            materialPanel.SetActive(true);
            materialButton.image.sprite = tabToggle;
        }
       
        if(HerosPanel != null)
        {
            HerosPanel.SetActive(false);
            HerosButton.image.sprite = tabDefault;
        }
    }
    public void ShowHerosPanel()
    {
        if (HerosPanel != null)
        {
            HerosPanel.SetActive(true);
            HerosButton.image.sprite = tabToggle;
        }
      
        if (materialPanel != null)
        {
            materialPanel.SetActive(false);
            materialButton.image.sprite = tabDefault;
        }
    }
    public void OnBuyCoinItem1Button() => OnWatchAdToGetCoinButton();
    public void OnBuyCoinItem2Button() => BuyCoinItem(coin2, coinPack2Amount);
    public void OnBuyCoinItem3Button() => BuyCoinItem(coin3, coinPack3Amount);
    public void OnBuyGemItem1Button() => OnWatchAdToGetGemButton();

    private void BuyCoinItem(int price, int amount)
    {
        if (GemManager.Instance.GetCurrentGems() >= price)
        {
            GemManager.Instance.SpendGems(price);
            CurrencyManage.Instance.AddMoney(amount);
            ShowShopMessage(messageSuccess);
        }
        else
        {
            ShowShopMessage(messagefail);
        }
    }
    //public void OnWatchAdToGetCoinsButton(int coinAmount, int gemAmount)
    //{
    //    if (AdsManager.Instance != null)
    //    {
    //        AdsManager.Instance.ShowRewardedAd(() => // Truyền lambda expression (callback)
    //        {
    //            if (coinAmount > 0)
    //            {
    //                CurrencyManage.Instance.AddMoney(coinAmount);
    //                Debug.Log($"Người chơi đã xem quảng cáo và nhận được {coinAmount} xu");
    //                // Thêm bất kỳ logic UI nào cần thiết sau khi cấp thưởng (ví dụ: cập nhật hiển thị tiền
    //            }
    //            else if (gemAmount > 0)
    //            {
    //                GemManager.Instance.AddGems(gemAmount);
    //            }
    //        });
    //    }
    //    else
    //    {
    //        Debug.LogWarning("AdsManager is not initialized. Cannot show rewarded ad.");
    //        // Trong môi trường debug, bạn có thể cấp thưởng ngay lập tức:
    //        // CurrencyManage.Instance.AddMoney(coinAmount);
    //        // Debug.Log($"[DEBUG] Cấp {coinAmount} xu ngay lập tức vì AdsManager không tồn tại.");
    //    }
    //}
    public void OnWatchAdToGetCoinButton()
    {
        if (System.DateTime.UtcNow < GetAdCooldown("CoinAdCooldown"))
        {
            Debug.Log("Chưa hết thời gian cooldown coin ad");
            return;
        }

        AdsManager.Instance?.ShowRewardedAd(() =>
        {
            CurrencyManage.Instance.AddMoney(coinPack1Amount);
            SetAdCooldown("CoinAdCooldown", adCooldown);
            ShowShopMessage(messageSuccess);
        });
    }

    public void OnWatchAdToGetGemButton()
    {
        if (System.DateTime.UtcNow < GetAdCooldown("GemAdCooldown"))
        {
            Debug.Log("Chưa hết thời gian cooldown gem ad");
            return;
        }

        AdsManager.Instance?.ShowRewardedAd(() =>
        {
            GemManager.Instance.AddGems(gemPack1Amount);
            SetAdCooldown("GemAdCooldown", adCooldown);
            ShowShopMessage(messageSuccess);
        });
    }
    public void OnBuyHero1Button() => BuyHero(1, hero1);
    public void OnBuyHero2Button() => BuyHero(2, hero2);
    public void OnBuyHero3Button() => BuyHero(3, hero3);
    public void OnBuyHero4Button() => BuyHeroGem(4, hero1gem);
    public void OnBuyHero5Button() => BuyHeroGem(5, hero2gem);


    private void BuyHero(int characterID, int price)
    {
        if (CharacterUnlockManager.IsCharacterUnlocked(characterID))
        {
            Debug.Log("Hero đã được mở khóa");
            return;
        }

        if (CurrencyManage.Instance.GetCurrentMoney() >= price)
        {
            CurrencyManage.Instance.SpendMoney(price);
            CharacterUnlockManager.UnlockCharacter(characterID);
            ShowShopMessage(messageSuccess);
        }
        else
        {
            ShowShopMessage(messagefail);
        }
        UpdateHeroBuyButtons();
    }  
    private void BuyHeroGem(int characterID, int price)
    {
        if (CharacterUnlockManager.IsCharacterUnlocked(characterID))
        {
            Debug.Log("Hero đã được mở khóa");
            return;
        }

        if (GemManager.Instance.GetCurrentGems() >= price)
        {
            GemManager.Instance.SpendGems(price);
            CharacterUnlockManager.UnlockCharacter(characterID);
            ShowShopMessage(messageSuccess);
        }
        else
        {
            ShowShopMessage(messagefail);
        }
        UpdateHeroBuyButtons();
    }
    private void UpdateHeroBuyButtons()
    {
        if (buyHero1Button != null)
            buyHero1Button.interactable = !CharacterUnlockManager.IsCharacterUnlocked(1);

        if (buyHero2Button != null)
            buyHero2Button.interactable = !CharacterUnlockManager.IsCharacterUnlocked(2);

        if (buyHero3Button != null)
            buyHero3Button.interactable = !CharacterUnlockManager.IsCharacterUnlocked(3); 
        if (buyHero4Button != null)
            buyHero4Button.interactable = !CharacterUnlockManager.IsCharacterUnlocked(4); 
        if (buyHero5Button != null)
            buyHero5Button.interactable = !CharacterUnlockManager.IsCharacterUnlocked(5);
      

    }
    void updateNoadsButton()
    {
        removeAdsButton.interactable = (!AdsManager.HasRemovedAds);
    } 
    private void BuyRerollWithGem()
    {
        if (GemManager.Instance.GetCurrentGems() >= rerollGem)
        {
            GemManager.Instance.SpendGems(rerollGem);
            RerollManager.Instance.AddReroll(rerollGemAmount);
            ShowShopMessage(messageSuccess);
        }
        else
        {
            ShowShopMessage(messagefail);
        }
    }
    private void BuyRerollWithCoin()
    {
        if (CurrencyManage.Instance.GetCurrentMoney() >= rerollCoin)
        {
            CurrencyManage.Instance.SpendMoney(rerollCoin);
            RerollManager.Instance.AddReroll(rerollCoinAmount);
            ShowShopMessage(messageSuccess);
        }
        else
        {
            ShowShopMessage(messagefail);
        }
    }
    public void OnRemoveAdsButton()
    {
      //  int price = 200;
        if (GemManager.Instance.GetCurrentGems()>=price)
        {
           
            GemManager.Instance.SpendGems(price);
            PlayerPrefs.SetInt(PlayerPrefsKeys.RemoveAds, 1);
            PlayerPrefs.Save();
            removeAdsButton.interactable = (!AdsManager.HasRemovedAds);
            Debug.Log("Player purchased Remove Ads.");
            // TODO: Cập nhật UI để ẩn nút "Mua Loại bỏ quảng cáo"
            ShowShopMessage(messageSuccess);
        }
        else
        {
            Debug.Log("Not enough gems to purchase Remove Ads.");
            ShowShopMessage(messagefail);
            // TODO: Hiển thị thông báo không đủ gem
        }
    }
    private void SetAdCooldown(string key, float seconds)
    {
        var cooldownTime = System.DateTime.UtcNow.AddSeconds(seconds);
        PlayerPrefs.SetString(key, cooldownTime.ToString());
        PlayerPrefs.Save();
    }

    private System.DateTime GetAdCooldown(string key)
    {
        string timeStr = PlayerPrefs.GetString(key, "");
        if (System.DateTime.TryParse(timeStr, out var result))
            return result;
        return System.DateTime.MinValue;
    }
   
    private void ShowShopMessage(string message)
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }
        currentMessageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        txtBuy.text = message;
        popupBuy.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        popupBuy.SetActive(false);
    }
}
