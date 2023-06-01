using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class AreaSpawnMonster : BaseMonoBehaviour
{
    private StateMachine playerState;

    public GameObject tutorialPanel;
    public Button TutorialQuitButton;
    public GameObject[] monsterPrefabs;
    public BoxCollider2D spawnArea;
    public int numberOfMonsters;
    private int activeMonsters;

    [Space]
    public GameObject thirdRoomEnter;

    private void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<StateMachine>();
        TutorialQuitButton.onClick.AddListener(() => QuitPanel(tutorialPanel));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitPanel(tutorialPanel);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialPanel.SetActive(true);
            playerState.CURRENT_STATE = StateMachine.State.Pause;
            for (int i = 0; i < numberOfMonsters; i++)
            {
                SpawnMonster();
            }
            Time.timeScale = 0;
            spawnArea.enabled = false;
        }
    }

    private void QuitPanel(GameObject panel)
    {
        panel.SetActive(false);
        playerState.CURRENT_STATE = StateMachine.State.Idle;
        Time.timeScale = 1f;
    }

    private void SpawnMonster()
    {
        GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
        Vector2 spawnPosition = new Vector2(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                                            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y));

        GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        activeMonsters++;

        Health monsterHealth = spawnedMonster.GetComponent<Health>();
        if (monsterHealth != null)
        {
            monsterHealth.OnDie += HandleMonsterDeath;
        }
    }

    private void HandleMonsterDeath()
    {
        activeMonsters--;
        if (activeMonsters <= 0)
        {
            thirdRoomEnter.SetActive(true);
        }
    }
}
