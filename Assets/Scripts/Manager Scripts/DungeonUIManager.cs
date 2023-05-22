using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonUIManager : BaseMonoBehaviour
{
    public GameObject escPanel;
    public GameObject settingPanel;

    private void Start()
    {
        escPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale != 1) Time.timeScale = 1;
            else Time.timeScale = 0;
            escPanel.SetActive(!escPanel.activeSelf);
            settingPanel.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Resume()
    {
        escPanel.SetActive(false);
    }

    public void SettingUI()
    {
        escPanel.SetActive(!escPanel.activeSelf);
        settingPanel.SetActive(!settingPanel.activeSelf);
    }
}
