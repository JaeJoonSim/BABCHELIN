using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class OrangeDog : UnitObject
{
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
    private float hideTimer = 0;

    [Space]

    public float forceDir;
    public float xDir;
    public float yDir;

    private SkeletonAnimation spineAnimation;

    private void Start()
    {
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

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            FollowTarget();
        }

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }
        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    hideTimer += Time.deltaTime;
                    if(hideTimer >= 5f)
                    {
                        hideTimer = 0;
                        state.CURRENT_STATE = StateMachine.State.Hide;
                    }

                    if (isPlayerInRange)
                        state.CURRENT_STATE = StateMachine.State.Moving;

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;
                case StateMachine.State.Moving:
                    AttackTimer = 0f;
                    if (Time.timeScale == 0f)
                    {
                        break;
                    }
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));

                    state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    if (!isPlayerInRange)
                        state.ChangeToIdleState();
                    break;
                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    detectionRange *= 2f;
                    break;
                case StateMachine.State.Attacking:
                    SpineTransform.localPosition = Vector3.zero;
                    forceDir = state.facingAngle;
                    AttackTimer += Time.deltaTime;
                    distanceToPlayer = Vector3.Distance(transform.position, target.position);

                    if (AttackTimer >= AttackDuration / 2f && !hasAppliedDamage)
                    {
                    }
                    else if (AttackTimer < AttackDuration / 2f)
                    {
                        hasAppliedDamage = false;
                    }

                    if (distanceToPlayer > AttackDistance && AttackTimer >= AttackDuration)
                    {
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }
                    else if (AttackTimer < 0.1f)
                    {
                        state.facingAngle = (forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir)));
                    }
                    else if (AttackTimer >= AttackDuration && AttackTimer < AttackDuration + AttackDelay)
                    {
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }
                    else if (AttackTimer >= AttackDuration + AttackDelay)
                    {
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;
                case StateMachine.State.Hide:
                    hideTimer += Time.deltaTime;
                    if(hideTimer >= 2f)
                    {
                        hideTimer = 0;
                        state.CURRENT_STATE = StateMachine.State.Hidden;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;
                case StateMachine.State.Hidden:
                    if (isPlayerInRange)
                        state.CURRENT_STATE = StateMachine.State.Idle;

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;
            }
        }
    }

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
                //state.CURRENT_STATE = StateMachine.State.Moving;
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

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if (!hasAppliedDamage && state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                if (AttackDistance > distanceToPlayer)
                    playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
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