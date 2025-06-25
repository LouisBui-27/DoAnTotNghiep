using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialCharacterUnlocker : MonoBehaviour
{
    void Awake()
    {
        // Luôn mở khóa nhân vật mặc định (ID = 0)
        if (!CharacterUnlockManager.IsCharacterUnlocked(0))
        {
            CharacterUnlockManager.UnlockCharacter(0);
            Debug.Log("Đã mở khóa nhân vật mặc định (ID 0)");
        }
    }
}
