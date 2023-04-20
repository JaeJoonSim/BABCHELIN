using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using System.Collections;

public class CookieMouse2 : UnitObject
{
    public Transform SpineTransform;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float attackDetectionRange;
    [SerializeField] float AttackDistance = 0f;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;
    [SerializeField] float moveSpeed;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private float distanceToPlayer;
    Vector3 movePoint;

    [Space]

    public float forceDir;
    public float xDir;
    public float yDir;

    private SkeletonAnimation spineAnimation;

    [SerializeField] GameObject slowObj;

    private void Start()
    {
        movePoint = target.position;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    public override void Update()
    {
        base.Update();

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }
        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            Vector3 directionToTarget;
            float distanceToPoint;
            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    distanceToPlayer = Vector3.Distance(transform.position, target.position);
                    if (distanceToPlayer <= detectionRange)
                    {
                        Debug.Log("pc 감지");
                        state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                        state.LookAngle = state.facingAngle;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }
                    agent.isStopped = true;

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;
                case StateMachine.State.Moving:
                    agent.speed = moveSpeed;

                    directionToTarget = (target.position - transform.position).normalized;

                    xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                    yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);

                    agent.SetDestination(target.position);
                    agent.isStopped = false;

                    distanceToPoint = Vector3.Distance(transform.position, target.position);
                    if (distanceToPoint <= attackDetectionRange)
                    {
                        Debug.Log("pc 공격");
                        movePoint = target.position;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;
                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    detectionRange *= 2f;
                    break;
                case StateMachine.State.Attacking:
                    agent.speed = moveSpeed;

                    directionToTarget = (movePoint - transform.position).normalized;

                    xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                    yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);

                    agent.SetDestination(movePoint);
                    agent.isStopped = false;

                    distanceToPoint = Vector3.Distance(transform.position, movePoint);
                    if (distanceToPoint <= AttackDistance)
                    {
                        float stopTime = 0;
                        agent.isStopped = true;
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }

                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;
            }
        }
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }

}
