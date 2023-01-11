using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�̵��� �� �̸�")]
    private string sceneName;

    // �ݶ��̴� ����
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
