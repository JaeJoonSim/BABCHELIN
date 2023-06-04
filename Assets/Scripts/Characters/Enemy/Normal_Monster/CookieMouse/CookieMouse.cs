using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class CookieMouse : UnitObject
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

    public float Damaged = 2f;
    bool hasAppliedDamage = false;

    [Space]
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float AttackDistance;
    [SerializeField] float hitDistance;

    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float hitSpeed;

    [Space]
    [SerializeField] float attackDelayMinTime;
    [SerializeField] float attackDelayMaxTime;
    [SerializeField] float time = 0;
    private float AttackTimer;
    private float attackDelay;

    private Health playerHealth;
    private NavMeshAgent agent;
    private Collider2D col;
    private float distanceToPlayer;

    [Space]
    [SerializeField] float patrolRange = 10f;
    private float patrolMoveDuration;
    [SerializeField] float patrolMinTime;
    [SerializeField] float patrolMaxTime;
    private float idleToPatrolDelay;
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

    public GameObject ExplosionEffect;

    private SkeletonAnimation spineAnimation;

    private Cream creamParent;

    private void Start()
    {
        idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
        patrolStartPosition = transform.position;

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

        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;

        creamParent = gameObject.GetComponentInParent<Cream>();
        if (creamParent != null)
            agent.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        if (creamParent != null && !agent.enabled && transform.position.z >= 0)
            agent.enabled = true;

        if (state.CURRENT_STATE == StateMachine.State.Moving || state.CURRENT_STATE == StateMachine.State.Patrol)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
            forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
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
            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:
                    Stop();
                    idleTimer += Time.deltaTime;
                    time = 0;

                    if (idleTimer >= idleToPatrolDelay)
                    {
                        patrolTargetPosition = GetRandomPositionInPatrolRange();
                        patrolMoveDuration = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (distanceToPlayer <= detectionRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Notice;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
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

                    if (time >= 0.8f)
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

                    if (distanceToPlayer <= AttackDistance)
                    {
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;

                case StateMachine.State.HitLeft:
                    if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        agent.SetDestination(target.position);
                    }

                    agent.speed = hitSpeed;
                    break;
                case StateMachine.State.HitRight:
                    if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        agent.SetDestination(target.position);
                    }

                    agent.speed = hitSpeed;
                    break;

                case StateMachine.State.Attacking:
                    SpineTransform.localPosition = Vector3.zero;
                    state.LockStateChanges = true;
                    forceDir = state.facingAngle;
                    AttackTimer += Time.deltaTime;
                    agent.isStopped = true;

                    if (AttackTimer >= 0.7)
                    {
                        AttackTimer = 0;
                        state.LockStateChanges = false;
                        attackDelay = UnityEngine.Random.Range(attackDelayMinTime, attackDelayMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    //forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    //state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    //state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;

                case StateMachine.State.Delay:
                    time += Time.deltaTime;

                    if (time >= attackDelay)
                    {
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }

                    agent.isStopped = true;
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

        if (patrolTimer < patrolMoveDuration)
        {
            agent.SetDestination(patrolTargetPosition);
            agent.isStopped = false;
            agent.speed = patrolSpeed;
        }
        else
        {
            agent.isStopped = true;
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

    private void Stop()
    {
        if (agent != null && agent.enabled)
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
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if (AttackDistance > distanceToPlayer)
                playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
        }
    }

    public void OnDie()
    {
        speed = 0f;
        agent.isStopped = true;
        agent.enabled = false;
        col.enabled = false;
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
