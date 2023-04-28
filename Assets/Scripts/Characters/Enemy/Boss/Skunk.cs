using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skunk : UnitObject
{
    [SerializeField] private PatternManager patternManager;

    [SerializeField] private List<BossPattern> basicPatterns;
    [SerializeField] private List<BossPattern> gimmickPatterns;

    [Space]
    //public Transform SpineTransform;

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

    [Space]

    public float forceDir;
    public float xDir;
    public float yDir;

    //private SkeletonAnimation spineAnimation;

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

        patternManager = new PatternManager();
        RegisterPatterns();
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
            patternManager.DequeuePattern();
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            switch (state.CURRENT_STATE)
            {

                case StateMachine.State.Idle:
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

    private void RegisterPatterns()
    {
        foreach (var pattern in basicPatterns)
        {
            patternManager.EnqueuePattern(pattern);
        }

        foreach (var pattern in gimmickPatterns)
        {
            pattern.onPatternStart += () =>
            {
                patternManager.ClearPatterns(BossPattern.PatternType.Basic);
            };

            patternManager.EnqueuePattern(pattern);
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
