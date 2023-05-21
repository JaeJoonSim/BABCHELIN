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
    private bool isDefend;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;

    [Space]
    private int ObjectRand;
    public GameObject DestructionObject;
    public GameObject BombObject;
    public GameObject ExplosionEffect;

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

                    if(time >= 1.234f)  //스폰 애니메이션 시간
                    {
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }

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

                    if (AttackTimer > 3.21f) //어택 애니메이션 시간
                    {
                        state.LockStateChanges = false;
                        AttackTimer = 0;
                        state.CURRENT_STATE = StateMachine.State.Runaway;
                    }
                    break;

                case StateMachine.State.Defend:
                    time += Time.deltaTime;

                    if(time >= 2.46f)   //defend 애니메이션 시간
                    {
                        time = 0;
                        state.CURRENT_STATE = StateMachine.State.Attacking;
                    }
                    break;

                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    time += Time.deltaTime;
                    if (state.PREVIOUS_STATE == StateMachine.State.Runaway && !isDefend)
                    {
                        time = 0;
                        isDefend = true;
                        state.PREVIOUS_STATE = StateMachine.State.Defend;
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
            state.CURRENT_STATE = StateMachine.State.Attacking;
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            GameObject SpawnBullet;
            if (ObjectRand < 6)
            {
                SpawnBullet = DestructionObject;
            }
            else
            {
                SpawnBullet = BombObject;
            }
            Instantiate(SpawnBullet, transform);
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
    }
}
