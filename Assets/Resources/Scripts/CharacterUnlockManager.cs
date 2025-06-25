using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnlockManager : MonoBehaviour
{
    private const string UnlockKeyPrefix = "CharacterUnlocked_";

    public static void UnlockCharacter(int characterID)
    {
        PlayerPrefs.SetInt(UnlockKeyPrefix + characterID, 1);
        PlayerPrefs.Save();
    }

    public static bool IsCharacterUnlocked(int characterID)
    {
        return PlayerPrefs.GetInt(UnlockKeyPrefix + characterID, 0) == 1;
    }
}
