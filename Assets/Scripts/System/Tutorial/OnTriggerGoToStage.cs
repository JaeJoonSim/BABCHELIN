using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTriggerGoToStage : BaseMonoBehaviour
{
    public Transform target;
    private CameraFollowTarget Cam;

    public GameObject DialoguePanel;
    public float delayInSeconds;
    public GameObject[] tutorialPanel;
    public Button TutorialQuitButton;

    public void Start()
    {
        Cam = FindObjectOfType<CameraFollowTarget>();
        if (TutorialQuitButton != null)
        {
            TutorialQuitButton.onClick.AddListener(() => QuitPanel(tutorialPanel));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && tutorialPanel != null)
        {
            QuitPanel(tutorialPanel);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player Enter");
            collision.transform.position = target.position;
            Cam.SnappyMovement = true;
            StartCoroutine(CamSnappyOff());
            if (DialoguePanel != null && tutorialPanel != null)
            {
                StartCoroutine(OpenPanel());
            }
        }
    }

    public IEnumerator CamSnappyOff()
    {
        yield return new WaitForSeconds(1.5f);
        Cam.SnappyMovement = false;
    }

    public IEnumerator OpenPanel()
    {
        yield return new WaitForSeconds(delayInSeconds);
        DialoguePanel.SetActive(true);
        yield return new WaitUntil(() => !DialoguePanel.activeInHierarchy);
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
        }
        tutorialPanel[0].SetActive(true);
    }

    private void QuitPanel(GameObject[] panel)
    {
        for (int i = 0; i < panel.Length; i++)
        {
            if (panel[i].activeInHierarchy)
            {
                panel[i].SetActive(false);
                Time.timeScale = 1f;
            }
        }

    }
}
