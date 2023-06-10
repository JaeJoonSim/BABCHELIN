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

    public LayerMask obstacleMask;


    [Space]
    [SerializeField] float patrolSpeed;
    [SerializeField] float runawaySpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float hitSpeed;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float hitDistance;
    [SerializeField] float time = 0;

    [Space]
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;

    [Space]
    private Health playerHealth;
    private NavMeshAgent agent;
    private Collider2D col;
    private float distanceToPlayer;
    Vector3 spawnPoint;
    Vector3 directionToPoint;



    [Space]
    private float runawayTime;
    [SerializeField] float runMinTime;
    [SerializeField] float runMaxTime;
    private float runTimer;
    [SerializeField] private bool isDefend = false;
    private float defendTime;
    [SerializeField] float defendMinTime;
    [SerializeField] float defendMaxTime;
    private bool isShield = false;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    [Space]
    public GameObject BombObject;

    public GameObject LandEffect;
    public GameObject ShieldEffect;
    public GameObject DefaultDeathEffect;

    Vector3 direction;

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    private void Start()
    {

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
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, target.position);

        FindVisibleTargets();
        if (playerHealth.CurrentHP() <= 0)
        {
            state.CURRENT_STATE = StateMachine.State.Idle;
        }

        SetRotation();

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
                    health.isInvincible = true;
                    if (time >= 1.234f)  //스폰 애니메이션 시간
                    {
                        time = 0;
                        aniCount = 0;
                        state.LockStateChanges = false;
                        health.isInvincible = false;
                        runawayTime = UnityEngine.Random.Range(runMinTime, runMaxTime);
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

                    if(transform.localScale.x > 0)
                    {
                        //direction = Vector3.left;
                        spawnPoint = new Vector3(transform.position.x - 6, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        //direction = Vector3.right;
                        spawnPoint = new Vector3(transform.position.x + 6, transform.position.y, transform.position.z);
                    }

                    agent.SetDestination(spawnPoint);
                    //Vector3 moveVector = direction * 5 * Time.deltaTime;
                    //transform.Translate(moveVector);


                    SpineTransform.localPosition = Vector3.zero;
                    agent.speed = 5;
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
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }

                        aniCount = 0;
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
                    state.PREVIOUS_STATE = StateMachine.State.Runaway;
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
                    Stop();
                    state.LockStateChanges = true;
                    AttackTimer += Time.deltaTime;

                    if (AttackTimer > 1.9333f) //어택 애니메이션 시간
                    {
                        state.LockStateChanges = false;
                        isDefend = false;
                        time = 0;
                        AttackTimer = 0;
                        runawayTime = UnityEngine.Random.Range(runMinTime, runMaxTime);
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
                    if(time >= defendTime)
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
                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway)
                    {
                        if (!isDefend)
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
                            aniCount = 0;
                            isDefend = true;
                            defendTime = UnityEngine.Random.Range(defendMinTime, defendMaxTime);
                            isShield = true;
                            state.CURRENT_STATE = StateMachine.State.Defend;
                        }
                        else
                        {
                            time += Time.deltaTime;
                        }
                    }
                    break;

                case StateMachine.State.HitRight:
                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway)
                    {
                        if (!isDefend)
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
                            aniCount = 0;
                            isDefend = true;
                            defendTime = UnityEngine.Random.Range(defendMinTime, defendMaxTime);
                            isShield = true;
                            state.CURRENT_STATE = StateMachine.State.Defend;
                        }
                        else
                        {
                            time += Time.deltaTime;
                        }
                    }
                    break;

            }
        }
    }

    private void ShieldOn()
    {
        if(isShield)
        {
            isShield = false;
        }
    }
    public bool FindVisibleTargets()
    {
        //거리가 시야 범위 안에 들어오면
        if (distanceToPlayer <= detectionRange)
        {
            //플레이어의 방향
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            // 공격 범위 안에 들어오면 
            if (!Physics2D.Raycast(transform.position, dirToTarget, distanceToPlayer, obstacleMask))
            {
                return true;
            }
        }
        return false;
    }


    private void RunAway()
    {
        time += Time.deltaTime;
        agent.isStopped = false;
        agent.speed = runawaySpeed;
        directionToPoint = (target.position - transform.position).normalized;
        agent.SetDestination(transform.position - directionToPoint);
        if (time >= runawayTime)
        {
            time = 0;
            aniCount = 0;
            runawayTime = UnityEngine.Random.Range(runMinTime, runMaxTime);
            if (distanceToPlayer <= detectionAttackRange)
            {
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
                state.CURRENT_STATE = StateMachine.State.Moving;
            }
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

    private void SetRotation()
    {
        if (transform.rotation.x != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "ateck" || e.Data.Name == "attack")
        {
            GameObject SpawnBullet = BombObject;
            SpawnBullet.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            Instantiate(SpawnBullet);
        }
        else if (e.Data.Name == "land" || e.Data.Name == "Land")
        {
            if (state.CURRENT_STATE == StateMachine.State.Spawn)
            {
                GameObject landeffect = LandEffect;
                landeffect.transform.position = transform.position;
                Instantiate(landeffect);
            }
        }
        else if (e.Data.Name == "defend" || e.Data.Name == "Defend")
        {
            Debug.Log("Shield");
            GameObject shieldeffect = Instantiate(ShieldEffect);
            shieldeffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            //shieldeffect.GetComponent<ParticleSystem>().Play();
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
        GameObject explosion = DefaultDeathEffect;
        explosion.transform.position = transform.position;
        Instantiate(explosion);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionAttackRange);
    }
}
