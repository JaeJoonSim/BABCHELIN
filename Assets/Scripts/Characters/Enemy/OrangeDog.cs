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
    private int ac = 0;

    public float Damaged = 2f;
    bool hasAppliedDamage = false;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float AttackDistance = 1f;
    [SerializeField] float JumpDistance;

    [Space]
    [SerializeField] float AttackDelay;
    [SerializeField] float AttackTimer;
    [SerializeField] float delayTime;
    [SerializeField] float time = 0;
    [SerializeField] float time2 = 0;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private float distanceToPlayer;
    Vector3 jumpPoint;
    private float distanceToJumppoint;

    [Space]
    Vector3 directionToTarget;
    [SerializeField] float patrolRange = 10f;
    private float patrolMoveDuration;
    [SerializeField] float patrolMinTime;
    [SerializeField] float patrolMaxTime;
    private float idleToPatrolDelay;
    [SerializeField] float idleMinTime;
    [SerializeField] float idleMaxTime;
    private float idleTimer;
    private float patrolTimer;
    private float hideTimer = 0f;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;
    private float jumpZ;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    float ad = 0;

    public GameObject ExplosionEffect;

    private SkeletonAnimation spineAnimation;

    Rigidbody2D rb2d;

    public float gravity = 9.81f;
    private float fallSpeed = 0f;
    private Vector3 fallDirection = new Vector3(0, 0, 1);

    private void Start()
    {
        rb2d = transform.GetComponent<Rigidbody2D>();

        idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
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
        fallDirection = new Vector3(transform.position.x, transform.position.y, 1);
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            FollowTarget();
        }

        if (state.CURRENT_STATE == StateMachine.State.Moving || state.CURRENT_STATE == StateMachine.State.Patrol)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
            forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
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
                    agent.speed = 1f;
                    idleTimer += Time.deltaTime;
                    hideTimer += Time.deltaTime;

                    if (!isPlayerInRange && idleTimer >= idleToPatrolDelay)
                    {
                        patrolTargetPosition = GetRandomPositionInPatrolRange();
                        patrolMoveDuration = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        idleTimer = 0f;
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
                    agent.speed = 3f;
                    AttackTimer = 0f;
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

                    agent.SetDestination(target.position);

                    if (distanceToPlayer <= AttackDistance)
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
                    forceDir = state.facingAngle;
                    AttackTimer += Time.deltaTime;
                    agent.isStopped = true;

                    if (AttackTimer >= 0.7)
                    {
                        AttackTimer = 0f;
                        state.CURRENT_STATE = StateMachine.State.Delay;
                    }

                    //forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    //state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                    //state.LookAngle = state.facingAngle;
                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;

                    break;
                case StateMachine.State.Jump:
                    SpineTransform.localPosition = Vector3.zero;
                    forceDir = state.facingAngle;
                    agent.isStopped = false;
                    agent.speed = 7f;
                    //float distanceToJumppoint = Vector3.Distance(transform.position, jumpPoint);
                    //if (distanceToPlayer > distanceToJumppoint / 2)
                    //{
                    //    time2 += Time.deltaTime;
                    //    transform.position -= jumpVector;
                    //    //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - time2);
                    //}
                    //else
                    //{
                    //    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + time2);
                    //}

                    //if(transform.position.z > -2)
                    //{
                    //    time += Time.deltaTime;
                    //    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - time);
                    //}
                    //else if (transform.position.z <= -2)
                    //{
                    //    time2 += Time.deltaTime;
                    //    agent.SetDestination(jumpPoint);
                    //    rb2d.gravityScale = 2f;
                    //    //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + time2);
                    //}

                    if(transform.position.z < -5)
                    {
                        fallSpeed -= gravity * Time.deltaTime;
                        transform.position += fallDirection * fallSpeed * Time.deltaTime;
                    }

                    //if (transform.position.z < 0f)
                    //{
                    //    fallSpeed += gravity * Time.deltaTime;
                    //    transform.position += fallDirection * fallSpeed * Time.deltaTime;
                    //}

                    //if (distanceToJumppoint < 0.5f)
                    //{
                    //    playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                    //    state.CURRENT_STATE = StateMachine.State.JumpDelay;
                    //}
                    //if (transform.position.z > -0.03333196f)
                    //{
                    //    playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                    //    transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -0.03333196f);
                    //    rb2d.gravityScale = 0f;
                    //    state.CURRENT_STATE = StateMachine.State.Delay;
                    //}
                    break;

                case StateMachine.State.Delay:
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        time = 0f;

                        if (distanceToPlayer <= JumpDistance)
                        {
                            xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                            if (0 <= xDir)  //보는 방향
                            {
                                this.transform.localScale = new Vector3(1f, 1f, 1f);
                            }
                            else
                            {
                                this.transform.localScale = new Vector3(-1f, 1f, 1f);
                            }
                            jumpPoint = target.transform.position;
                            distanceToJumppoint = Vector3.Distance(transform.position, jumpPoint);
                            time2 = 0;
                            state.CURRENT_STATE = StateMachine.State.Jump;
                        }
                        else
                        {
                            state.CURRENT_STATE = StateMachine.State.Idle;
                        }
                    }

                    agent.isStopped = true;
                    break;

                case StateMachine.State.JumpDelay:
                    time += Time.deltaTime;
                    if (time >= delayTime)
                    {
                        time = 0f;

                        if (distanceToPlayer <= AttackDistance)
                        {
                            state.CURRENT_STATE = StateMachine.State.Attacking;
                        }
                        else if (AttackDistance < distanceToPlayer && distanceToPlayer <= detectionRange)
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
                        state.CURRENT_STATE = StateMachine.State.Moving;
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
            patrolTimer = 0f;
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        if (isPlayerInRange)
        {
            idleToPatrolDelay = UnityEngine.Random.Range(idleMinTime, idleMaxTime);
            state.CURRENT_STATE = StateMachine.State.Moving;
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
