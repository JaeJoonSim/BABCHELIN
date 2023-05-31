using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using System.Collections;

public class CookieMouse2 : UnitObject
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
    public AnimationReferenceAsset Runaway;
    public AnimationReferenceAsset StartSliding;
    public AnimationReferenceAsset Sliding;
    private int aniCount = 0;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float attackDistance = 0f;
    [SerializeField] float moveTime = 0f;

    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float slidingSpeed;
    [SerializeField] float runawaySpeed;
    [SerializeField] float hitSpeed;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float moveSpeed;

    private Health playerHealth;
    private NavMeshAgent agent;
    private Collider2D col;
    private float distanceToPlayer;
    Vector3 movePoint;
    Vector3 directionToPoint;

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
    
    //public GameObject SlideEffect;
    public GameObject SlideEffect_L;
    public GameObject SlideEffect_R;
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
        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
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
                    SlideEffect_L.SetActive(false);
                    SlideEffect_R.SetActive(false);
                    idleTimer += Time.deltaTime;

                    if (idleTimer >= idleToPatrolDelay)
                    {
                        patrolMoveDuration = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
                    }

                    if (distanceToPlayer <= detectionRange)
                    {
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }

                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;

                    break;

                case StateMachine.State.Moving:
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
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

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;


                    if (distanceToPlayer <= detectionAttackRange)
                    {
                        movePoint = target.position;
                        directionToPoint = (movePoint - transform.position).normalized;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;

                case StateMachine.State.HitLeft:
                    if(state.PREVIOUS_STATE == StateMachine.State.Runaway)
                    {
                        moveTime += Time.deltaTime;
                        directionToPoint = (transform.position - target.position).normalized;
                        agent.SetDestination(transform.position + directionToPoint);
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        agent.SetDestination(target.position);
                    }

                    agent.speed = hitSpeed;
                    aniCount = 0;
                    break;
                case StateMachine.State.HitRight:
                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway)
                    {
                        moveTime += Time.deltaTime;
                        directionToPoint = (transform.position - target.position).normalized;
                        agent.SetDestination(transform.position + directionToPoint);
                    }
                    else if (state.PREVIOUS_STATE == StateMachine.State.Moving)
                    {
                        agent.SetDestination(target.position);
                    }

                    agent.speed = hitSpeed;
                    aniCount = 0;
                    break;
                case StateMachine.State.Attacking:
                    if (StartSliding != null && Sliding != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StartSliding, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Sliding, loop: true, 0f);
                            aniCount++;
                        }
                    }

                    state.LockStateChanges = true;
                    agent.isStopped = false;
                    moveTime += Time.deltaTime;
                    agent.speed = slidingSpeed;

                    if (moveTime < 2f)
                    {
                        agent.SetDestination(transform.position + directionToPoint);

                        if (distanceToPlayer <= attackDistance)
                        {
                            playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                        }
                    }
                    else
                    {
                        state.LockStateChanges = false;
                        moveTime = 0;
                        aniCount = 0;
                        if (distanceToPlayer <= detectionAttackRange)
                        {
                            state.CURRENT_STATE = StateMachine.State.Runaway;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Idle;
                        }
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

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
                case StateMachine.State.Runaway:
                    SlideEffect_L.SetActive(false);
                    SlideEffect_R.SetActive(false);
                    if (Runaway != null)
                    {
                        while (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Runaway, loop: true);
                            aniCount++;
                        }
                    }
                    RunAway();
                    break;
            }
        }
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (Vector3.Distance(transform.position, patrolTargetPosition) < 0.5f)
        {
            patrolTargetPosition = GetRandomPositionInPatrolRange();
        }

        if (patrolTimer < patrolMoveDuration)
        {
            agent.SetDestination(patrolTargetPosition);
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
            state.CURRENT_STATE = StateMachine.State.Moving;
        }

        if (transform.position.x <= patrolTargetPosition.x)
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
        agent.speed = runawaySpeed;
        if(moveTime <= 2)
        {
            directionToPoint = (transform.position - target.position).normalized;
            agent.SetDestination(transform.position + directionToPoint);
        }
        else
        {
            moveTime = 0;
            aniCount = 0;
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (transform.position.x <= target.position.x)
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void Stop()
    {
        agent.isStopped = true;
        speed = 0;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "slide" || e.Data.Name == "Slide")
        {
            if (0 <= xDir)
            {
                SlideEffect_L.SetActive(true);
            }
            else
            {
                SlideEffect_R.SetActive(true);
            }
        }
    }

    public void OnDie()
    {
        SlideEffect_L.SetActive(false);
        SlideEffect_R.SetActive(false);
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
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

}
