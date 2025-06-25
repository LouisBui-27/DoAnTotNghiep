using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionMoneyUI : MonoBehaviour
{
    [SerializeField] private Text sessionMoneyText;

    private void OnEnable()
    {
        if (CurrencyManage.Instance != null)
        {
            CurrencyManage.Instance.OnSessionMoneyChanged += UpdateSessionText;
            UpdateSessionText();
        }
    }

    private void OnDestroy()
    {
        if (CurrencyManage.Instance != null)
            CurrencyManage.Instance.OnSessionMoneyChanged -= UpdateSessionText;
    }

    private void UpdateSessionText()
    {
        if (sessionMoneyText != null)
            sessionMoneyText.text = CurrencyManage.Instance.GetSessionMoney().ToString("N0");
    }
}
