using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnterAndActivePanel : BaseMonoBehaviour
{
    public GameObject DialoguePanel;
    public float delayInSeconds;

    public IEnumerator OpenPanel()
    {
        yield return new WaitForSeconds(delayInSeconds);
        DialoguePanel.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(OpenPanel());
        }
    }
}
