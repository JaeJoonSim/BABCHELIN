using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class CookieMouse3 : UnitObject
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

    private Health playerHealth;
    private NavMeshAgent agent;
    private Collider2D col;
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
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;
    [SerializeField] float time = 0;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    public GameObject BulletObject;
    public GameObject BombEffect;
    public GameObject ExplosionEffect;

    private SkeletonAnimation spineAnimation;

    public override void Awake()
    {
        base.Awake();
    }

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
    }

    public override void Update()
    {
        base.Update();
        SpineTransform.localPosition = Vector3.zero;
        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (playerHealth.CurrentHP() <= 0)
        {
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

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

                    if (time >= idleToPatrolDelay)
                    {
                        patrolMoveDuration = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        time = 0f;
                    }

                    if (distanceToPlayer <= detectionRange)
                    {
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Notice;
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
                    {
                        break;
                    }

                    if (transform.position.x <= target.position.x)
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
                    state.LockStateChanges = true;
                    Stop();
                    AttackTimer += Time.deltaTime;

                    if(AttackTimer >= 0.7667f)
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
                        time = 0f;
                        if (distanceToPlayer <= detectionAttackRange)
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
                    break;
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

        if (time < patrolMoveDuration)
        {
            agent.SetDestination(patrolTargetPosition);
            agent.isStopped = false;
            agent.speed = patrolSpeed;
        }
        else
        {
            aniCount = 0;
            time = 0f;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (distanceToPlayer <= detectionRange)
        {
            aniCount = 0;
            time = 0;
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
        if (e.Data.Name == "attack_grape")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                GameObject bullet = BulletObject;
                Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.Euler(0, 0, state.facingAngle));
            }
            else if(state.CURRENT_STATE == StateMachine.State.Dead)
            {
                GameObject bombEffect = BombEffect;

                if(0 <= xDir)
                {
                    Instantiate(bombEffect, new Vector3(transform.position.x + 0.6f, transform.position.y, transform.position.z), Quaternion.Euler(0, 0, state.facingAngle));
                }
                else
                {
                    Instantiate(bombEffect, new Vector3(transform.position.x - 0.6f, transform.position.y, transform.position.z), Quaternion.Euler(0, 0, state.facingAngle));
                }
            }
        }
    }

    public void OnDie()
    {
        speed = 0f;
        agent.isStopped = true;
        agent.enabled = false;
        col.enabled = false;
        Invoke("DeathEffect", 4f);
        Destroy(gameObject, 4f);
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
        Gizmos.DrawWireSphere(transform.position, detectionAttackRange);
    }

}
