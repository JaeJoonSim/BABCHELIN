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
    public AnimationReferenceAsset StartDefend;
    public AnimationReferenceAsset Defend;
    public AnimationReferenceAsset StopDefend;
    private float aniCount = 1;


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
    public GameObject ExplosionEffect;

    Vector3 direction;

    // Start is called before the first frame update
    private void Start()
    {
        runToAttackDelay = UnityEngine.Random.Range(runMinTime, runMaxTime);

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
                case StateMachine.State.Idle:
                    time += Time.deltaTime;
                    agent.enabled = false;
                    health.isInvincible = true;
                    if (time >= 1.234f)  //스폰 애니메이션 시간
                    {
                        time = 0;
                        agent.enabled = true;
                        health.isInvincible = false;
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

                case StateMachine.State.Runaway:
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
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }
                    break;

                case StateMachine.State.Defend:
                    state.LockStateChanges = true;
                    if (StartDefend != null || Defend != null)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StartDefend, loop: false);
                            anim.AnimationState.AddAnimation(AnimationTrack, Defend, loop: true, 0f);
                            aniCount--;
                        }
                    }

                    time += Time.deltaTime;
                    health.isInvincible = true;
                    if(time >= defendToAttackDelay)
                    {
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 1;
                        state.CURRENT_STATE = StateMachine.State.DefendDelay;
                    }
                    break;

                case StateMachine.State.DefendDelay:
                    state.LockStateChanges = true;
                    if (StopDefend != null)
                    {
                        if (aniCount > 0)
                        {
                            anim.AnimationState.SetAnimation(AnimationTrack, StopDefend, loop: false);
                            aniCount--;
                        }
                    }

                    time += Time.deltaTime;
                    if (time >= 0.3)
                    {
                        state.LockStateChanges = false;
                        time = 0;
                        aniCount = 1;
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
        agent.speed = 1.5f;
        directionToPoint = (target.position - transform.position).normalized;
        agent.SetDestination(transform.position - directionToPoint);
        if (time > runToAttackDelay)
        {
            time = 0;
            runToAttackDelay = UnityEngine.Random.Range(runMinTime, runMaxTime);
            ObjectRand = UnityEngine.Random.Range(0, 10);
            Debug.Log(ObjectRand);
            state.CURRENT_STATE = StateMachine.State.Attacking;
        }
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
    }

    public void OnDie()
    {
        speed = 0f;
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
    }
}
