    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    //using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CurrencyManage : MonoBehaviour
    {
        public static CurrencyManage Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int startingMoney = 100;
        private int sessionMoney = 0;
        public event System.Action OnMoneyChanged;
        public event System.Action OnSessionMoneyChanged;

        private int currentMoney;

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

            LoadMoney();
            //UpdateUI();
        }

        private void LoadMoney()
        {
            currentMoney = PlayerPrefs.GetInt("PlayerMoney", startingMoney);
            OnMoneyChanged?.Invoke();
        }

        private void SaveMoney()
        {
            PlayerPrefs.SetInt("PlayerMoney", currentMoney);
            PlayerPrefs.Save();
        }
        public void AddMoney(int amount)
        {
            if (amount <= 0) return;

            currentMoney += amount;
            Debug.Log("current money " +currentMoney);
            SaveMoney();
            OnMoneyChanged?.Invoke();
        }

        public bool SpendMoney(int amount)
        {
            if (amount <= 0) return false;
            if (currentMoney < amount) return false;

            currentMoney -= amount;
            Debug.Log("Tiền sau khi tiêu: " + currentMoney);
            OnMoneyChanged?.Invoke();
            MissionManager.Instance.UpdateMissionProgress("mission_3", MissionManager.Instance.allMissions.Find(m => m.id == "mission_3").currentProgress + amount);
            SaveMoney();
        
            return true;
        }

        public int GetCurrentMoney()
        {
            return currentMoney;
        }

        public void ResetMoney()
        {
            currentMoney = startingMoney;
            SaveMoney();
           // UpdateUI();
        }
        public void AddSessionMoney(int amount)
        {
            if (amount <= 0) return;
            sessionMoney += amount;
            OnSessionMoneyChanged?.Invoke();
            // UpdateUISession();
        }

        public int GetSessionMoney()
        {
            return sessionMoney;
        }

        public void CommitSessionMoney()
        {
            AddMoney(sessionMoney);
            sessionMoney = 0;
            OnMoneyChanged?.Invoke();
            OnSessionMoneyChanged?.Invoke();
        }

        public void ResetSessionMoney()
        {
            sessionMoney = 0;
        }
   
    }
