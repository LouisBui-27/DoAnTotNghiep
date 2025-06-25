using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RerollDisplayUI : MonoBehaviour
{
    [SerializeField] private Text totalGemsText;

    private void Start()
    {
        if (RerollManager.Instance != null)
        {
            UpdateRerollText();
        }
    }

    private void OnEnable()
    {
        if (RerollManager.Instance != null)
        {
            RerollManager.Instance.OnRerollChanged += UpdateRerollText;
            UpdateRerollText();
        }
    }

    private void OnDestroy()
    {
        if (RerollManager.Instance != null)
            RerollManager.Instance.OnRerollChanged -= UpdateRerollText;
    }

    private void UpdateRerollText()
    {
        if (totalGemsText != null)
        {
            totalGemsText.text = RerollManager.Instance.GetCurrentRerolls().ToString();
        }
    }
}
