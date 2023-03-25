using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultCommands : CommandBehaviour
{
    protected override void Start()
    {
        base.Start();
    }

    [Command]
    public void print_hello_world()
    {
        Debug.Log("Hello World!");
    }

    [Command]
    public void reload_current_scene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Command]
    public void load_scene(int index)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
    }

    [Command]
    public void help()
    {
        Debug.Log("< Command List >");
        for (int i = 0; i < Command.List.Count; i++)
        {
            Debug.Log(Command.List[i].MethodInfo.Name);
        }
    }
}