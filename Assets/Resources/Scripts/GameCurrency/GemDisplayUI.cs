using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GemDisplayUI : MonoBehaviour
{
    [SerializeField] private Text totalGemsText;

    private void Start()
    {
        if (GemManager.Instance != null)
        {
            UpdateGemsText();
        }
    }

    private void OnEnable()
    {
        if (GemManager.Instance != null)
        {
            GemManager.Instance.OnGemChanged += UpdateGemsText;
            UpdateGemsText();
        }
    }

    private void OnDestroy()
    {
        if (GemManager.Instance != null)
            GemManager.Instance.OnGemChanged -= UpdateGemsText;
    }

    private void UpdateGemsText()
    {
        if (totalGemsText != null)
        {
            totalGemsText.text = FormatNumber(GemManager.Instance.GetCurrentGems());
        }
    }
    private string FormatNumber(int number)
    {
        if (number >= 1000000)
        {
            return (number / 1000000f).ToString("F1") + "M";
        }
        else if (number >= 1000)
        {
            return (number / 1000f).ToString("F1") + "K";
        }
        else
        {
            return number.ToString(); // Hiển thị số nguyên
        }
    }
}
