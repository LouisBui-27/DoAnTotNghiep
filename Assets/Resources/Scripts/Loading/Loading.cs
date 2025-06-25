using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public static Loading instance;
    public float loadingTimer = 3f;
    protected float currentTimer;
    public Image loadingbar;
    public Text percentageText;
    public bool load = true;
    public bool loadComplete = false;
    private AsyncOperation asyncLoad;
    public static string SceneToLoadName = "MainMenu";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Thường không cần DontDestroyOnLoad cho Loading Scene
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        currentTimer = 0f;
        //randomLoading();
        StartCoroutine(LoadSceneAsync());
    }
    private void Update()
    {
        if (load)
        {
            currentTimer += Time.deltaTime;
            float progress = Mathf.Min(1f, currentTimer / loadingTimer); // Tính toán tiến độ
            loadingbar.fillAmount = progress;
            if (percentageText != null)
            {
                percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";
            }
            if (currentTimer >= loadingTimer)
            {
          
                loadComplete = true;
                //asyncLoad.allowSceneActivation = true;
            }

        }
    }
    private IEnumerator LoadSceneAsync()
    {
        if (percentageText != null) percentageText.text = "0%";
        if (loadingbar != null) loadingbar.fillAmount = 0f;
        asyncLoad = SceneManager.LoadSceneAsync(SceneToLoadName);
        asyncLoad.allowSceneActivation = false; // Ngăn không cho scene tự động kích hoạt

        while (!asyncLoad.isDone)
        {
            //// Cập nhật thanh loading dựa trên tiến độ tải thực tế (0.0 đến 0.9)
            //float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            //// Kết hợp tiến độ thực tế với thời gian giả lập
            //// Sử dụng giá trị lớn hơn giữa tiến độ thực và tiến độ giả lập để thanh loading không bị giật lùi
            //float combinedProgress = Mathf.Max(realProgress, currentTimer / loadingTimer);
            //loadingbar.fillAmount = combinedProgress;
            //if (percentageText != null)
            //{
            //    percentageText.text = Mathf.RoundToInt(combinedProgress * 100f) + "%";
            //}

            // Đợi cho đến khi scene tải gần xong (progress >= 0.9f) VÀ thời gian giả lập đã đủ
            if (asyncLoad.progress >= 0.9f && loadComplete)
            {
                asyncLoad.allowSceneActivation = true; // Cho phép kích hoạt scene
            }

            yield return null;
        }

        Debug.Log($"Loading: Scene '{SceneToLoadName}' successfully loaded.");
    }
    public static void LoadGameScene(string sceneName)
    {
        SceneToLoadName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
