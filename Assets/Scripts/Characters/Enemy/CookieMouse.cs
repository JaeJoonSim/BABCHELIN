using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class CookieMouse : UnitObject
{
    public Transform SpineTransform;

    public float Damaged = 2f;
    bool hasAppliedDamage = false;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float AttackDistance = 1f;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;
    [SerializeField] float delayTime = 5f;
    [SerializeField] float time = 0;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private float distanceToPlayer;

    [Space]
    [SerializeField] float patrolRange = 10f;
    [SerializeField] float patrolMoveDuration = 2f;
    [SerializeField] float idleToPatrolDelay = 5f;
    private float idleTimer;
    private float patrolTimer;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    public GameObject BombObject;

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
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }
        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    idleTimer += Time.deltaTime;
                    time = 0;

                    if (!isPlayerInRange && idleTimer >= idleToPatrolDelay)
                    {
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (isPlayerInRange)
                        state.CURRENT_STATE = StateMachine.State.Moving;

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

                    if (distanceToPlayer <= AttackDistance)
                    {
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

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
                    agent.isStopped = true;

                    if(AttackTimer >= 0.7)
                    {
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
                    AttackTimer = 0;
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }

                    agent.isStopped = true;
                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

            }
        }
    }

    private void FollowTarget()
    {
        if (state.CURRENT_STATE != StateMachine.State.Attacking && state.CURRENT_STATE != StateMachine.State.Delay)
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
        Debug.Log("ÆÐÆ®·Ï µµÂø");
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRange;
        randomDirection += patrolStartPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRange, 1);
        return hit.position;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if (AttackDistance > distanceToPlayer)
                playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
        }
    }

    public void OnDie()
    {
        //GameObject bomb = BombObject;
        //bomb.transform.position = transform.position;
        //Instantiate(bomb);
        agent.speed = 0f;
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }
}
