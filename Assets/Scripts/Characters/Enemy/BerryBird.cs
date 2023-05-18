using Spine;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.AI;

public class BerryBird : UnitObject
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
    private float aniCount = 0;

    public float Damaged = 1f;

    [Space]

    [SerializeField] Transform target;
    private Transform flowerPot;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float moveTime = 0f;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;
    [SerializeField] float delayTime = 1f;
    [SerializeField] float time = 0;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
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
    public GameObject BombEffect;
    public GameObject ExplosionEffect;

    private SkeletonAnimation spineAnimation;

    private void Start()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        flowerPot = transform.parent.transform;
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        health.OnHit += OnHit;
        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;

        patrolStartPosition = flowerPot.position;       //flowerPot 주변으로 패트롤 범위 설정
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

        if (state.CURRENT_STATE != StateMachine.State.Attacking)
        {
            state.LockStateChanges = false;
        }

        if(health.CurrentHP() <= 0)
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
                case StateMachine.State.Idle:
                    agent.speed = 1f;
                    idleTimer += Time.deltaTime;
                    time = 0;
                    if (!isPlayerInRange && idleTimer >= idleToPatrolDelay)
                    {
                        patrolTargetPosition = GetRandomPositionInPatrolRange();
                        patrolMoveDuration = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (isPlayerInRange)
                    {
                        directionToPoint = (transform.position - movePoint).normalized;
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Patrol:
                    Patrol();
                    break;

                case StateMachine.State.Runaway:
                    RunAway();
                    break;

                case StateMachine.State.Moving:
                    agent.speed = 3f;
                    AttackTimer = 0f;
                    if (agent.enabled)
                        agent.isStopped = false;
                    if (Time.timeScale == 0f)
                        break;

                    if (0 <= xDir)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }

                    if (agent.enabled)
                        agent.SetDestination(target.position);

                    if (distanceToPlayer <= detectionAttackRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

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
                    state.LockStateChanges = true;
                    forceDir = state.facingAngle;
                    AttackTimer += Time.deltaTime;
                    agent.isStopped = true;

                    if (AttackTimer >= 0.7)
                    {
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    //forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    //state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    //state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;

                case StateMachine.State.Delay:
                    AttackTimer = 0;
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }

                    agent.isStopped = true;
                    break;
            }
        }
    }

    private void FollowTarget()
    {
        if (state.CURRENT_STATE != StateMachine.State.Attacking || !IsAttackAnimationPlaying())
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
            aniCount = 0;
            patrolTimer = 0f;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (isPlayerInRange)
        {
            aniCount = 0;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
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
        agent.speed = 2f;
        xDir = Mathf.Clamp((patrolTargetPosition.x - transform.position.x), -1f, 1f);
        if (0 <= xDir)  //보는 방향
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        agent.SetDestination(transform.position + directionToPoint);
    }

    private bool IsAttackAnimationPlaying()
    {
        var currentAnimation = spineAnimation.AnimationState.GetCurrent(0);
        return currentAnimation.Animation.Name.ToLower().Contains("attack") && !currentAnimation.IsComplete;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "ATTACK")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
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
