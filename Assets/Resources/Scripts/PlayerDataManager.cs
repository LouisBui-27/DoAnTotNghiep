using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public CharacterData SelectedCharacter { get; private set; }

    public List<CharacterData> characterList; // Gán list này giống như trong CharacterSpawner

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

        LoadSelectedCharacter(); // Đọc từ PlayerPrefs khi bắt đầu game
    }

    public void LoadSelectedCharacter()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterList != null && index >= 0 && index < characterList.Count)
        {
            SelectedCharacter = characterList[index];
            Debug.Log(SelectedCharacter);
        }
        else
        {
            Debug.LogError("Invalid character index or character list not set in PlayerDataManager.");
        }
    }
    public void SetSelectedCharacter(int index)
    {
        if (characterList != null && index >= 0 && index < characterList.Count)
        {
            SelectedCharacter = characterList[index];
            PlayerPrefs.SetInt("SelectedCharacter", index);
        }
    }
}
