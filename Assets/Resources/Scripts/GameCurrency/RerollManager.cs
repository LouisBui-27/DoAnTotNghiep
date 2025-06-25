using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollManager : MonoBehaviour
{
    public static RerollManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int defaultRerolls = 5;

    private int currentRerolls;

    public event System.Action OnRerollChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRerolls();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadRerolls()
    {
        currentRerolls = PlayerPrefs.GetInt("PlayerRerolls", defaultRerolls);
        OnRerollChanged?.Invoke();
    }

    private void SaveRerolls()
    {
        PlayerPrefs.SetInt("PlayerRerolls", currentRerolls);
        PlayerPrefs.Save();
    }

    public int GetCurrentRerolls()
    {
        return currentRerolls;
    }

    public void AddReroll(int amount)
    {
        if (amount <= 0) return;
        currentRerolls += amount;
        SaveRerolls();
        OnRerollChanged?.Invoke();
    }

    public bool UseReroll()
    {
        if (currentRerolls <= 0) return false;
        currentRerolls--;
        SaveRerolls();
        OnRerollChanged?.Invoke();
        return true;
    }

    public void ResetRerolls()
    {
        currentRerolls = defaultRerolls;
        SaveRerolls();
        OnRerollChanged?.Invoke();
    }
}
