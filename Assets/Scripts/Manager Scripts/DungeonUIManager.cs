using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIManager : BaseMonoBehaviour
{
    private static DungeonUIManager _instance;
    public static DungeonUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DungeonUIManager>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<DungeonUIManager>();
                    singleton.name = typeof(DungeonUIManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    private int tabNum;

    public GameObject escPanel;
    public GameObject settingPanel;
    public GameObject PlayerUI;
    public GameObject BossUI;
    public GameObject Minimap;

    public GameObject[] Tabs;
    public GameObject[] tabBtn;

    public int enemyCount;

    [Space]
    public string SceneName;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private void Start()
    {
        escPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escPanel.activeSelf)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;

            escPanel.SetActive(!escPanel.activeSelf);
            settingPanel.SetActive(false);
            PlayerUI.SetActive(!PlayerUI.activeSelf);
            if (BossUI != null)
                BossUI.SetActive(!BossUI.activeSelf);
            if(Minimap != null)
                Minimap.SetActive(!Minimap.activeSelf);
        }

        for (int a = 0; a < 5; a++)
        {
            if (tabNum == a)
            {
                Tabs[a].SetActive(true);
                tabBtn[a].SetActive(false);
            }
            else
            {
                Tabs[a].SetActive(false);
                tabBtn[a].SetActive(true);
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
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

    public void Resume()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        escPanel.SetActive(!escPanel.activeSelf);
        settingPanel.SetActive(false);
        PlayerUI.SetActive(!PlayerUI.activeSelf);
        if (BossUI != null)
            BossUI.SetActive(!BossUI.activeSelf);
        Minimap.SetActive(!Minimap.activeSelf);
    }

    public void SettingUI()
    {
        //escPanel.SetActive(!escPanel.activeSelf);
        settingPanel.SetActive(!settingPanel.activeSelf);
    }

    public void SetTabNum(int a)
    {
        tabNum = a;
    }

    public void RegisterEnemy()
    {
        enemyCount++;
    }
}
