using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;


public class BerryBird3_Single : UnitObject
{
    public Transform SpineTransform;

    [SerializeField] Transform target;
    [SerializeField] float detectionAttackRange;
    [SerializeField] float hitDistance;
    //[SerializeField] float moveTime = 0f;

    [Space]
    [SerializeField] float AttackTimer;
    private float time = 0;

    public float Damaged = 1f;

    private Health playerHealth;
    private NavMeshAgent agent;
    private float distanceToPlayer;

    [Space]
    public float forceDir;
    public float xDir;
    public float yDir;


    public GameObject BulletObject;
    public GameObject ExplosionEffect;
    public GameObject WaterEffect;

    private SkeletonAnimation spineAnimation;

    private NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        nav = transform.GetComponent<NavMeshAgent>();

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
        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            //FollowTarget();
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
                    agent.isStopped = true;
                    agent.speed = 0f;

                    time += Time.deltaTime;
                    if(time >= 0.9334f)
                    {
                        time = 0f;
                        state.CURRENT_STATE = StateMachine.State.Moving;
                    }

                    break;

                case StateMachine.State.Moving:
                    agent.isStopped = false;
                    agent.speed = 3f;
                    AttackTimer = 0f;
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

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    break;

                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                case StateMachine.State.Attacking:
                    state.LockStateChanges = true;
                    agent.speed = 0f;
                    agent.isStopped = true;
                    AttackTimer += Time.deltaTime;

                    if (transform.position.x < target.position.x)  //보는 방향
                    {
                        this.transform.localScale = new Vector3(1f, 1f, 1f);    //오른쪽
                    }
                    else
                    {
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);   //왼쪽
                    }
                    
                    if (AttackTimer >= 0.7667f)
                    {
                        AttackTimer = 0f;
                        if (detectionAttackRange < distanceToPlayer)
                        {
                            state.LockStateChanges = false;
                            state.CURRENT_STATE = StateMachine.State.Moving;
                        }
                    }

                    speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                    break;
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


    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                GameObject bullet = BulletObject;
                Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.Euler(0, 0, state.facingAngle));
            }
        }
    }

    public void OnDie()
    {
        agent.isStopped = true;
        speed = 0f;
        nav.enabled = false;
        Invoke("DeathEffect", 3.2333f);
        Destroy(gameObject, 3.2333f);
    }
    private void DeathEffect()
    {
        GameObject explosion = ExplosionEffect;
        explosion.transform.position = transform.position;
        Instantiate(explosion);
        GameObject water = WaterEffect;
        water.transform.position = transform.position;
        Instantiate(water);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionAttackRange);
    }
}
