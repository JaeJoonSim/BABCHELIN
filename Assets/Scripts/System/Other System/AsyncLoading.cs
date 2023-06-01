using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoading : BaseMonoBehaviour
{
    [SerializeField] private float delayTime = 3f;
    private string sceneToLoad;

    private void Start()
    {
        sceneToLoad = PlayerPrefs.GetString("SceneToLoad");

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(delayTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
