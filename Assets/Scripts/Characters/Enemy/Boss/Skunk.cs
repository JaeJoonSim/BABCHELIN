using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skunk : UnitObject
{
    [SerializeField] private PatternManager patternManager;

    [SerializeField] private List<BasicPatternScriptableObject> phase1BasicPatterns;
    [SerializeField] private List<BasicPatternScriptableObject> phase2BasicPatterns;
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
    [HideInInspector] public bool destructionStun = false;
    [HideInInspector] public bool isPatternPause = false;
    public int destructionCount = 3;
    private IEnumerator destructionCoroutine;
    public float InstantKillTimelimit = 60f;
    public float checkMultipleHealthLine = 3f;

    [Header("부위파괴 성공 패턴")]
    public float phaseChangeTimeLimit = 30f;
    public int currentPhase = 1;
    public bool isPhaseChanged = false;
    public float phaseChangeAttackAngle = 180f;

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

    [Header("Runaway settings")]
    public float runawaySpeed = 5f;
    public float bombDropInterval = 2f;
    public float waitAfterReachingDestination = 2f;
    public GameObject bombPrefab;

    public bool isRunningAway = false;
    private Vector3 runawayDestination;
    private float bombDropTimer;

    [Header("Fart Shield settings")]
    public GameObject shieldPrefab;
    private GameObject currentShield;
    public float shieldRegenTime = 5.0f;
    public float shieldRadius = 2.0f;
    public float waitIdleTime = 2.0f;
    [HideInInspector] public bool isShieldActive = false;
    private float shieldRegenTimer;

    [Header("Outburst settings")]
    public int healthLineThreshold = 1;
    public float outburstDamage = 5f;
    public GameObject objectToSpawnDuringOutburst;
    private int lastHealthLine;

    private BossSounds bossSounds;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();
        bossSounds = GetComponent<BossSounds>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (patternManager == null)
        {
            patternManager = FindObjectOfType<PatternManager>();
        }

        patternManager.basicPatterns = phase1BasicPatterns;
        patternManager.gimmickPatterns = gimmickPatterns;
        patternManager.Initialize();

        originGauge = DestructionGauge;
        currentDestructionTime = DestructionTime - 0.1f;

        if (checkMultipleHealthLine >= health.multipleHealthLine)
            checkMultipleHealthLine = health.multipleHealthLine;

        destructionCoroutine = StartDestructTime();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        health.OnDie += OnDie;
        if (spineAnimation == null)
            spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();
        spineAnimation.AnimationState.Event += OnSpineEvent;

        destructionCoroutine = StartDestructTime();
        originGauge = DestructionGauge;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        health.OnDie -= OnDie;
        spineAnimation.AnimationState.Event -= OnSpineEvent;
    }

    public override void Update()
    {
        base.Update();

        if (state.CURRENT_STATE == StateMachine.State.Dead)
        {
            return;
        }

        if (state.CURRENT_STATE != StateMachine.State.Farting)
        {
            wasFarting = false;
        }

        if (InstantKillTimelimit > 0 && currentPhase == 1)
            InstantKillTimelimit -= Time.deltaTime;

        if (InstantKillTimelimit <= 0 && checkMultipleHealthLine <= health.multipleHealthLine)
        {
            isPatternPause = true;
            state.CURRENT_STATE = StateMachine.State.InstantKill;
        }

        if (InstantKillTimelimit > 0 && destructionCount <= 0 && currentPhase == 1 && !isPatternPause)
        {
            isPatternPause = true;
            health.damageDecrease = true;

            state.CURRENT_STATE = StateMachine.State.PhaseChange;
        }

        if (currentPhase == 1 && health.CurrentHP() < health.MaxHP() / 2)
        {
            health.untouchable = true;
            health.SetHP(health.MaxHP() / 2f);
        }
        else if (currentPhase > 1 && state.CURRENT_STATE != StateMachine.State.Dead)
        {
            health.untouchable = false;
        }

        if (currentPhase == 2 && lastHealthLine - health.multipleHealthLine >= healthLineThreshold)
        {
            state.CURRENT_STATE = StateMachine.State.Outburst;
            lastHealthLine = health.multipleHealthLine;
        }

        DestructionPart();

        if (currentPhase > 1 && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Dieing)
        {
            forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
        }

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
                    if (!isRunningAway)
                    {
                        StartCoroutine(RunawayAndDropBombs());
                    }
                    break;
                case StateMachine.State.Patrol:
                    break;
                case StateMachine.State.Jump:
                    break;
                case StateMachine.State.Farting:
                    
                    break;
                case StateMachine.State.FartShield:
                    
                    break;
                case StateMachine.State.Tailing:
                    break;
                case StateMachine.State.Throwing:
                    break;
                case StateMachine.State.InstantKill:
                    break;
                case StateMachine.State.PhaseChange:
                    if (isPhaseChanged)
                    {
                        bossSounds.PlayBossSound(bossSounds.skunk_Explosion);
                        currentPhase++;
                        patternManager.basicPatterns.Clear();
                        patternManager.patternList.Clear();
                        patternManager.basicPatterns = phase2BasicPatterns;
                        isPhaseChanged = false;
                        isPatternPause = false;
                        health.damageDecrease = false;
                        state.CURRENT_STATE = StateMachine.State.Idle;
                    }
                    else if (!isPhaseChanged)
                    {
                        phaseChangeTimeLimit -= Time.deltaTime;

                        if (!isTailing)
                        {
                            StartCoroutine(Tailing(phaseChangeAttackAngle));
                        }
                        if (!isThrowing)
                        {
                            StartCoroutine(CreamThrow());
                        }
                    }

                    if (phaseChangeTimeLimit <= 0)
                    {
                        state.CURRENT_STATE = StateMachine.State.InstantKill;
                        isPhaseChanged = true;
                    }
                    break;
                case StateMachine.State.Outburst:
                    
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
    private void DestructionPart()
    {
        if (destructionCount > 0)
        {
            if (DestructionGauge <= 0)
            {
                if (!destructionStun)
                {
                    StartCoroutine(destructionCoroutine);
                }
            }
        }

        if (destructionStun)
        {
            if (state.CURRENT_STATE != StateMachine.State.Stun)
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
        health.doNotChange = true;
        currentDestructionHP = health.CurrentHP() - DestructionHP;
        GameObject poisonGas = Instantiate(poisonGasPrefab, transform.position, Quaternion.identity);
        poisonGas.GetComponent<PoisonGas>().radius = poisonGasRadius;
        poisonGas.GetComponent<PoisonGas>().damage = poisonGasDamage;
        poisonGas.GetComponent<PoisonGas>().duration = poisonGasDuration;

        yield return new WaitForSeconds(DestructionTime);
        health.doNotChange = false;
        destructionStun = false;
        destructionCoroutine = StartDestructTime();
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

    private IEnumerator Tailing(float angle)
    {
        var curAnim = GetComponentInChildren<SimpleSpineAnimator>();
        isTailing = true;
        float angleStep = angle / numberOfBullets;
        float currentAngle = -angle / 2 - 90;
        float angleIncrement = angleStep;

        while (state.CURRENT_STATE == StateMachine.State.Tailing || state.CURRENT_STATE == StateMachine.State.PhaseChange)
        {
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            direction.Normalize();

            GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

            currentAngle += angleIncrement;

            if (currentAngle > angle / 2 - 90)
            {
                angleIncrement = -angleStep;
            }
            else if (currentAngle < -angle / 2 - 90)
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

        if (mapObject == null)
        {
            mapObject = GameObject.FindWithTag("Ground");
        }

        if (mapObject != null)
        {
            var mapCollider = mapObject.GetComponent<Collider2D>();

            if (mapCollider != null)
            {
                Bounds mapBounds = mapCollider.bounds;

                float minX = mapBounds.min.x;
                float maxX = mapBounds.max.x;
                float minY = mapBounds.min.y;
                float maxY = mapBounds.max.y;

                if (mapCollider is BoxCollider2D)
                {
                    BoxCollider2D boxCollider = (BoxCollider2D)mapCollider;

                    minX = boxCollider.bounds.min.x;
                    maxX = boxCollider.bounds.max.x;
                    minY = boxCollider.bounds.min.y;
                    maxY = boxCollider.bounds.max.y;

                    for (int i = 0; i < numberOfBombs; i++)
                    {
                        Vector3 dropPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), zOffset);

                        GameObject bombPrefab = bombPrefabs[Random.Range(0, bombPrefabs.Length)];

                        Instantiate(bombPrefab, dropPosition, Quaternion.identity);

                        yield return new WaitForSeconds(dropDelay);
                    }
                }
                else if (mapCollider is CircleCollider2D)
                {
                    CircleCollider2D circleCollider = (CircleCollider2D)mapCollider;

                    Vector2 mapCenter = circleCollider.bounds.center;
                    float mapRadius = circleCollider.bounds.extents.x;
                    
                    minX = mapCenter.x - mapRadius;
                    maxX = mapCenter.x + mapRadius;
                    minY = mapCenter.y - mapRadius;
                    maxY = mapCenter.y + mapRadius;

                    for (int i = 0; i < numberOfBombs; i++)
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2);
                        float distance = Random.Range(0f, mapRadius);

                        float x = mapCenter.x + distance * Mathf.Cos(angle);
                        float y = mapCenter.y + distance * Mathf.Sin(angle);
                        Vector3 dropPosition = new Vector3(x, y, zOffset);

                        GameObject bombPrefab = bombPrefabs[Random.Range(0, bombPrefabs.Length)];

                        Instantiate(bombPrefab, dropPosition, Quaternion.identity);

                        yield return new WaitForSeconds(dropDelay);
                    }
                }
            }
            else
            {
                Debug.LogError("No Collider2D found on map object.");
            }
        }
        else
        {
            Debug.LogError("Map object not found.");
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

    private IEnumerator RunawayAndDropBombs()
    {
        isRunningAway = true;

        if (mapObject == null)
        {
            mapObject = GameObject.FindWithTag("Ground");
        }

        var mapCollider = mapObject.GetComponent<Collider2D>();

        if (mapCollider != null)
        {
            if (mapCollider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = (BoxCollider2D)mapCollider;
                Vector2 mapBoundsMin = boxCollider.bounds.min;
                Vector2 mapBoundsMax = boxCollider.bounds.max;

                runawayDestination = new Vector2(
                    Random.Range(mapBoundsMin.x, mapBoundsMax.x),
                    Random.Range(mapBoundsMin.y, mapBoundsMax.y)
                );
            }
            else if (mapCollider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = (CircleCollider2D)mapCollider;
                Vector2 mapCenter = circleCollider.bounds.center;
                float mapRadius = circleCollider.bounds.extents.x;

                Vector2 randomPoint = Random.insideUnitCircle.normalized * mapRadius;
                runawayDestination = mapCenter + randomPoint;
            }
        }
        else
        {
            Debug.LogWarning("mapObject does not have a Collider2D! Using current position as the runawayDestination.");
            runawayDestination = transform.position;
        }

        while (Vector3.Distance(transform.position, runawayDestination) > 1f)
        {
            if (state.CURRENT_STATE != StateMachine.State.Runaway)
            {
                isRunningAway = false;
                yield break;
            }

            Vector3 direction = (runawayDestination - transform.position).normalized;
            
            agent.Move(direction * runawaySpeed * Time.deltaTime);
            float xDirection = runawayDestination.x - transform.position.x;

            if (xDirection > 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (xDirection < 0)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            bombDropTimer += Time.deltaTime;
            if (bombDropTimer >= bombDropInterval && bombPrefab != null)
            {
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
                bombDropTimer = 0f;
            }
            yield return null;
        }

        var curAnim = GetComponentInChildren<SimpleSpineAnimator>();

        if (Vector3.Distance(transform.position, runawayDestination) <= 1f)
        {
            spineAnimation.AnimationState.SetAnimation(curAnim.AnimationTrack, curAnim.phase2Idle, loop: true);
            yield return new WaitForSeconds(waitAfterReachingDestination);
            spineAnimation.AnimationState.SetAnimation(curAnim.AnimationTrack, curAnim.Moving, loop: true);
        }

        isRunningAway = false;
    }

    private void CreateShield()
    {
        if (!isShieldActive)
        {
            currentShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            currentShield.transform.SetParent(this.transform);
            currentShield.transform.localScale = new Vector3(shieldRadius * 0.1f, shieldRadius * 0.1f, shieldRadius * 0.1f);
            isShieldActive = true;
            health.untouchable = true;
        }
    }

    public void RemoveShield()
    {
        if (isShieldActive)
        {
            Destroy(currentShield);
            isShieldActive = false;
            health.untouchable = false;
            shieldRegenTimer = shieldRegenTime;
        }
    }

    private IEnumerator KeepStateIdle(float time)
    {
        state.CURRENT_STATE = StateMachine.State.Idle;
        yield return new WaitForSeconds(time);
        isPatternPause = false;
    }

    private void DealDamageToPlayer(float damage)
    {
        playerHealth.Damaged(gameObject, transform.position, damage, Health.AttackType.Normal);
    }

    private IEnumerator OutburstAttack()
    {
        isRunningAway = true;

        Vector3 playerPosition = target.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        while (Vector3.Distance(transform.position, playerPosition) > 0.1f)
        {
            transform.position += directionToPlayer * runawaySpeed * Time.deltaTime;

            if (objectToSpawnDuringOutburst != null && isShieldActive)
                Instantiate(objectToSpawnDuringOutburst, transform.position, Quaternion.identity);

            if (Vector3.Distance(transform.position, playerPosition) < 1f)
            {
                DealDamageToPlayer(outburstDamage);
                break;
            }

            yield return null;
        }

        isRunningAway = false;
        state.CURRENT_STATE = StateMachine.State.Idle;
    }

    #endregion

    #region Event
    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Jump:
                if (!hasJumpAttacked && e.Data.Name == "bump")
                {
                    StartCoroutine(JumpAttack());
                }
                break;
            case StateMachine.State.Tailing:
                if (!isTailing && e.Data.Name == "effect_tail_whip")
                {
                    StartCoroutine(Tailing(maxAngle));
                }
                break;
            case StateMachine.State.Throwing:
                if (!isThrowing && e.Data.Name == "cream_throw")
                {
                    StartCoroutine(CreamThrow());
                }
                break;
            case StateMachine.State.Farting:
                if (!wasFarting && e.Data.Name == "effeck_fart")
                {
                    Fart();
                    wasFarting = true;
                }
                break;
            case StateMachine.State.Stun:
                if (e.Data.Name == "stop")
                {
                    spineAnimation.timeScale = 0.5f / Time.timeScale;
                }
                if (e.Data.Name == "move_start")
                {
                    spineAnimation.timeScale = 1f / Time.timeScale;
                }
                break;
            case StateMachine.State.InstantKill:
                if (e.Data.Name == "effect_explode")
                    playerHealth.Damaged(gameObject, transform.position, playerHealth.MaxHP() * 1.01f, Health.AttackType.Normal);
                break;
            case StateMachine.State.FartShield:
                if (e.Data.Name == "effeck_fart")
                {
                    if (!isShieldActive)
                    {
                        shieldRegenTimer -= Time.deltaTime;
                        if (shieldRegenTimer <= 0)
                        {
                            CreateShield();
                        }
                    }
                    else
                    {
                        patternManager.CurrentPattern = null;
                    }
                }
                break;
            case StateMachine.State.Outburst:
                Vector3 playerPosition = target.position;
                Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

                if (directionToPlayer.x > 0)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else if (directionToPlayer.x < 0)
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }

                if (e.Data.Name == "run")
                {
                    if (!isRunningAway)
                    {
                        StartCoroutine(OutburstAttack());
                    }
                }
                break;
        }
    }

    public float destructionGauge()
    {
        return this.DestructionGauge;
    }

    public void OnDie()
    {
        agent.speed = 0f;
        isPatternPause = true;
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
