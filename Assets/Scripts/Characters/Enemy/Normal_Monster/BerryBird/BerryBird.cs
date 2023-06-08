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
    public AnimationReferenceAsset Walk;
    public AnimationReferenceAsset Notice;
    public AnimationReferenceAsset Runaway;
    private float aniCount = 0;

    public float Damaged = 1f;

    [Space]
    [SerializeField] Transform target;
    private Transform flowerPot;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float hitDistance;

    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float runawaySpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float hitSpeed;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;
    [SerializeField] float delayTime = 1f;
    [SerializeField] float time = 0;

    private Health playerHealth;
    private NavMeshAgent agent;
    private Collider2D col;
    private float distanceToPlayer;
    Vector3 directionToPoint;
    private StateMachine[] otherBirdState;

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
    public GameObject WaterEffect;

    private SkeletonAnimation spineAnimation;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        flowerPot = transform.parent.GetChild(0).transform;
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider2D>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        otherBirdState = new StateMachine[transform.parent.childCount - 1];         //다른 베뱁새 상태 가져오기
        for (int a = 0; a < otherBirdState.Length; a++)
        {
            otherBirdState[a] = transform.parent.GetChild(a + 1).GetComponent<StateMachine>();
        }

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
        SpineTransform.localPosition = Vector3.zero;

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.position);
            BodyHit();
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
                case StateMachine.State.Idle:
                    Stop();
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleToPatrolDelay)
                    {
                        patrolTargetPosition = GetRandomPositionInPatrolRange();
                        patrolToIdleDelay = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (distanceToPlayer <= detectionRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Notice;
                    }

                    if (flowerPot == null)
                    {
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }

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
                    Patrol();
                    if (flowerPot == null)
                    {
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }
                    break;
                case StateMachine.State.Notice:
                    if (Notice != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Notice, loop: false);
                            aniCount++;
                        }
                    }

                    time += Time.deltaTime;

                    if(time >= 0.6333f)
                    {
                        time = 0;
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }
                    break;

                case StateMachine.State.Runaway:
                    if (Runaway != null && Notice != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Runaway, loop: true);
                            aniCount++;
                        }
                    }
                    if (flowerPot == null)
                    {
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }
                    RunAway();
                    break;

                case StateMachine.State.Moving:
                    agent.speed = chaseSpeed;
                    agent.isStopped = false;
                    if (Time.timeScale == 0f)
                        break;

                    if (transform.position.x <= target.position.x)  //보는 방향
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

                    break;

                case StateMachine.State.HitLeft:
                    aniCount = 0;
                    agent.speed = hitSpeed;

                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway || state.PREVIOUS_STATE == StateMachine.State.Idle || state.PREVIOUS_STATE == StateMachine.State.Patrol || state.PREVIOUS_STATE == StateMachine.State.Notice)
                    {
                        for (int a = 0; a < otherBirdState.Length; a++)
                        {
                            if (otherBirdState[a].gameObject != gameObject)
                            {
                                otherBirdState[a].CURRENT_STATE = StateMachine.State.Notice;
                            }
                            else
                            {
                                otherBirdState[a].PREVIOUS_STATE = StateMachine.State.Notice;
                            }
                        }
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        agent.SetDestination(target.position);
                    }
                    break;
                case StateMachine.State.HitRight:
                    aniCount = 0;
                    agent.speed = hitSpeed;

                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway || state.PREVIOUS_STATE == StateMachine.State.Idle || state.PREVIOUS_STATE == StateMachine.State.Patrol)
                    {
                        for (int a = 0; a < otherBirdState.Length; a++)
                        {
                            if (otherBirdState[a].gameObject != gameObject)
                            {
                                otherBirdState[a].CURRENT_STATE = StateMachine.State.Moving;
                            }
                            else
                            {
                                otherBirdState[a].PREVIOUS_STATE = StateMachine.State.Moving;
                            }
                        }
                    }
                    else if(state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        agent.SetDestination(target.position);
                    }
                    break;

                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    AttackTimer += Time.deltaTime;
                    Stop();

                    if (target.position.x <= transform.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }

                    if (AttackTimer >= 1.0333f)
                    {
                        state.LockStateChanges = false;
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    break;

                case StateMachine.State.Delay:
                    Stop();
                    time += Time.deltaTime;
                    state.PREVIOUS_STATE = StateMachine.State.Delay;
                    if (time >= delayTime)
                    {
                        time = 0;
                        if (distanceToPlayer <= detectionAttackRange)
                        {
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Moving;
                        }
                    }
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
            agent.isStopped = false;
            agent.speed = patrolSpeed;
        }
        else
        {
            aniCount = 0;
            patrolTimer = 0f;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (distanceToPlayer <= detectionRange)
        {
            aniCount = 0;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Notice;
        }

        if (transform.position.x <= patrolTargetPosition.x)  //보는 방향
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
        agent.isStopped = false;
        agent.speed = runawaySpeed;
        directionToPoint = (transform.position - target.position).normalized;
        xDir = Mathf.Clamp((transform.position.x + directionToPoint.x), -1f, 1f);
        if (target.position.x <= transform.position.x)  //보는 방향
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        agent.SetDestination(transform.position + directionToPoint);

        if (distanceToPlayer > detectionRange)
        {
            aniCount = 0;
            state.CURRENT_STATE = StateMachine.State.Idle;
        }
    }

    private void Stop()
    {
        agent.isStopped = true;
        speed = 0;
    }

    private void BodyHit()
    {
        if (distanceToPlayer < hitDistance)
            playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
    }

    private bool IsAttackAnimationPlaying()
    {
        var currentAnimation = spineAnimation.AnimationState.GetCurrent(0);
        return currentAnimation.Animation.Name.ToLower().Contains("attack") && !currentAnimation.IsComplete;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "ATTACK" || e.Data.Name == "Attack")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                GameObject bullet = BulletObject;
                Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.Euler(0, 0, state.facingAngle));
            }
        }
    }

    public void OnDie()
    {
        speed = 0f;
        agent.isStopped = true;
        agent.enabled = false;
        col.enabled = false;
        Invoke("DeathEffect", 1f);
        Destroy(gameObject, 1f);
    }

    private void DeathEffect()
    {
        GameObject explosion = ExplosionEffect;
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
        Instantiate(explosion);
        GameObject water = WaterEffect;
        water.transform.position = transform.position;
        Instantiate(water);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionAttackRange);
    }


}
