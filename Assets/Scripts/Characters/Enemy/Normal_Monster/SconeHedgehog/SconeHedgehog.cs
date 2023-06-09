using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using System.Collections;

public class SconeHedgehog : UnitObject
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
    public AnimationReferenceAsset Notice;
    public AnimationReferenceAsset StartMoving;
    public AnimationReferenceAsset MovingToDash;
    public AnimationReferenceAsset StartDash;
    public AnimationReferenceAsset Dash;
    public AnimationReferenceAsset StopDash;
    public AnimationReferenceAsset Jump;
    public AnimationReferenceAsset NormalHit;
    public AnimationReferenceAsset ChaseHit;
    private float aniCount = 0;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float detectionJumpRange;
    [SerializeField] float hitDistance;
    [SerializeField] float jumpDamageRange;
    [SerializeField] float attackDistance = 0f;
    [SerializeField] float dashTime = 0f;
    [SerializeField] float delayTime = 5f;
    [SerializeField] float time = 0;
    [SerializeField] float AttackTimer = 0;
    private int paternPer;
    [SerializeField] int dashPer;
    [SerializeField] int jumpPer;
    private int dashCount = 0;
    private bool isJump = false;
    private bool isLand = false;
    private float jumpTime = 0;

    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float hitSpeed;


    [Space]
    private Health playerHealth;
    private NavMeshAgent agent;
    private Collider2D col;
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
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;


    private GameObject AttackPoint;
    public GameObject jumpPointObject;
    private GameObject jumppointobj;
    public GameObject BulletObject;
    public GameObject DeathBulletObject;

    public GameObject DashEffect_L;
    public GameObject DashEffect_R;
    public GameObject LandEffect;
    public GameObject LandEffect2;
    public GameObject DefaulDeathEffect;
    public GameObject WaterEffect;
    public GameObject BreakEffect;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        moveTarget = transform;
        state.CURRENT_STATE = StateMachine.State.Idle;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider2D>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
        patrolStartPosition = transform.position;
        patrolTargetPosition = GetRandomPositionInPatrolRange();

        AttackPoint = transform.GetChild(1).gameObject;

        DashEffect_L.SetActive(false);
        DashEffect_R.SetActive(false);

        health.OnHit += OnHit;
        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
        yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);
        if (playerHealth.CurrentHP() <= 0)
        {
            state.CURRENT_STATE = StateMachine.State.Idle;
        }


        if (state.CURRENT_STATE != StateMachine.State.Dead || state.CURRENT_STATE != StateMachine.State.Jump)
        {
            BodyHit();
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
            SpineTransform.localPosition = Vector3.zero;

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    Stop();
                    time += Time.deltaTime;
                    if (time >= idleToPatrolDelay)
                    {
                        patrolToIdleDelay = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        time = 0f;
                    }
                    DashEffect_L.SetActive(false);
                    DashEffect_R.SetActive(false);

                    if (distanceToPlayer <= detectionRange)
                    {
                        time = 0f;
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.Notice;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    break;

                case StateMachine.State.Patrol:
                    if (Walk != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Walk, loop: true);
                            aniCount++;
                        }
                    }
                    agent.isStopped = false;
                    Patrol();
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Notice:
                    Stop();
                    if (Notice != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Notice, loop: false);
                            aniCount++;
                        }
                    }
                    time += Time.deltaTime;
                    state.LockStateChanges = true;

                    if (time >= 1.333f)
                    {
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.StartMoving;
                    }
                    break;

                case StateMachine.State.StartMoving:
                    time += Time.deltaTime;
                    Stop();
                    state.LockStateChanges = true;
                    if (StartMoving != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StartMoving, loop: false);
                            aniCount++;
                        }
                    }

                    if (transform.position.x <= target.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }

                    if (time >= 1.3667f)
                    {
                        time = 0;
                        aniCount = 0;
                        state.LockStateChanges = false;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }
                    break;

                case StateMachine.State.Moving:
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
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
                        paternPer = UnityEngine.Random.Range(0, 10);
                        if (paternPer >= dashPer)
                        {
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else
                        {
                            directionToPoint = (target.position - transform.position).normalized;
                            state.CURRENT_STATE = StateMachine.State.StartDash;
                        }
                    }
                    aniCount = 0;
                    break;

                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    Stop();
                    AttackTimer += Time.deltaTime;
                    DashEffect_L.SetActive(false);
                    DashEffect_R.SetActive(false);

                    if (AttackTimer < 0.5666f)
                    {
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }
                    }

                    if (AttackTimer >= 1.4f)
                    {
                        state.LockStateChanges = false;
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    break;

                case StateMachine.State.StartDash:
                    time += Time.deltaTime;
                    state.LockStateChanges = true;
                    if (state.PREVIOUS_STATE == StateMachine.State.Delay || state.PREVIOUS_STATE == StateMachine.State.JumpDelay || state.PREVIOUS_STATE == StateMachine.State.DashDelay)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StartDash, loop: false);
                            aniCount++;
                        }
                        agent.SetDestination(transform.position + directionToPoint);
                        agent.speed = 5;
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        Stop();
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, MovingToDash, loop: false);
                            aniCount++;
                        }
                    }

                    if (state.PREVIOUS_STATE == StateMachine.State.Delay || state.PREVIOUS_STATE == StateMachine.State.JumpDelay || state.PREVIOUS_STATE == StateMachine.State.DashDelay)
                    {
                        if (time >= 1.6f)
                        {
                            state.LockStateChanges = false;
                            time = 0;
                            aniCount = 0;
                            state.CURRENT_STATE = StateMachine.State.Dash;
                        }
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        if (time >= 0.5333f)
                        {
                            state.LockStateChanges = false;
                            time = 0;
                            aniCount = 0;
                            state.CURRENT_STATE = StateMachine.State.Dash;
                        }
                    }

                    break;

                case StateMachine.State.Dash:
                    if (Dash != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Dash, loop: true);
                            aniCount++;
                        }
                    }
                    health.isInvincible = true;
                    state.LockStateChanges = true;
                    agent.isStopped = false;
                    time += Time.deltaTime;
                    agent.speed = dashSpeed;
                    state.PREVIOUS_STATE = StateMachine.State.Dash;

                    agent.SetDestination(transform.position + directionToPoint);

                    if (transform.localScale.x > 0)
                    {
                        DashEffect_R.SetActive(true);
                    }
                    else
                    {
                        DashEffect_L.SetActive(true);
                    }

                    if (time <= dashTime)
                    {
                        if (distanceToPlayer <= attackDistance)
                        {
                            playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                        }
                    }
                    else
                    {
                        health.isInvincible = false;
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 0;
                        dashCount++;
                        if (dashCount < 3)
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
                    Stop();
                    time += Time.deltaTime;
                    if (StopDash != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopDash, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, 0f);
                            aniCount++;
                        }
                    }
                    DashEffect_L.SetActive(false);
                    DashEffect_R.SetActive(false);
                    state.PREVIOUS_STATE = StateMachine.State.DashDelay;

                    if (time >= 1f)
                    {
                        time = 0f;
                        aniCount = 0;
                        directionToPoint = (target.position - transform.position).normalized;
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }
                        state.CURRENT_STATE = StateMachine.State.StartDash;
                    }

                    break;

                case StateMachine.State.Delay:
                    Stop();
                    DashEffect_L.SetActive(false);
                    DashEffect_R.SetActive(false);
                    time += Time.deltaTime;
                    if (StopDash != null && state.PREVIOUS_STATE == StateMachine.State.Dash)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopDash, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, 0f);
                            aniCount++;
                        }
                    }
                    state.PREVIOUS_STATE = StateMachine.State.Delay;
                    if (time >= 1f)
                    {
                        time = 0f;
                        aniCount = 0;
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
                            paternPer = UnityEngine.Random.Range(0, 10);
                            if (paternPer < jumpPer)
                            {
                                jumpPoint = target.transform.position;
                                state.CURRENT_STATE = StateMachine.State.Jump;
                            }
                            else
                            {
                                state.CURRENT_STATE = StateMachine.State.Attacking;
                            }
                        }
                        else if (detectionJumpRange < distanceToPlayer && distanceToPlayer <= detectionAttackRange)
                        {
                            directionToPoint = (target.position - transform.position).normalized;
                            paternPer = UnityEngine.Random.Range(0, 10);
                            if (paternPer < dashPer)
                            {
                                state.CURRENT_STATE = StateMachine.State.StartDash;
                            }
                            else
                            {
                                state.CURRENT_STATE = StateMachine.State.Attacking;
                            }
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Moving;
                        }
                    }
                    break;

                case StateMachine.State.Jump:
                    time += Time.deltaTime;
                    if (Jump != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Jump, loop: true);
                            aniCount++;
                        }
                    }

                    state.LockStateChanges = true;

                    if (isJump && !isLand)
                    {
                        agent.isStopped = false;
                        agent.speed = 10;
                        jumpTime += Time.deltaTime;
                        agent.SetDestination(jumpPoint);
                    }

                    if (isLand)
                    {
                        agent.isStopped = true;
                        speed = 0;
                        isJump = false;
                        isLand = false;
                    }

                    if(time >= 1.9f)
                    {
                        time = 0;
                        state.LockStateChanges = false;
                        state.CURRENT_STATE = StateMachine.State.JumpDelay;
                    }

                    break;

                case StateMachine.State.JumpDelay:
                    Stop();
                    time += Time.deltaTime;
                    state.PREVIOUS_STATE = StateMachine.State.JumpDelay;
                    if (time >= 1f)
                    {
                        time = 0f;
                        aniCount = 0;
                        dashCount = 0;
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }

                        if (distanceToPlayer <= detectionJumpRange)
                        {
                            paternPer = UnityEngine.Random.Range(0, 10);
                            if (paternPer < jumpPer)
                            {
                                jumpPoint = target.transform.position;
                                state.CURRENT_STATE = StateMachine.State.Jump;
                            }
                            else
                            {
                                state.CURRENT_STATE = StateMachine.State.Attacking;
                            }
                        }
                        else if (detectionJumpRange < distanceToPlayer && distanceToPlayer <= detectionAttackRange)
                        {
                            directionToPoint = (target.position - transform.position).normalized;
                            paternPer = UnityEngine.Random.Range(0, 10);
                            if (paternPer < dashPer)
                            {
                                state.CURRENT_STATE = StateMachine.State.StartDash;
                            }
                            else
                            {
                                state.CURRENT_STATE = StateMachine.State.Attacking;
                            }
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Moving;
                        }
                    }
                    break;

                case StateMachine.State.HitLeft:
                    if(state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        if (aniCount < 2)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, ChaseHit, loop: false);
                            aniCount++;
                        }
                        agent.SetDestination(target.position);
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Idle || state.PREVIOUS_STATE == StateMachine.State.Patrol)
                    {
                        aniCount = 0;
                    }
                    agent.speed = 1;
                    break;
                case StateMachine.State.HitRight:
                    if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        if (aniCount < 2)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, ChaseHit, loop: false);
                            aniCount++;
                        }
                        agent.SetDestination(target.position);
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Idle || state.PREVIOUS_STATE == StateMachine.State.Patrol)
                    {
                        aniCount = 0;
                    }
                    agent.speed = 1;
                    break;
            }
        }
    }

    private void Patrol()
    {
        time += Time.deltaTime;

        if (transform.position.x <= patrolTargetPosition.x)  //보는 방향
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Vector3.Distance(transform.position, patrolTargetPosition) < 0.5f)
        {
            patrolTargetPosition = GetRandomPositionInPatrolRange();
        }

        if (time < patrolToIdleDelay)
        {
            agent.SetDestination(patrolTargetPosition);
            agent.speed = patrolSpeed;
        }
        else
        {
            time = 0f;
            aniCount = 0;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (distanceToPlayer <= detectionRange)
        {
            time = 0;
            aniCount = 0;
            state.CURRENT_STATE = StateMachine.State.Notice;
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

    private void Stop()
    {
        agent.isStopped = true;
        speed = 0;
    }

    private void BodyHit()
    {
        if (distanceToPlayer < hitDistance)
        {
            if (state.CURRENT_STATE != StateMachine.State.Jump)
                playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        Debug.Log(e.Data.Name);
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                GameObject bullet = Instantiate(BulletObject);
                if(transform.localScale.x < 0)
                {
                    bullet.transform.position = new Vector3(transform.position.x - 0.638f, transform.position.y + 0.372f, transform.position.z - 0.526f);
                }
                else
                {
                    bullet.transform.position = new Vector3(transform.position.x + 0.638f, transform.position.y + 0.372f, transform.position.z - 0.526f);
                }
                //Instantiate(bullet, AttackPoint.transform);
            }
        }
        else if (e.Data.Name == "jump" || e.Data.Name == "Jump")
        {
            jumpPointObject.transform.position = jumpPoint;
            jumppointobj = Instantiate(jumpPointObject);
            jumppointobj.SetActive(true);
            isJump = true;
        }
        else if (e.Data.Name == "land" || e.Data.Name == "Land")
        {
            isLand = true;
            //Spine.Skeleton
            if (state.CURRENT_STATE == StateMachine.State.Jump)
            {
                Debug.Log("Land");
                Destroy(jumppointobj);
                GameObject landeffect = LandEffect;
                landeffect.transform.position = transform.position;
                GameObject landeffect2 = LandEffect2;
                landeffect2.transform.position = transform.position;
                Instantiate(landeffect);
                Instantiate(landeffect2);
                GameObject watereffect = WaterEffect;
                watereffect.transform.position = transform.position;
                Instantiate(watereffect);
            }

            if (distanceToPlayer < jumpDamageRange)
            {
                playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
            }
        }
        else if(e.Data.Name == "death")
        {
            GameObject daethbullet = DeathBulletObject;
            daethbullet.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            Instantiate(daethbullet);
        }
    }

    public void OnDie()
    {
        speed = 0f;
        agent.isStopped = true;
        agent.enabled = false;
        col.enabled = false;
        DashEffect_L.SetActive(false);
        DashEffect_R.SetActive(false);
        Invoke("DeathEffect", 1.9667f);
        Destroy(gameObject, 1.9667f);
    }

    private void DeathEffect()
    {
        GameObject death = DefaulDeathEffect;
        death.transform.position = transform.position;
        Instantiate(death);
        GameObject breakEffect = BreakEffect;
        breakEffect.transform.position = transform.position;
        Instantiate(breakEffect);
        GameObject watereffect = WaterEffect;
        watereffect.transform.position = transform.position;
        Instantiate(watereffect);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

}
