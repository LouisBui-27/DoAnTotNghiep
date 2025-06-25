using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;
    public Image iconImage;
    public Text txtLevel;

    private void Awake()
    {
        instance = this;
    }

    public void SetCharacterIcon(Sprite icon)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }
    }
    public void SetLevelTxt(int level)
    {
        if(txtLevel != null)
        {
            txtLevel.text = level.ToString();
        }
    }
}
