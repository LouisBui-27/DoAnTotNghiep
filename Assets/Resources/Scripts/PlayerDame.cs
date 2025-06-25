using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDame : MonoBehaviour
{
    public float baseDamage = 0f;
    public float GetCurrentDamage()
    {
        return baseDamage;
    }

    // Hàm để tăng dame cơ bản (ví dụ, khi nâng cấp)
    public void IncreaseBaseDamage(float amount)
    {
        baseDamage += amount;
    }

    // (Tùy chọn) Hàm để thiết lập dame cơ bản trực tiếp
    public void SetBaseDamage(float amount)
    {
        baseDamage = amount;
    }
}
