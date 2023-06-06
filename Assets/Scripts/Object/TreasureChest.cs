using UnityEngine;
using UnityEngine.Events;
using Spine;
using Spine.Unity;
using System.Collections;

public class TreasureChest : BaseMonoBehaviour, Interactable
{
    public Transform SpineTransform;
    private SkeletonAnimation spineAnimation;
    public int AnimationTrack = 0;
    private SkeletonAnimation _anim;
    private SkeletonAnimation anim
    {
        get
        {
            if (_anim == null)
            {
                _anim = this.transform.GetChild(0).GetComponent<SkeletonAnimation>();
            }
            return _anim;
        }
    }
    public AnimationReferenceAsset Idle;
    public AnimationReferenceAsset Open;
    private float aniCount = 0;

    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    [SerializeField] float spawnRange;
    [SerializeField] GameObject[] buffObject;

    public GameObject spawnEffect;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    public UnityEvent offInteraction;
    #endregion

    private void Start()
    {
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();
        spineAnimation.AnimationState.Event += OnSpineEvent;

        if (Idle != null)
        {
            anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
        }

    }


    public bool OnInteract(Interactor interactor)
    {
        onInteraction.Invoke();
        Debug.Log($"{gameObject.name} : Open the Treasure Chest");
        return true;
    }

    public void OffInteract()
    {
    }

    public void SpawnObject()
    {
        if (Open != null)
        {
            while (aniCount < 1)
            {
                anim.AnimationState.SetAnimation(AnimationTrack, Open, loop: false);
                aniCount++;
            }
        }

        Destroy(gameObject, 5f);
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "open")
        {
            Vector2 randomPoint = Random.insideUnitCircle * spawnRange;
            Vector3 dropPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y, transform.position.z);

            spawnEffect.transform.position = new Vector3(dropPosition.x, dropPosition.y, dropPosition.z - 0.5f);
            Instantiate(spawnEffect);
            GameObject spawnMonster = Instantiate(buffObject[Random.Range(0, buffObject.Length)], dropPosition, Quaternion.identity);
            spawnMonster.transform.SetParent(transform.parent);
        }
    }
}
