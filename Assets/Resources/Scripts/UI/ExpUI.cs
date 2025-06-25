using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpUI : MonoBehaviour
{
    public static ExpUI instance;
    public Slider sliderBar;
    private PlayerExp player;

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
    public void updateUI(int currentExp, int expToNextLevel, int level)
    {
      
            sliderBar.maxValue = expToNextLevel;
            sliderBar.value = currentExp;
       
    }
}
