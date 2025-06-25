using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimeUI : MonoBehaviour
{
    public Text playTimeText;

    private float elapsedTime = 0f;

    void Update()
    {
        // Không tăng thời gian nếu game bị pause
        if (Time.timeScale == 0) return;

        elapsedTime += Time.unscaledDeltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        playTimeText.text = $"{minutes:00}:{seconds:00}s";
    }

    public float GetPlayTime()
    {
        return elapsedTime;
    }
}
