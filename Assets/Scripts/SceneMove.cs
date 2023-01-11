using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("이동할 씬 이름")]
    private string sceneName;

    // 콜라이더 접촉
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneChange(sceneName);
        }
    }

    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(this.sceneName);
    }

}
