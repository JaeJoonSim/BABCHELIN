using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using System.Collections;

public class ButterCat : UnitObject
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
    public AnimationReferenceAsset Spawn;
    public AnimationReferenceAsset Walk;
    public AnimationReferenceAsset Runaway;
    public AnimationReferenceAsset StartDefend;
    public AnimationReferenceAsset Defend;
    public AnimationReferenceAsset StopDefend;
    private float aniCount = 0;


    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float runawaySpeed;
    [SerializeField] float hitSpeed;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float moveTime = 0f;
    [SerializeField] float time = 0;

    [Space]
    [SerializeField] float AttackDelay;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;

    [Space]
    private Health playerHealth;
    private NavMeshAgent agent;
    private float distanceToPlayer;
    Vector3 movePoint;
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
    private float runToAttackDelay;
    [SerializeField] float runMinTime;
    [SerializeField] float runMaxTime;
    private float runTimer;
    [SerializeField] private bool isDefend = false;
    private float defendToAttackDelay;
    [SerializeField] float defendMinTime;
    [SerializeField] float defendMaxTime;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    [Space]
    private int ObjectRand;
    public GameObject DestructionObject;
    public GameObject BombObject;

    public GameObject LandEffect;
    public GameObject DefaultDeathEffect;

    Vector3 direction;

    // Start is called before the first frame update
    private void Start()
    {

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

        health.OnDie += OnDie;
        spineAnimation.AnimationState.Event += OnSpineEvent;

    }
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp(new Vector2(xDir, yDir).magnitude, 0f, 3f);
        }

        if (state.CURRENT_STATE != StateMachine.State.Attacking)
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
            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Spawn:
                    if (Spawn != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Spawn, loop: false);
                            aniCount++;
                        }
                    }

                    time += Time.deltaTime;
                    state.LockStateChanges = true;
                    agent.enabled = false;
                    if (time >= 1.234f)  //스폰 애니메이션 시간
                    {
                        time = 0;
                        aniCount = 0;
                        agent.enabled = true;
                        state.LockStateChanges = false;
                        runToAttackDelay = UnityEngine.Random.Range(runMinTime, runMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }

                    if(transform.localScale.x > 0)
                    {
                        direction = Vector3.left;
                    }
                    else
                    {
                        direction = Vector3.right;
                    }

                    Vector3 moveVector = direction * 5 * Time.deltaTime;
                    transform.Translate(moveVector);


                    SpineTransform.localPosition = Vector3.zero;
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.Idle:
                    Stop();
                    time += Time.deltaTime;
                    if (time >= idleToPatrolDelay)
                    {
                        patrolToIdleDelay = UnityEngine.Random.Range(patrolMinTime, patrolMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Patrol;
                        time = 0f;
                    }

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


                case StateMachine.State.Runaway:
                    if (Runaway != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, Runaway, loop: true);
                            aniCount++;
                        }
                    }

                    if (transform.position.x >= target.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                    RunAway();
                    break;

                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    AttackTimer += Time.deltaTime;

                    if (transform.position.x <= target.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }

                    if (AttackTimer > 1.9333f) //어택 애니메이션 시간
                    {
                        state.LockStateChanges = false;
                        isDefend = false;
                        AttackTimer = 0;
                        runToAttackDelay = UnityEngine.Random.Range(runMinTime, runMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }
                    break;

                case StateMachine.State.Defend:
                    state.LockStateChanges = true;
                    if (StartDefend != null || Defend != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StartDefend, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Defend, loop: true, 0f);
                            aniCount++;
                        }
                    }

                    time += Time.deltaTime;
                    health.isInvincible = true;
                    if(time >= defendToAttackDelay)
                    {
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 0;
                        state.CURRENT_STATE = StateMachine.State.DefendDelay;
                    }
                    break;

                case StateMachine.State.DefendDelay:
                    state.LockStateChanges = true;
                    if (StopDefend != null)
                    {
                        if (aniCount < 1)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopDefend, loop: false);
                            aniCount++;
                        }
                    }

                    time += Time.deltaTime;
                    if (time >= 0.3)
                    {
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 0;
                        health.isInvincible = false;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;

                case StateMachine.State.HitLeft:
                    time += Time.deltaTime;
                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway && !isDefend)
                    {
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }

                        time = 0;
                        isDefend = true;
                        defendToAttackDelay = UnityEngine.Random.Range(defendMinTime, defendMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Defend;
                    }
                    break;

                case StateMachine.State.HitRight:
                    time += Time.deltaTime;
                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway && !isDefend)
                    {
                        if (transform.position.x <= target.position.x)  //보는 방향
                        {
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }

                        time = 0;
                        isDefend = true;
                        defendToAttackDelay = UnityEngine.Random.Range(defendMinTime, defendMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Defend;
                    }

                    break;

            }
        }
    }

    private void RunAway()
    {
        time += Time.deltaTime;
        agent.speed = runawaySpeed;
        directionToPoint = (target.position - transform.position).normalized;
        agent.SetDestination(transform.position - directionToPoint);
        if (time > runToAttackDelay)
        {
            time = 0;
            aniCount = 0;
            runToAttackDelay = UnityEngine.Random.Range(runMinTime, runMaxTime);
            ObjectRand = UnityEngine.Random.Range(0, 10);
            Debug.Log(ObjectRand);
            state.CURRENT_STATE = StateMachine.State.Attacking;
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

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "ateck" || e.Data.Name == "attack")
        {
            ObjectRand = UnityEngine.Random.Range(0, 10);
            Debug.Log(ObjectRand);
            GameObject SpawnBullet;
            if (ObjectRand < 6)
            {
                Debug.Log("Dest");
                SpawnBullet = DestructionObject;
            }
            else
            {
                Debug.Log("bomb");
                SpawnBullet = BombObject;
            }
            SpawnBullet.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            Instantiate(SpawnBullet);
        }
        else if (e.Data.Name == "land" || e.Data.Name == "Land")
        {
            if(state.CURRENT_STATE == StateMachine.State.Spawn)
            {
                GameObject landeffect = LandEffect;
                landeffect.transform.position = transform.position;
                Instantiate(landeffect);
            }
        }
    }

    public void OnDie()
    {
        speed = 0f;
        Invoke("DeathEffect", 2f);
        Destroy(gameObject, 2f);
    }

    private void DeathEffect()
    {
        GameObject explosion = DefaultDeathEffect;
        explosion.transform.position = transform.position;
        Instantiate(explosion);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
