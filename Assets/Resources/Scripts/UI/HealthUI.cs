using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthUI : MonoBehaviour
{
    public Slider healthBar;
    public static HealthUI instance;
    public Text healthTxt;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealthUI(float currentHP, float maxHP)
    {
        if (healthBar != null)
            healthBar.value = currentHP / maxHP;
        if (healthTxt != null)
        {
            healthTxt.text = $"{currentHP}/{maxHP}";
        }
                    
    }
}
