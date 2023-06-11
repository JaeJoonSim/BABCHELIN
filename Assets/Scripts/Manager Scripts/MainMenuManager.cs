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

    [Space]
    [Header("Key Setting")]
    public KeyCode HideKey;
    public KeyCode OptionKey;
    public KeyCode CreditKey;
    public KeyCode QuitKey;

    public GameObject UICanvas;

    [Space]
    [Header("Option UI")]
    private int tabNum;
    public GameObject OptionUI;
    public GameObject[] TabBtn;
    public GameObject[] Tabs;

    public GameObject CreditUI;

    private void Start()
    {
        if (StartBtn != null)
            StartBtn.onClick.AddListener(() => LoadScene(SceneName));

        if (QuitBtn != null)
            QuitBtn.onClick.AddListener(() => QuitGame());

        OptionUI.SetActive(false);
        CreditUI.SetActive(false);
    }

    private void Update()
    {
        HideUI();
        OpenOptionUI();
        OpebCreditUI();
        CloseGame();
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

    private void HideUI()
    {
        if (Input.GetKeyDown(HideKey))
        {
            UICanvas.SetActive(!UICanvas.activeSelf);
        }
    }

    private void OpenOptionUI()
    {
        if (Input.GetKeyDown(OptionKey))
            OptionUI.SetActive(!OptionUI.activeSelf);

        for (int a = 0; a < 5; a++)
        {
            if (tabNum == a)
            {
                Tabs[a].SetActive(true);
                TabBtn[a].SetActive(false);
            }
            else
            {
                Tabs[a].SetActive(false);
                TabBtn[a].SetActive(true);
            }
        }
    }

    public void SetTabNum(int a)
    {
        tabNum = a;
    }

    public void CloseButton()
    {
        OptionUI.SetActive(!OptionUI.activeSelf);
    }

    private void OpebCreditUI()
    {
        if (Input.GetKeyDown(CreditKey))
            CreditUI.SetActive(!CreditUI.activeSelf);
    }

    private void CloseGame()
    {
        if (Input.GetKeyDown(QuitKey))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
