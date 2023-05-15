using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skunk : UnitObject
{
    [SerializeField] private PatternManager patternManager;

    [SerializeField] private List<BasicPatternScriptableObject> basicPatterns;
    [SerializeField] private List<GimmickScriptableObject> gimmickPatterns;

    [Space]
    public Transform SpineTransform;

    public float Damaged = 1f;
    bool hasAppliedDamage = false;

    [Space]
    [Header("부위 파괴")]
    public float DestructionGauge = 10f;
    public float DestructionHP = 2f;
    public float DestructionTime = 3f;
    private float currentDestructionTime;
    private float currentDestructionHP;
    private float originGauge;
    public bool destructionStun = false;
    public int destructionCount = 3;
    private IEnumerator destructionCoroutine;

    [Space]
    [Header("부위파괴시 독가스 설정")]
    public GameObject poisonGasPrefab;
    public float poisonGasRadius;
    public float poisonGasDamage;
    public float poisonGasDuration;

    [Space]

    [SerializeField] Transform target;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float AttackDistance = 2f;

    [Space]
    [SerializeField] float AttackDelay = 2f;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;
    private float distanceToPlayer;
    [HideInInspector] public bool wasFarting = false;

    [Space]

    [HideInInspector] public float forceDir;
    [HideInInspector] public float xDir;
    [HideInInspector] public float yDir;

    public GameObject fartPrefab;

    private SkeletonAnimation spineAnimation;


    public GameObject bulletPrefab;

    [Space]

    private int partDestruction = 3;

    private bool isTailing = false;
    public bool showTailigPattern = false;
    [DrawIf("showTailigPattern", true)]
    public int numberOfBullets = 10;
    [DrawIf("showTailigPattern", true)]
    public float maxAngle = 90f;
    [DrawIf("showTailigPattern", true)]
    public float fireRate = 0.1f;
    [DrawIf("showTailigPattern", true)]
    public float bulletSpeed = 3.0f;

    private bool isThrowing = false;
    public bool showThrowPattern = false;
    [DrawIf("showThrowPattern", true)]
    public GameObject mapObject;
    [DrawIf("showThrowPattern", true)]
    public int numberOfBombs = 2;
    [DrawIf("showThrowPattern", true)]
    public float dropDelay = 0.5f;
    [DrawIf("showThrowPattern", true)]
    public float zOffset = -10f;
    public GameObject[] bombPrefabs;

    private bool wasJumping = false;
    private bool hasJumpAttacked = false;
    public bool showJumpPattern = false;
    [DrawIf("showJumpPattern", true)]
    public GameObject shockwavePrefab;
    [DrawIf("showJumpPattern", true)]
    public float shockwaveRange = 5f;
    [DrawIf("showJumpPattern", true)]
    public float shockwaveDuration = 0.5f;

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        //spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (patternManager == null)
        {
            patternManager = FindObjectOfType<PatternManager>();
        }

        patternManager.basicPatterns = basicPatterns;
        patternManager.gimmickPatterns = gimmickPatterns;
        patternManager.Initialize();

        originGauge = DestructionGauge;
        currentDestructionTime = DestructionTime - 0.1f;

        destructionCoroutine = StartDestructTime();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        health.OnDie += OnDie;
        //spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        health.OnDie -= OnDie;
        //spineAnimation.AnimationState.Event -= OnSpineEvent;
    }

    public override void Update()
    {
        base.Update();

        if (state.CURRENT_STATE != StateMachine.State.Farting)
        {
            wasFarting = false;
        }

        DestructionPart();

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            switch (state.CURRENT_STATE)
            {

                case StateMachine.State.Idle:
                    break;
                case StateMachine.State.Moving:
                    break;
                case StateMachine.State.Attacking:
                    break;
                case StateMachine.State.Runaway:
                    break;
                case StateMachine.State.Patrol:
                    break;
                case StateMachine.State.Jump:
                    if (!hasJumpAttacked)
                    {
                        StartCoroutine(JumpAttack());
                        hasJumpAttacked = true;
                    }
                    break;
                case StateMachine.State.Farting:
                    if (!wasFarting)
                    {
                        Fart();
                        wasFarting = true;
                    }
                    break;
                case StateMachine.State.FartShield:
                    break;
                case StateMachine.State.Tailing:
                    if (!isTailing)
                    {
                        StartCoroutine(Tailing());
                    }
                    break;
                case StateMachine.State.Throwing:
                    if (!isThrowing)
                    {
                        StartCoroutine(CreamThrow());
                    }
                    break;
                case StateMachine.State.Dead:
                    break;

                default:
                    break;
            }
            if (state.CURRENT_STATE != StateMachine.State.Jump && hasJumpAttacked)
            {
                hasJumpAttacked = false;
            }

            wasJumping = (state.CURRENT_STATE == StateMachine.State.Jump);
        }

        foreach (var gimmickPattern in patternManager.gimmickPatterns)
        {
            if (health.multipleHealthLine <= gimmickPattern.triggerHealthLine)
            {
                gimmickPattern.CheckHealthThreshold();
            }
        }
    }

    #region Func
    private void FollowTarget()
    {
        if (state.CURRENT_STATE != StateMachine.State.Attacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

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
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);
                agent.SetDestination(target.position);
                if (distanceToPlayer <= AttackDistance)
                {
                    state.CURRENT_STATE = StateMachine.State.Attacking;
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                agent.isStopped = true;
                xDir = 0f;
                yDir = 0f;
            }
        }
    }

    private void DestructionPart()
    {
        if (destructionCount > 0)
        {
            if (DestructionGauge <= 0)
            {
                currentDestructionHP = health.CurrentHP() - DestructionHP;
                
                if(!destructionStun)
                    StartCoroutine(destructionCoroutine);
            }
        }

        if (destructionStun)
        {
            state.CURRENT_STATE = StateMachine.State.Stun;
            currentDestructionTime -= Time.deltaTime;

            if (currentDestructionTime <= 0)
            {
                if (health.CurrentHP() <= currentDestructionHP)
                {
                    destructionCount--;
                    StopCoroutine(destructionCoroutine);
                    destructionStun = false;
                    DestructionGauge = originGauge;
                }
                else if (health.CurrentHP() > currentDestructionHP)
                {
                    StopCoroutine(destructionCoroutine);
                    destructionStun = false;
                    DestructionGauge = originGauge;
                }
            }
        }
        else
        {
            currentDestructionTime = DestructionTime - 0.1f;
        }
    }

    private IEnumerator StartDestructTime()
    {
        destructionStun = true;
        
        GameObject poisonGas = Instantiate(poisonGasPrefab, transform.position, Quaternion.identity);
        poisonGas.GetComponent<PoisonGas>().radius = poisonGasRadius;
        poisonGas.GetComponent<PoisonGas>().damage = poisonGasDamage;
        poisonGas.GetComponent<PoisonGas>().duration = poisonGasDuration;
        
        yield return new WaitForSeconds(DestructionTime);
        
        destructionStun = false;
        yield return null;
    }

    private void Fart()
    {
        if (state.CURRENT_STATE == StateMachine.State.Farting)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            //SpineTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            Instantiate(fartPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator Tailing()
    {
        isTailing = true;
        float angleStep = maxAngle / numberOfBullets;
        float currentAngle = -maxAngle / 2 - 90;
        float angleIncrement = angleStep;

        while (state.CURRENT_STATE == StateMachine.State.Tailing)
        {
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            direction.Normalize();

            GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

            currentAngle += angleIncrement;

            if (currentAngle > maxAngle / 2 - 90)
            {
                angleIncrement = -angleStep;
            }
            else if (currentAngle < -maxAngle / 2 - 90)
            {
                angleIncrement = angleStep;
            }

            yield return new WaitForSeconds(fireRate);
        }
        isTailing = false;
    }

    private IEnumerator CreamThrow()
    {
        isThrowing = true;
        if (state.CURRENT_STATE == StateMachine.State.Throwing)
        {
            if (mapObject == null)
            {
                mapObject = GameObject.FindWithTag("Ground");
            }

            if (mapObject != null)
            {
                BoxCollider2D mapCollider = mapObject.GetComponent<BoxCollider2D>();

                if (mapCollider != null)
                {
                    Bounds mapBounds = mapCollider.bounds;

                    float minX = mapBounds.min.x;
                    float maxX = mapBounds.max.x;
                    float minY = mapBounds.min.y;
                    float maxY = mapBounds.max.y;

                    for (int i = 0; i < numberOfBombs; i++)
                    {
                        Vector3 dropPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), zOffset);

                        GameObject bombPrefab = bombPrefabs[Random.Range(0, bombPrefabs.Length)];

                        Instantiate(bombPrefab, dropPosition, Quaternion.identity);

                        yield return new WaitForSeconds(dropDelay);
                    }
                }
                else
                {
                    Debug.LogError("No BoxCollider2D found on map object.");
                }
            }
            else
            {
                Debug.LogError("Map object not found.");
            }
        }
        isThrowing = false;
    }

    private IEnumerator JumpAttack()
    {
        if (!hasJumpAttacked)
        {
            hasJumpAttacked = true;
            Vector3 shockwavePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            GameObject shockwaveInstance = Instantiate(shockwavePrefab, shockwavePosition, Quaternion.identity);

            shockwaveInstance.GetComponent<Shockwave>().Initialize(shockwaveRange, shockwaveDuration);

            float currentShockwaveRange = 0;
            float detectionRangeLowerLimit = currentShockwaveRange - 0.2f;
            while (currentShockwaveRange <= shockwaveRange)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(shockwavePosition, currentShockwaveRange);
                foreach (Collider2D enemy in hitEnemies)
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                    if (enemy.CompareTag("Player") && distanceToEnemy > detectionRangeLowerLimit)
                    {
                        Health enemyHealth = enemy.GetComponent<Health>();

                        if (enemyHealth != null)
                        {
                            enemyHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                        }
                    }
                }
                currentShockwaveRange += Time.deltaTime * (shockwaveRange / shockwaveDuration);
                detectionRangeLowerLimit = currentShockwaveRange - 0.2f;
                yield return null;
            }
        }
        hasJumpAttacked = false;
    }

    #endregion

    #region Event
    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack" || e.Data.Name == "Attack")
        {
            if (!hasAppliedDamage && state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                if (AttackDistance > distanceToPlayer)
                    playerHealth.Damaged(gameObject, transform.position, Damaged, Health.AttackType.Normal);
                hasAppliedDamage = true;
            }
        }
    }

    public void OnDie()
    {
        agent.speed = 0f;
        Destroy(gameObject, 5f);
    }
    #endregion

    #region Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }
    #endregion
}
