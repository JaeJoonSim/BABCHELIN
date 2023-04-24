using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerGoToStage : BaseMonoBehaviour
{
    public Transform target;
    private CameraFollowTarget Cam;

    public GameObject DialoguePanel;
    public float delayInSeconds;
    public GameObject[] tutorialPanel;

    public void Start()
    {
        Cam = FindObjectOfType<CameraFollowTarget>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player Enter");
            collision.transform.position = target.position;
            Cam.SnappyMovement = true;
            StartCoroutine(CamSnappyOff());
            StartCoroutine(OpenPanel());
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
    }
}
