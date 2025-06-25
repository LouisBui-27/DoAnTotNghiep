using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplayUI : MonoBehaviour
{
    [SerializeField] private Text totalMoneyText;

    private void Start()
    {
   //     Debug.Log("MoneyDisplayUI Start: CurrencyManage Instance is " + (CurrencyManage.Instance != null ? "not null" : "null"));
        if (CurrencyManage.Instance != null)
        {
            UpdateMoneyText();
        }
    }
    private void OnEnable()
    {
     //   Debug.Log("MoneyDisplayUI OnEnable: CurrencyManage Instance is " + (CurrencyManage.Instance != null ? "not null" : "null"));

        if (CurrencyManage.Instance != null)
        {
            CurrencyManage.Instance.OnMoneyChanged += UpdateMoneyText;
            UpdateMoneyText();
        }
    }

    private void OnDestroy()
    {
        if (CurrencyManage.Instance != null)
            CurrencyManage.Instance.OnMoneyChanged -= UpdateMoneyText;
    }

    private void UpdateMoneyText()
    {
        if (totalMoneyText != null)
        {
            //Debug.Log("Cập nhật UI tiền: " + CurrencyManage.Instance.GetCurrentMoney()); // Thêm dòng này
            totalMoneyText.text = FormatNumber( CurrencyManage.Instance.GetCurrentMoney());
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
