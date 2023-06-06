using UnityEngine;
using UnityEngine.Events;
using Spine;
using Spine.Unity;
using System.Collections;

public class TreasureChest : UnitObject, Interactable
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

    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    public UnityEvent offInteraction;
    #endregion

    [SerializeField] float dropRange;
    [SerializeField] GameObject[] itemsToDrop;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        state.CURRENT_STATE = StateMachine.State.Idle;
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

    }

    public override void Update()
    {
        base.Update();

        speed = Mathf.Max(speed, 0f);
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            SpineTransform.localPosition = Vector3.zero;

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    break;

                case StateMachine.State.Patrol:
                    break;

                case StateMachine.State.Attacking:
                    break;

            }
        }
    }


    public bool OnInteract(Interactor interactor)
    {
        if (!UIManagerScript.OnUI)
        {
            onInteraction.Invoke();
            Debug.Log($"{gameObject.name} : OnInteracted with Chest");
        }
        return true;
    }

    public void OffInteract()
    {
    }

    public void DropItems()
    {
        int numDrops = Random.Range(1, 3);
        for (int i = 0; i < numDrops; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * dropRange;
            Vector3 dropPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y, transform.position.z);

            GameObject item = itemsToDrop[Random.Range(0, itemsToDrop.Length)];
            Instantiate(item, dropPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
