using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class BerryBird3_Group : UnitObject
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
    private float aniCount = 0;

    public float Damaged = 1f;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float hitDistance;

    [Space]
    [SerializeField] float patrolSpeed;
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
    private float distanceToPlayer;
    private StateMachine[] otherBirdState;

    [Space]
    [SerializeField] float patrolRange = 10f;
    private float patrolToIdleTime= 0f;
    [SerializeField] float patrolMinTime;
    [SerializeField] float patrolMaxTime;
    private float idleToPatrolTime = 0f;
    [SerializeField] float idleMinTime;
    [SerializeField] float idleMaxTime;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    private GameObject[] AttackPoint;
    public GameObject BulletObject;
    public GameObject SplitEffect;
    public GameObject ExplosionEffect;
    public GameObject Single_Bird;

    private SkeletonAnimation spineAnimation;

    private NavMeshAgent nav;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        nav = transform.GetComponent<NavMeshAgent>();
        idleToPatrolTime = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
        patrolStartPosition = transform.position;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        otherBirdState = new StateMachine[transform.parent.childCount - 1];         //다른 베뱁새 상태 가져오기
        for (int a = 0; a < otherBirdState.Length; a++)
        {
            otherBirdState[a] = transform.parent.GetChild(a + 1).GetComponent<StateMachine>();
        }

        AttackPoint = new GameObject[transform.childCount - 1];
        for (int a = 0; a < 3; a++)
        {
            AttackPoint[a] = transform.GetChild(a + 1).gameObject;
        }

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            BodyHit();
        }
        if (health.CurrentHP() <= 0)
        {
            state.LockStateChanges = false;
        }

        if (health.isInvincible)
        {
            for (int a = 0; a < otherBirdState.Length; a++)
            {
                if (otherBirdState[a].gameObject != gameObject)
                {
                    otherBirdState[a].CURRENT_STATE = StateMachine.State.Notice;
                }
                else
                {
                    otherBirdState[a].PREVIOUS_STATE = StateMachine.State.Moving;
                }
            }
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
                    time += Time.deltaTime;

                    if (time >= idleToPatrolTime)
                    {
                        time = 0f;
                        patrolToIdleTime = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                    }

                    if (distanceToPlayer <= detectionRange)
                    {
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Notice;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    //speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Patrol:
                    if (Walk != null)
                    {
                        while (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Walk, loop: true);
                            aniCount++;
                        }
                    }
                    Patrol();
                    break;

                case StateMachine.State.Notice:
                    Stop();
                    if (Notice != null)
                    {
                        while (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Notice, loop: false);
                            aniCount++;
                        }
                    }
                    time += Time.deltaTime;
                    state.LockStateChanges = true;

                    if (time >= 0.7f)
                    {
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }
                    break;

                case StateMachine.State.Moving:
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    AttackTimer = 0f;
                    if (Time.timeScale == 0f)
                    {
                        break;
                    }

                    agent.SetDestination(target.position);
                    if (transform.position.x <= target.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    if (distanceToPlayer <= detectionAttackRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;

                case StateMachine.State.HitLeft:
                    agent.speed = hitSpeed;
                    aniCount = 0;
                    if (state.PREVIOUS_STATE == StateMachine.State.Idle || state.PREVIOUS_STATE == StateMachine.State.Patrol)
                    {
                        agent.speed = 0;
                        for (int a = 0; a < otherBirdState.Length; a++)
                        {
                            otherBirdState[a].CURRENT_STATE = StateMachine.State.Notice;
                        }
                    }
                    break;
                case StateMachine.State.HitRight:
                    agent.speed = hitSpeed;
                    aniCount = 0;
                    if (state.PREVIOUS_STATE == StateMachine.State.Idle || state.PREVIOUS_STATE == StateMachine.State.Patrol)
                    {
                        agent.speed = 0;
                        for (int a = 0; a < otherBirdState.Length; a++)
                        {
                            otherBirdState[a].CURRENT_STATE = StateMachine.State.Notice;
                        }
                    }
                    break;

                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    Stop();
                    AttackTimer += Time.deltaTime;

                    if(AttackTimer < 0.3666f)
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

                    if (AttackTimer >= 0.7667f)
                    {
                        state.LockStateChanges = false;
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }
                    break;

                case StateMachine.State.Delay:
                    Stop();
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        time = 0f;
                        if(distanceToPlayer <= detectionAttackRange)
                        {
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else if (distanceToPlayer <= detectionRange)
                        {
                            state.CURRENT_STATE = StateMachine.State.Moving;
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


    private void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolTargetPosition) < 0.5f)
        {
            patrolTargetPosition = GetRandomPositionInPatrolRange();
        }

        if (time < patrolToIdleTime)
        {
            agent.SetDestination(patrolTargetPosition);
            agent.isStopped = false;
            agent.speed = patrolSpeed;
        }
        else
        {
            aniCount = 0;
            time = 0f;
            idleToPatrolTime = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (distanceToPlayer <= detectionRange)
        {
            aniCount = 0;
            time = 0f;
            idleToPatrolTime = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Notice;
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

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                Debug.Log(e.Time);
                for (int a = 0; a < 3; a++)
                {
                    GameObject bullet = BulletObject;
                    Instantiate(bullet, AttackPoint[a].transform);
                }
            }
        }
    }

    public void OnDie()
    {
        speed = 0f;
        agent.isStopped = true;
        nav.enabled = false;
        Invoke("DeathEffect", 0.8333f);
        Destroy(gameObject, 0.8333f);
    }

    private void DeathEffect()
    {
        GameObject explosion = ExplosionEffect;
        explosion.transform.position = transform.position;
        Instantiate(explosion);

        GameObject split = SplitEffect;
        split.transform.position = transform.position;
        Instantiate(split);
        for (int a = 0; a < 3; a++)
        {
            GameObject jinjin = Single_Bird;
            Vector3 spawnPosition = new Vector3(transform.localPosition.x + a, transform.localPosition.y + a, 0f);
            jinjin.transform.position = spawnPosition;
            Instantiate(jinjin, transform.parent);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionAttackRange);
    }

}
