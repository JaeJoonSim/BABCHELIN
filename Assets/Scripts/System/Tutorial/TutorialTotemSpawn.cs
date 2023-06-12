using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTotemSpawn : BaseMonoBehaviour
{
    public DialogueSystemDialogue dialogue;
    public GameObject TotemTutorial;
    public TotemObj totem;

    [Space]
    public SkeletonAnimation anim;
    public AnimationReferenceAsset apperance;

    bool isSpawn;

    private void Start()
    {
        TotemTutorial.SetActive(false);
        totem.gameObject.SetActive(false);

        totem.OnGetTotem += OnGetTotem;
    }

    private void OnGetTotem()
    {
        anim.AnimationState.SetAnimation(0, apperance, false);
        anim.gameObject.SetActive(true);
    }

    void Update()
    {
        if (dialogue == null)
            dialogue = FindObjectOfType<DialogueSystemDialogue>();

        if (DungeonUIManager.Instance.enemyCount <= 1 && !isSpawn)
        {
            //totem.SetActive(true);
            StartCoroutine(AfterMonsterDead());

            isSpawn = true;
        }
    }

    private IEnumerator AfterMonsterDead()
    {
        yield return new WaitForSeconds(1f);
        totem.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        dialogue.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        TotemTutorial.SetActive(true);
    }
}
