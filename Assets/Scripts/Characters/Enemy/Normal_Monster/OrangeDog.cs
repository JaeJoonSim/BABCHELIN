using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class OrangeDog : UnitObject
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
    public AnimationReferenceAsset Hide;
    public AnimationReferenceAsset Hidden;
    public AnimationReferenceAsset Appearance;
    public AnimationReferenceAsset Attack2;
    private int ac = 0;

    public float Damaged = 2f;

    [Space]
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float AttackDistance = 1f;
    [SerializeField] float JumpDistance;

    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;

    [Space]
    [SerializeField] float AttackDelay;
    [SerializeField] float AttackTimer;
    [SerializeField] float delayTime;
    [SerializeField] float time = 0;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange = false;
    private float distanceToPlayer;
    Vector3 jumpPoint;
    private float distanceToJumppoint;

    [Space]
    Vector3 directionToTarget;
    [SerializeField] float patrolRange = 10f;
    private float patrolToIdleTime;
    [SerializeField] float patrolMinTime;
    [SerializeField] float patrolMaxTime;
    private float idleToPatrolTime;
    [SerializeField] float idleMinTime;
    [SerializeField] float idleMaxTime;
    private float hideTimer = 0f;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;
    private float jumpZ;
    private int attackNum = 0;
    private float attackAniTime;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    float ad = 0;

    public GameObject ExplosionEffect;

    private SkeletonAnimation spineAnimation;


    public float gravity = 9.81f;
    private float fallSpeed = 0f;
    private Vector3 fallDirection = new Vector3(0, 0, 1);

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

        if (state.CURRENT_STATE == StateMachine.State.Moving || state.CURRENT_STATE == StateMachine.State.Patrol)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
            forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
        }

        if(state.CURRENT_STATE != StateMachine.State.Attacking)
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
            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    Stop();
                    time += Time.deltaTime;
                    hideTimer += Time.deltaTime;

                    if (!isPlayerInRange && time >= idleToPatrolTime)
                    {
                        patrolTargetPosition = GetRandomPositionInPatrolRange();
                        patrolToIdleTime = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        time = 0f;
                    }

                    if (isPlayerInRange)
                        state.CURRENT_STATE = StateMachine.State.Moving;

                    if(hideTimer >= 10f)
                    {
                        hideTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Hidden;
                    }
                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Patrol:
                    hideTimer += Time.deltaTime;
                    if (hideTimer >= 10f)
                    {
                        hideTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Hidden;
                    }
                    Patrol();
                    break;

                case StateMachine.State.Moving:
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    AttackTimer = 0f;
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

                    agent.SetDestination(target.position);

                    if (distanceToPlayer <= AttackDistance)
                    {
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    time += Time.deltaTime;
                    break;

                case StateMachine.State.Attacking:
                    SpineTransform.localPosition = Vector3.zero;
                    state.LockStateChanges = true;
                    Stop();

                    if (attackAniTime == 0)
                    {
                        attackAniTime = 1.4f;
                    }
                    else
                    {
                        attackAniTime = 0.8667f;
                    }

                    if (attackNum == 1)
                    {
                        if (Attack2 != null)
                        {
                            while (ac < 1)
                            {
                                anim.AnimationState.SetAnimation(AnimationTrack, Attack2, loop: false);
                                ac++;
                            }
                        }
                    }
                    forceDir = state.facingAngle;
                    AttackTimer += Time.deltaTime;
                    agent.isStopped = true;

                    if (AttackTimer >= attackAniTime)
                    {
                        state.LockStateChanges = false;
                        ac = 0;
                        AttackTimer = 0f;
                        time = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;

                case StateMachine.State.Delay:
                    SpineTransform.localPosition = Vector3.zero;
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        time = 0f;
                        attackNum = UnityEngine.Random.Range(0, 2);
                        if (distanceToPlayer <= AttackDistance)
                        {
                            xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                            if (transform.position.x <= target.position.x)  //보는 방향
                            {
                                this.transform.localScale = new Vector3(1f, 1f, 1f);
                            }
                            else
                            {
                                this.transform.localScale = new Vector3(-1f, 1f, 1f);
                            }
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Idle;
                        }
                    }

                    agent.speed = 0f;
                    agent.isStopped = true;
                    break;

                case StateMachine.State.Hidden:
                    SpineTransform.localPosition = Vector3.zero;
                    if (Hidden != null && Hide != null)
                    {
                        while (ac < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Hide, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Hidden, loop: true, 0f);
                            ac++;
                        }
                    }

                    if (isPlayerInRange)
                    {
                        ac = 0;
                        state.CURRENT_STATE = StateMachine.State.Appearance;
                    }

                    agent.isStopped = true;
                    break;

                case StateMachine.State.Appearance:
                    SpineTransform.localPosition = Vector3.zero;
                    time += Time.deltaTime;

                    if (Appearance != null && Hide != null)
                    {
                        while (ac < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Appearance, loop: false);
                            ac++;
                        }
                    }

                    if (time >= 3.3f)
                    {
                        ac = 0;
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Idle;
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

            if (isPlayerInRange && state.CURRENT_STATE != StateMachine.State.Attacking)
            {
                directionToTarget = (target.position - transform.position).normalized;
                xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);
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
        time += Time.deltaTime;

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
            time = 0f;
            idleToPatrolTime = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;

        }

        if (isPlayerInRange)
        {
            idleToPatrolTime = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Moving;
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

    private void Stop()
    {
        agent.isStopped = true;
        speed = 0;
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
        agent.speed = 0f;
        nav.enabled = false;
        Invoke("DeathEffect", 2f);
        Destroy(gameObject, 2f);
    }

    private void DeathEffect()
    {
        GameObject explosion = ExplosionEffect;
        explosion.transform.position = transform.position;
        Instantiate(explosion);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }
}
