using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int startingGems = 0;
    private int currentGems;

    public event System.Action OnGemChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadGems();
    }

    private void LoadGems()
    {
        currentGems = PlayerPrefs.GetInt("PlayerGems", startingGems);
        OnGemChanged?.Invoke();
    }

    private void SaveGems()
    {
        PlayerPrefs.SetInt("PlayerGems", currentGems);
        PlayerPrefs.Save();
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;

        currentGems += amount;
        Debug.Log("current gems " + currentGems);
        SaveGems();
        OnGemChanged?.Invoke();
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0) return false;
        if (currentGems < amount) return false;

        currentGems -= amount;
        Debug.Log("Gems sau khi tiêu: " + currentGems);
        OnGemChanged?.Invoke();
        SaveGems();

        return true;
    }

    public int GetCurrentGems()
    {
        return currentGems;
    }

    public void ResetGems()
    {
        currentGems = startingGems;
        SaveGems();
    }
}
