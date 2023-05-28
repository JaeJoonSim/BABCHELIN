using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using System.Collections;

public class SconeHedgehog2 : UnitObject
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
    public AnimationReferenceAsset Walk;
    public AnimationReferenceAsset StartMoving;
    public AnimationReferenceAsset StopMoving;
    public AnimationReferenceAsset Dash;
    public AnimationReferenceAsset StopDash;
    public AnimationReferenceAsset Jump;
    private float aniCount = 1;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float detectionJumpRange;
    [SerializeField] float jumpDamageRange;
    [SerializeField] float attackDistance = 0f;
    [SerializeField] float dashTime = 0f;
    [SerializeField] float delayTime = 5f;
    [SerializeField] float time = 0;
    [SerializeField] float AttackTimer = 0;
    private int attackPercentage;
    [SerializeField] int dashPer;
    private int dashCount = 0;
    private bool isJump = false;
    private bool isLand = false;

    [Space]
    private Health playerHealth;
    private Health thisHealth;
    private NavMeshAgent agent;
    private float distanceToPlayer;
    Transform moveTarget;
    Vector3 directionToTarget;
    Vector3 jumpPoint;
    Vector3 directionToPoint;

    [Space]
    [SerializeField] float patrolRange = 10f;
    private float patrolToIdleDelay = 0f;
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
    public GameObject Buttercat;

    private NavMeshAgent nav;

    private void Start()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        moveTarget = transform;
        state.CURRENT_STATE = StateMachine.State.Idle;

        thisHealth = transform.GetComponent<Health>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
        patrolStartPosition = transform.position;
        patrolTargetPosition = GetRandomPositionInPatrolRange();

        health.OnHit += OnHit;
        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;

        nav = transform.GetComponent<NavMeshAgent>();
    }

    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
        yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }

        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            SpineTransform.localPosition = Vector3.zero;

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    agent.isStopped = true;
                    speed = 0f;
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleToPatrolDelay)
                    {
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (distanceToPlayer <= detectionRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    break;

                case StateMachine.State.Patrol:
                    agent.isStopped = false;
                    Patrol();
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Moving:
                    agent.isStopped = false;
                    agent.speed = 3;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    if (Time.timeScale == 0f)
                    {
                        break;
                    }

                    if (transform.position.x <= target.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }

                    agent.SetDestination(target.position);
                    if (distanceToPlayer <= detectionAttackRange)
                    {
                        attackPercentage = UnityEngine.Random.Range(0, 10);
                        if (attackPercentage < dashPer)
                        {
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else
                        {
                            directionToPoint = (target.position - transform.position).normalized;
                            state.CURRENT_STATE = StateMachine.State.Dash;
                        }
                    }
                    if (detectionRange < distanceToPlayer)
                    {
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.StopMoving;
                    }

                    aniCount = 1;
                    break;

                case StateMachine.State.StopMoving:
                    agent.isStopped = true;
                    agent.speed = 0;
                    time += Time.deltaTime;
                    if (StopMoving != null)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopMoving, loop: false);
                            aniCount--;
                        }
                    }

                    if (time > 0.4667f)
                    {
                        aniCount = 1;
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }
                    break;

                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    agent.speed = 0f;
                    agent.isStopped = true;
                    AttackTimer += Time.deltaTime;

                    if (AttackTimer >= 1.4f)
                    {
                        state.LockStateChanges = false;
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Dash:
                    if (Dash != null)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Dash, loop: true);
                            aniCount--;
                        }
                    }
                    thisHealth.isInvincible = true;
                    state.LockStateChanges = true;
                    agent.isStopped = false;
                    dashTime += Time.deltaTime;
                    agent.speed = 10f;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    agent.SetDestination(transform.position + directionToPoint);

                    if (dashTime <= 1f)
                    {
                        if (distanceToPlayer <= attackDistance)
                        {
                            playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                        }
                    }
                    else
                    {
                        thisHealth.isInvincible = false;
                        state.LockStateChanges = false;
                        dashTime = 0;
                        aniCount = 1;
                        dashCount++;
                        if(dashCount < 3)
                        {
                            state.CURRENT_STATE = StateMachine.State.DashDelay;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Delay;
                        }
                    }
                    break;

                case StateMachine.State.DashDelay:
                    time += Time.deltaTime;
                    if (StopDash != null)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopDash, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, 0f);
                            aniCount--;
                        }
                    }

                    if (time >= 1f)
                    {
                        time = 0f;
                        aniCount = 1;
                        directionToPoint = (target.position - transform.position).normalized;
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }
                        state.CURRENT_STATE = StateMachine.State.Dash;
                    }

                    break;

                case StateMachine.State.Delay:
                    time += Time.deltaTime;
                    if (StopDash != null && state.PREVIOUS_STATE == StateMachine.State.Dash)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopDash, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, 0f);
                            aniCount--;
                        }
                    }

                    if (time >= 1f)
                    {
                        time = 0f;
                        aniCount = 1;
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }

                        dashCount = 0;
                        if (distanceToPlayer <= detectionJumpRange)
                        {
                            jumpPoint = target.transform.position;
                            state.CURRENT_STATE = StateMachine.State.Jump;
                        }
                        else if (detectionJumpRange < distanceToPlayer && distanceToPlayer <= detectionAttackRange)
                        {
                            directionToPoint = (target.position - transform.position).normalized;
                            attackPercentage = UnityEngine.Random.Range(0, 10);
                            if (attackPercentage < dashPer)
                            {
                                state.CURRENT_STATE = StateMachine.State.Attacking;
                            }
                            else
                            {
                                state.CURRENT_STATE = StateMachine.State.Dash;
                            }
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Moving;
                        }
                    }
                    break;

                case StateMachine.State.Jump:
                    if (Jump != null)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Jump, loop: true);
                            aniCount--;
                        }
                    }

                    state.LockStateChanges = true;

                    if (isJump && !isLand)
                    {
                        agent.isStopped = false;
                        agent.speed = 5f;
                        agent.SetDestination(jumpPoint);
                    }

                    if (isLand)
                    {
                        agent.isStopped = true;
                        speed = 0;
                        isJump = false;
                        isLand = false;
                        state.LockStateChanges = false;
                        state.CURRENT_STATE = StateMachine.State.JumpDelay;
                    }

                    break;

                case StateMachine.State.JumpDelay:
                    time += Time.deltaTime;

                    if (time >= 1f)
                    {
                        time = 0f;
                        aniCount = 1;
                        dashCount = 0;
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }
                        if (detectionJumpRange < distanceToPlayer && distanceToPlayer <= detectionAttackRange)
                        {
                            attackPercentage = UnityEngine.Random.Range(0, 10);
                            if (attackPercentage < dashPer)
                            {
                                state.CURRENT_STATE = StateMachine.State.Attacking;
                            }
                            else
                            {
                                directionToPoint = (target.position - transform.position).normalized;
                                state.CURRENT_STATE = StateMachine.State.Dash;
                            }
                        }
                        else if (distanceToPlayer <= detectionJumpRange)
                        {
                            jumpPoint = target.transform.position;
                            state.CURRENT_STATE = StateMachine.State.Jump;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Moving;
                        }
                    }
                    break;

                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    break;
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

        if (patrolTimer < patrolToIdleDelay)
        {
            agent.SetDestination(patrolTargetPosition);
        }
        else
        {
            patrolTimer = 0f;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (distanceToPlayer <= detectionRange)
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
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if(state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                GameObject bullet = BulletObject;
                Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f), Quaternion.Euler(0, 0, state.facingAngle));
            }
            else if (state.CURRENT_STATE == StateMachine.State.Jump)
            {
                //점프 이펙트 들어갈듯
            }
        }
        else if (e.Data.Name == "jump" || e.Data.Name == "Jump")
        {
            isJump = true;
        }
        else if (e.Data.Name == "land" || e.Data.Name == "Land")
        {
            isLand = true;
            if (distanceToPlayer < jumpDamageRange)
            {
                isLand = true;
                playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
            }
        }
    }

    public void OnDie()
    {
        speed = 0f;
        agent.isStopped = true;
        nav.enabled = false;
        Invoke("DeathEffect", 1.9667f);
        Destroy(gameObject, 1.9667f);
    }

    private void DeathEffect()
    {
        GameObject explosion = ExplosionEffect;
        explosion.transform.position = transform.position;
        Instantiate(explosion);
        GameObject buttercat = Buttercat;
        buttercat.transform.position = transform.position;
        if(transform.localScale.x < 0)
        {
            buttercat.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            buttercat.transform.localScale = new Vector3(1, 1, 1);
        }    
        Instantiate(buttercat);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

}
