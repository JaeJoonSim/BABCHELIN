using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class CookieMouse3 : UnitObject
{
    public Transform SpineTransform;

    public float Damaged = 1f;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float attackDistance = 0f;
    //[SerializeField] float moveTime = 0f;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;
    [SerializeField] float delayTime = 1f;
    [SerializeField] float time = 0;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private bool isPlayerInAttackRange;
    private float distanceToPlayer;
    Vector3 movePoint;
    Vector3 directionToPoint;

    [Space]
    [SerializeField] float patrolRange = 10f;
    [SerializeField] float patrolMoveDuration = 2f;
    [SerializeField] float runawaySpeedMultiplier = 1.5f;
    [SerializeField] float idleToPatrolDelay = 5f;
    private float idleTimer;
    private float patrolTimer;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    public GameObject BulletObject;

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
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            FollowTarget();
        }

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }
        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    idleTimer += Time.deltaTime;
                    if (!isPlayerInRange && idleTimer >= idleToPatrolDelay)
                    {
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (isPlayerInRange)
                    {
                        state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                        state.LookAngle = state.facingAngle;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Moving:
                    agent.speed = 3f;
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

                    if (isPlayerInAttackRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;

                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    detectionRange *= 2f;
                    break;
                case StateMachine.State.Attacking:
                    agent.speed = 0f;
                    agent.isStopped = true;
                    AttackTimer += Time.deltaTime;

                    if(AttackTimer >= 0.7667f)
                    {
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Patrol:
                    Patrol();
                    break;

                case StateMachine.State.Delay:
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        time = 0f;
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }
                    break;
            }
        }
    }

    private void FollowTarget()
    {
        if (state.CURRENT_STATE != StateMachine.State.Attacking && state.CURRENT_STATE != StateMachine.State.Delay)
        {
            if (distanceToPlayer <= detectionRange)
            {
                isPlayerInRange = true;
            }
            else
            {
                isPlayerInRange = false;
            }

            if (distanceToPlayer <= detectionAttackRange)
            {
                isPlayerInAttackRange = true;
            }
            else
            {
                isPlayerInAttackRange = false;
            }

            if (isPlayerInRange && state.CURRENT_STATE != StateMachine.State.Attacking)
            {
                agent.speed = 3f;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);

                agent.SetDestination(target.position);
                if (distanceToPlayer <= attackDistance)
                {
                    state.CURRENT_STATE = StateMachine.State.Attacking;
                    playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
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

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (Vector3.Distance(transform.position, patrolTargetPosition) < 0.5f)
        {
            patrolTargetPosition = GetRandomPositionInPatrolRange();
        }

        if (patrolTimer < patrolMoveDuration)
        {
            agent.SetDestination(patrolTargetPosition);
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
            patrolTimer = 0f;
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (isPlayerInRange)
        {
            state.CURRENT_STATE = StateMachine.State.Moving;
        }
    }

    private Vector3 GetRandomPositionInPatrolRange()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRange;
        randomDirection += patrolStartPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRange, 1);
        return hit.position;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack_grape")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                Debug.Log("투사체 발사");
                GameObject bullet = BulletObject;
                Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, state.facingAngle));
            }
        }
    }

    public void OnDie()
    {
        agent.speed = 0f;
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

}
