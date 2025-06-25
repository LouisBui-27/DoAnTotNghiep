using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
   // public static CharacterSelectManager instance;
    public List<CharacterData> availableCharacters;
    public Transform buttonContainer;
    public GameObject buttonPrefab;
    private int selectedCharacterIndex = -1;
    [SerializeField] PopupScaler PopupScaler;
    private void OnEnable()
    {
        PopulateCharacterButtons();
    }

    void PopulateCharacterButtons()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < availableCharacters.Count; i++)
        {
            CharacterData character = availableCharacters[i];
            if (!CharacterUnlockManager.IsCharacterUnlocked(character.characterID))
                continue; // Bỏ qua nếu chưa mở khóa

            int index = i;
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
           // buttonObj.GetComponentInChildren<Text>().text = availableCharacters[i].characterName;
            Image iconImage = buttonObj.transform.Find("Image").GetComponent<Image>();
            iconImage.sprite = character.icon;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedCharacterIndex = index;
                PlayerDataManager.Instance.SetSelectedCharacter(index);
                Debug.Log("Selected: " + character.characterID);
            });
        }
    }

    public void OnPlayButton()
    {
        if (selectedCharacterIndex >= 0)
        {
            PlayerPrefs.SetInt("SelectedCharacter", selectedCharacterIndex);

            int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1); // Mặc định Level1 nếu không có

            UnityEngine.SceneManagement.SceneManager.LoadScene("Level" + selectedLevel);
        }
    }
    public void OnBackButton()
    {
        PopupScaler.HidePopup(() =>
        {
        gameObject.SetActive(false);
        });
        
    }
}
