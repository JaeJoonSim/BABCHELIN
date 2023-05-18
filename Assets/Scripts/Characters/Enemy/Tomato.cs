using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class Tomato : UnitObject
{
    public Transform SpineTransform;
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
    public AnimationReferenceAsset Hidden;
    public AnimationReferenceAsset Runaway;
    private int aniCount = 0;

    public float Damaged = 1f;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    private float moveTime = 0f;
    //[SerializeField] float moveTime = 0f;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;
    [SerializeField] float AttackCount = 0f;
    [SerializeField] float delayTime = 1f;
    [SerializeField] float time = 0;
    private int runawayCount = 1;
    private bool isShot = false;

    private Health thisHealth;
    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private bool isPlayerInAttackRange;
    private float distanceToPlayer;
    Vector3 movePoint;
    Vector3 directionToPoint;

    [Space]
    [SerializeField] float patrolRange = 10f;
    private float patrolMoveDuration = 0f;
    [SerializeField] float patrolMinTime;
    [SerializeField] float patrolMaxTime;
    private float idleToPatrolDelay = 0f;
    [SerializeField] float idleMinTime;
    [SerializeField] float idleMaxTime;
    private float idleTimer;
    private float patrolTimer;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    public GameObject BulletObject;
    public GameObject ExplosionEffect;

    private SkeletonAnimation spineAnimation;

    private void Start()
    {
        idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
        patrolStartPosition = transform.position;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        thisHealth = transform.GetComponent<Health>();
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

            if(health.CurrentHP() <= 3 && runawayCount >= 1)
            {
                Debug.Log(health.CurrentHP());
                detectionRange *= 2;
                runawayCount--;
                state.CURRENT_STATE = StateMachine.State.Runaway;
            }
        }

        if (state.CURRENT_STATE == StateMachine.State.Runaway)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }
        if (state.CURRENT_STATE != StateMachine.State.Attacking)
        {
            state.LockStateChanges = false;
        }
        if (health.CurrentHP() <= 0)
        {
            state.LockStateChanges = false;
        }

        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Hidden:
                    //agent.isStopped = true;
                    if (Hidden != null)
                    {
                        while (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Hidden, loop: true);
                            aniCount++;
                        }
                    }

                    if (isPlayerInRange)
                    {
                        //moveTime = UnityEngine.Random.Range(2, 4);
                        aniCount = 0;
                        movePoint = target.position;
                        directionToPoint = (transform.position - movePoint).normalized;
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }
                    break;

                case StateMachine.State.Idle:
                    agent.speed = 1f;
                    idleTimer += Time.deltaTime;
                    time = 0;

                    if (!isPlayerInRange && idleTimer >= idleToPatrolDelay)
                    {
                        patrolMoveDuration = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (distanceToPlayer <= detectionAttackRange)
                    {
                        aniCount = 0;
                        moveTime = 0;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Patrol:
                    Patrol();
                    break;

                case StateMachine.State.Runaway:
                    if (Runaway != null)
                    {
                        while (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Runaway, loop: true);
                            aniCount++;
                        }
                    }
                    RunAway();

                    if (0 <= xDir)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                    break;

                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    agent.speed = 0f;
                    agent.isStopped = true;
                    state.LockStateChanges = true;
                    Vector3 directionToTarget = (target.position - transform.position).normalized;
                    xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                    if (0 <= xDir)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }

                    if (isShot && AttackCount < 2)
                    {
                        AttackTimer += Time.deltaTime;

                        if (AttackTimer >= 0.2f)
                        {
                            GameObject bullet = BulletObject;
                            Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.Euler(0, 0, state.facingAngle));
                            AttackTimer = 0;
                            AttackCount++;
                        }
                    }

                    if (AttackCount >= 2)
                    {
                        state.LockStateChanges = false;
                        isShot = false;
                        AttackCount = 0;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Delay:
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        time = 0f;
                        if(distanceToPlayer <= detectionAttackRange)
                        {
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Idle;
                        }
                    }

                    agent.isStopped = true;
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
                agent.isStopped = false;
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
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (isPlayerInRange)
        {
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            movePoint = target.position;
            directionToPoint = (transform.position - movePoint).normalized;
            state.CURRENT_STATE = StateMachine.State.Runaway;
        }

        xDir = Mathf.Clamp((patrolTargetPosition.x - transform.position.x), -1f, 1f);
        if (0 <= xDir)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
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

    private void RunAway()
    {
        moveTime += Time.deltaTime;
        agent.speed = 3f;
        xDir = Mathf.Clamp((transform.position.x + directionToPoint.x), -1f, 1f);
        if (moveTime <= 2)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            agent.SetDestination(transform.position + directionToPoint);

            if(detectionRange < distanceToPlayer)
            {
                aniCount = 0;
                moveTime = 0;
                state.CURRENT_STATE = StateMachine.State.Attacking;
            }
        }
        else
        {
            aniCount = 0;
            moveTime = 0;
            state.CURRENT_STATE = StateMachine.State.Idle;
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "ATTACK")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                isShot = true;
                AttackCount = 0;
                GameObject bullet = BulletObject;
                Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.Euler(0, 0, state.facingAngle));
            }
        }
        else if (e.Data.Name == "bang" || e.Data.Name == "BANG")
        {
            DeathEffect();
        }
    }

    public void OnDie()
    {
        agent.speed = 0f;
        Destroy(gameObject, 3f);
    }

    private void DeathEffect()
    {
        GameObject explosion = ExplosionEffect;
        Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, state.facingAngle));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionAttackRange);
    }

}
