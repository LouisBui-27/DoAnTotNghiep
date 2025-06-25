using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Các Canvas UI")]
    public GameObject shopCanvas;
    public GameObject upgradeCanvas;
    public GameObject missionCanvas;
    public GameObject playCanvas;

    [Header("Các nút chuyển tab")]
    public Button shopButton;
    public Button upgradeButton;
    public Button missionButton;
    public Button playButton;

    [Header("Sprites cho trạng thái Toggle")]
    public Sprite toggledSprite;
    public Sprite normalSprite;


    [Header("Kích thước khi Toggle")]
    public float toggledScale = 1.2f;
    public float otherScale = 0.9f;
    public float scaleDuration = 0.15f;

    private List<Button> allButtons = new List<Button>();
    void Start()
    {
        allButtons.Add(shopButton);
        allButtons.Add(upgradeButton);
        allButtons.Add(missionButton);
        allButtons.Add(playButton);

        // Gán sự kiện cho từng nút
        shopButton.onClick.AddListener(() => OnTabSelected(shopCanvas, shopButton));
        upgradeButton.onClick.AddListener(() => OnTabSelected(upgradeCanvas, upgradeButton));
        missionButton.onClick.AddListener(() => OnTabSelected(missionCanvas, missionButton));
        playButton.onClick.AddListener(() => OnTabSelected(playCanvas, playButton));

        // Mở mặc định tab đầu tiên
        OnTabSelected(playCanvas, playButton);
    }

    void OnTabSelected(GameObject canvasToOpen, Button selectedButton)
    {
        // Bật canvas tương ứng, tắt các canvas khác
        shopCanvas.SetActive(canvasToOpen == shopCanvas);
        upgradeCanvas.SetActive(canvasToOpen == upgradeCanvas);
        missionCanvas.SetActive(canvasToOpen == missionCanvas);
        playCanvas.SetActive(canvasToOpen == playCanvas);

        // Cập nhật hiệu ứng và sprite cho từng nút
        foreach (Button btn in allButtons)
        {
            bool isSelected = btn == selectedButton;
            float targetScale = isSelected ? toggledScale : otherScale;

            btn.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack);

            if (btn.GetComponent<Image>() != null)
            {
                btn.GetComponent<Image>().sprite = isSelected ? toggledSprite : normalSprite;
            }
        }
    }
}
