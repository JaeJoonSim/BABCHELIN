using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Start Button")]
    public Button StartBtn;
    public string SceneName;

    [Space]
    [Header("Quit Button")]
    public Button QuitBtn;

    private void Start()
    {
        if (StartBtn != null)
            StartBtn.onClick.AddListener(() => LoadScene(SceneName));

        if (QuitBtn != null)
            QuitBtn.onClick.AddListener(() => QuitGame());
    }

    public void LoadScene(string sceneName)
    {
        PlayerPrefs.SetString("SceneToLoad", sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
