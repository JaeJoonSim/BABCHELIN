using Spine;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.AI;

public class BerryBird : UnitObject
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
    private bool canAttack = false;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private float distanceToPlayer;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    [Space]
    [SerializeField] float patrolRange = 10f;
    [SerializeField] float patrolMoveDuration = 2f;
    [SerializeField] float patrolIdleDuration = 1f;
    [SerializeField] float runawaySpeedMultiplier = 1.5f;
    [SerializeField] float idleToPatrolDelay = 5f;
    private float idleTimer;
    private float patrolTimer;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;

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

        health.OnHit += OnHit;
        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;

        patrolStartPosition = transform.position;
        patrolTargetPosition = GetRandomPositionInPatrolRange();
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
                    idleTimer += Time.deltaTime;
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
                    else if (AttackTimer >= AttackDuration + AttackDelay && canAttack == true)
                    {
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;
                case StateMachine.State.Patrol:
                    Patrol();
                    break;
                case StateMachine.State.Runaway:
                    RunAway();
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
                if (distanceToPlayer <= AttackDistance && canAttack == true)
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
        else if (patrolTimer < patrolMoveDuration + patrolIdleDuration)
        {
            agent.isStopped = true;
        }
        else
        {
            patrolTimer = 0f;
        }

        if (isPlayerInRange)
        {
            state.CURRENT_STATE = StateMachine.State.Runaway;
        }
    }

    private void RunAway()
    {
        agent.speed *= runawaySpeedMultiplier;
        Vector3 directionToTarget = (transform.position - target.position).normalized;
        agent.SetDestination(transform.position + directionToTarget);

        if (!isPlayerInRange)
        {
            agent.speed /= runawaySpeedMultiplier;
            state.CURRENT_STATE = StateMachine.State.Patrol;
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

    public override void OnHit(GameObject Attacker, Vector3 AttackLocation)
    {
        if (canAttack == false) canAttack = true;
        Debug.Log($"공격가능: {canAttack}");
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolStartPosition, patrolRange);
    }


}
