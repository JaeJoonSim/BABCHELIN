using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skunk : UnitObject
{
    [SerializeField] private PatternManager patternManager;

    [SerializeField] private List<BasicPatternScriptableObject> basicPatterns;
    [SerializeField] private List<GimmickScriptableObject> gimmickPatterns;

    [Space]
    public Transform SpineTransform;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float AttackDistance = 2f;

    [Space]
    [SerializeField] float AttackDelay = 2f;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private float distanceToPlayer;
    [HideInInspector] public bool wasFarting = false;

    [Space]

    [HideInInspector] public float forceDir;
    [HideInInspector] public float xDir;
    [HideInInspector] public float yDir;

    public GameObject fartPrefab;

    private SkeletonAnimation spineAnimation;

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        //spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (patternManager == null)
        {
            patternManager = FindObjectOfType<PatternManager>();
        }

        patternManager.basicPatterns = basicPatterns;
        patternManager.gimmickPatterns = gimmickPatterns;
        patternManager.Initialize();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        health.OnDie += OnDie;
        //spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        health.OnDie -= OnDie;
        //spineAnimation.AnimationState.Event -= OnSpineEvent;
    }

    public override void Update()
    {
        base.Update();

        if (patternManager.CurrentPattern == null)
        {
            patternManager.DequeuePattern(health);
        }

        if (state.CURRENT_STATE != StateMachine.State.Farting)
        {
            wasFarting = false;
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            switch (state.CURRENT_STATE)
            {

                case StateMachine.State.Idle:
                    if (patternManager.CurrentPattern != null)
                    {
                        patternManager.CurrentPattern.ExecutePattern(this);
                    }
                    break;
                case StateMachine.State.Moving:
                    break;
                case StateMachine.State.Attacking:
                    break;
                case StateMachine.State.Runaway:
                    break;
                case StateMachine.State.Patrol:
                    break;
                case StateMachine.State.Jump:
                    break;
                case StateMachine.State.Farting:
                    if (!wasFarting)
                    {
                        Fart();
                        wasFarting = true;
                    }
                    break;
                case StateMachine.State.FartShield:
                    break;
                case StateMachine.State.Tailing:
                    break;
                case StateMachine.State.Throwing:
                    break;
                case StateMachine.State.Dead:
                    break;

                default:
                    break;
            }
        }

        foreach (var gimmickPattern in patternManager.gimmickPatterns)
        {
            if (health.multipleHealthLine <= gimmickPattern.triggerHealthLine)
            {
                gimmickPattern.CheckHealthThreshold();
            }
        }
    }

    #region Func
    private void FollowTarget()
    {
        if (state.CURRENT_STATE != StateMachine.State.Attacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= detectionRange)
            {
                isPlayerInRange = true;
            }
            else
            {
                isPlayerInRange = false;
            }

            if (isPlayerInRange && state.CURRENT_STATE != StateMachine.State.Attacking)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);
                agent.SetDestination(target.position);
                if (distanceToPlayer <= AttackDistance)
                {
                    state.CURRENT_STATE = StateMachine.State.Attacking;
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                agent.isStopped = true;
                xDir = 0f;
                yDir = 0f;
            }
        }
    }

    private void Fart()
    {
        if (state.CURRENT_STATE == StateMachine.State.Farting)
        {
            // Rotate and face the tail towards the player
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            //SpineTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Spawn the fart
            Instantiate(fartPrefab, transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Event
    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if (!hasAppliedDamage && state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                if (AttackDistance > distanceToPlayer)
                    playerHealth.Damaged(gameObject, transform.position, Damaged);
                hasAppliedDamage = true;
            }
        }
    }

    public void OnDie()
    {
        agent.speed = 0f;
        Destroy(gameObject, 5f);
    }
    #endregion

    #region Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }
    #endregion
}
