using Spine;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : BaseMonoBehaviour
{
    public delegate void PlayerEvent();
    public delegate void GoToEvent(Vector3 targetPosition);

    public static PlayerAction Instance;

    [HideInInspector] public Rigidbody2D rb;

    public GameObject CameraBone;
    private GameObject _CameraBone;
    private PlayerController _playerController;
    private UnitObject _unitObject;

    [Header("딜레이")]
    //공격 딜레이
    public float ShotDelay;
    //스킬 딜레이
    public float SkillDelay;


    public StateMachine _state;

    [Space, Header("Spine")]
    public SkeletonAnimation Spine;
    public SimpleSpineAnimator simpleSpineAnimator;

    [Space]
    public Material originalMaterial;
    public Material BW_Material;

    [HideInInspector] public CircleCollider2D circleCollider2D;
    private Skin PlayerSkin;

    [Space, Header("Debugging")]
    public bool GoToAndStopping;
    public bool IdleOnEnd;
    public GameObject LookToObject;
    private Action GoToCallback;

    public bool DodgeQueued;
    public bool AllowDodging = true;

    [Space]
    public bool HoldingAttack;

    //공격과 흡수에 사용할 변수
    public Vector3 toMousedirection;
    Collider2D[] targetInRange;


    public PlayerController playerController
    {
        get
        {
            if (_playerController == null)
            {
                _playerController = base.gameObject.GetComponent<PlayerController>();
            }
            return _playerController;
        }
    }

    public UnitObject unitObject
    {
        get
        {
            if (_unitObject == null)
            {
                _unitObject = base.gameObject.GetComponent<UnitObject>();
                //_unitObject.UseFixedDirectionalPathing = true;
            }
            return _unitObject;
        }
    }

    public StateMachine state
    {
        get
        {
            if (_state == null)
            {
                _state = base.gameObject.GetComponent<StateMachine>();
            }
            return _state;
        }
    }

    public Vector3 PreviousPosition { get; private set; }

    public static event PlayerEvent OnDodge;
    public static event PlayerEvent OnSuction;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        simpleSpineAnimator = GetComponentInChildren<SimpleSpineAnimator>();
        rb = base.gameObject.GetComponent<Rigidbody2D>();
        targetInRange = null;

        Spine.AnimationState.Event += OnSpineEvent;
    }

    private void Update()
    {

        if (Time.timeScale > 0f && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            if (state.CURRENT_STATE != StateMachine.State.Dodging)
            {
                playerController.DodgeDelay -= Time.deltaTime;
            }


            playerController.skill2CurCooltime -= Time.deltaTime;

            if (playerController.skill2count < playerController.TotalStatus.sk2Count.value)
            {
                if (playerController.skill2CurCooltime <= 0f)
                {
                    playerController.skill2CurCooltime = playerController.TotalStatus.sk2CoolDown.value;
                    if (playerController.skill2count < playerController.TotalStatus.sk2Count.value)
                        playerController.skill2count++;
                }
            }
            

            
            
            

            if (state.CURRENT_STATE == StateMachine.State.Skill2)
            {
                SkillDelay -= Time.deltaTime;
            }
            else
            {
                playerController.skill1CurCooltime -= Time.deltaTime;
            }


            ShotDelay -= Time.deltaTime;

            DodgeRoll();

            if (!playerController.UltObj.activeSelf)
            {
                Shot();
                Absorb();
                Skill();
            }
        }

        PreviousPosition = base.transform.position;
    }

    public void cheatMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.health.cheatMode = !playerController.health.cheatMode;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy.tag == "Enemy")
                {
                    enemy.GetComponent<Health>().Damaged(gameObject, transform.position, 99999, Health.AttackType.Normal);
                }
            }
        }
        else if (context.canceled)
        {
        }
    }

    public void move(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0f && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input != null)
            {
                playerController.xDir = input.x;
                playerController.yDir = input.y;
            }
        }
    }

    public void mouse(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0f && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            if (context.performed)
            {
                if (playerController.UltObj.activeSelf && state.CURRENT_STATE != StateMachine.State.Ultimate)
                {
                    if (playerController.UltObj.GetComponent<UltimateManager>().UltimateStart())
                    {
                        playerController.UltGauge -= 100;
                        if (UnityEngine.Random.value <= playerController.TotalStatus.ultTwice.value / 100f)
                        {
                            playerController.UltGauge += 100;
                        }
                        playerController.inSpineEvent = true;
                        playerController.Camerawork();
                        state.CURRENT_STATE = StateMachine.State.Ultimate;
                        playerController.UltEffet.Play(true);
                    }
                        
                    else
                        playerController.UltObj.SetActive(false);
                }
            }
            else if (context.canceled)
            {
                //Debug.Log("mouse UP");
            }
        }
    }
    public void mouseRight(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0f && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            if (context.performed)
            {
                if (playerController.UltObj.activeSelf && state.CURRENT_STATE != StateMachine.State.Ultimate)
                {
                    playerController.UltObj.SetActive(false);
                }
            }
            else if (context.canceled)
            {
                //Debug.Log("mouse UP");
            }
        }
    }

    public void UltimateIdx(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0f && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            if (context.performed)
            {
                //Debug.Log("mouse down");
                if (!playerController.UltObj.activeSelf)
                {
                    playerController.addUltIdx();
                }


            }
            else if (context.canceled)
            {
                //Debug.Log("mouse UP");
            }
        }
    }

    public void Ultimate(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0f && state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            if (context.performed)
            {
                if (playerController.UltGauge >= 100)
                {
                    playerController.UltObj.SetActive(true);
                }
                
            }
            else if (context.canceled)
            {

            }
        }
    }

    void getMouseInfo()
    {
        toMousedirection = Utils.GetMouseDirection(transform.position);
    }

    public bool DodgeRoll()
    {
        if (!AllowDodging || state.CURRENT_STATE == StateMachine.State.Ultimate)
        {
            return false;
        }

        StateMachine.State cURRENT_STATE = state.CURRENT_STATE;

        if (playerController.DodgeDelay <= 0f && Input.GetKey(KeyCode.LeftShift))
        {
            DodgeQueued = true;
            playerController.PreesAttack = false;
            //playerController.forceDir = Utils.GetAngle(Vector3.zero, new Vector3(playerController.xDir, playerController.yDir));
        }

        if (state.CURRENT_STATE != StateMachine.State.Dodging && (DodgeQueued || (playerController.DodgeDelay <= 0f && Input.GetKey(KeyCode.LeftShift))))
        {
            DodgeQueued = false;
            playerController.playerSound.StopSound();
            state.CURRENT_STATE = StateMachine.State.Dodging;
            playerController.playerSound.PlayPlayerSound(playerController.playerSound.pcRolling);
            playerController.DodgeDelay = playerController.TotalStatus.dodgeCoolDown.value;
            playerController.dodgeSpeed = playerController.TotalStatus.dodgeDistance.value / playerController.TotalStatus.dodgeTime.value;
            if (playerController.absorbEffet != null)
            {
                playerController.absorbEffet.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            PlayerAction.OnDodge?.Invoke();
            return true;
        }
        return false;
    }
    public bool Shot()
    {
        if (Input.GetMouseButton(0) &&
            (state.CURRENT_STATE == StateMachine.State.Idle || state.CURRENT_STATE == StateMachine.State.Moving))
        {
            if (playerController.BulletGauge < playerController.Attack.GetComponent<PlayerAttack>().Cost || ShotDelay > 0)
            {
                return false;
            }
            playerController.PreesAttack = true;
            playerController.inSpineEvent = true;  
            ShotDelay = 1 / (playerController.TotalStatus.atkSpd.value / 100f);
            playerController.facingAngle();
            state.CURRENT_STATE = StateMachine.State.Attacking;
        }
        else if (Input.GetMouseButton(0) && state.CURRENT_STATE == StateMachine.State.Attacking)
        {
            if (ShotDelay <= 0 || simpleSpineAnimator.Track.IsComplete)
            {
                state.CURRENT_STATE = StateMachine.State.Idle;
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
            {
                if (simpleSpineAnimator.Track.IsComplete)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
            }
            else if (state.CURRENT_STATE == StateMachine.State.Idle)
                playerController.PreesAttack = false;
        }

        return true;
    }
    public bool Absorb()
    {
        if (Input.GetMouseButton(1) &&
            (state.CURRENT_STATE == StateMachine.State.Idle || state.CURRENT_STATE == StateMachine.State.Moving))
        {

            if (state.CURRENT_STATE != StateMachine.State.Absorbing)
            {
                playerController.PreesAttack = true;
                state.CURRENT_STATE = StateMachine.State.Absorbing;
            }


        }
        else if (Input.GetMouseButton(1) &&
            (state.CURRENT_STATE == StateMachine.State.Absorbing))
        {
            getMouseInfo();
            FindVisibleTargets();
        }
        else if (state.CURRENT_STATE == StateMachine.State.Absorbing && !Input.GetMouseButton(1))
        {
            playerController.PreesAttack = false;
            playerController.playerSound.StopSound();
            state.CURRENT_STATE = StateMachine.State.Idle;
            if (targetInRange != null)
            {
                for (int i = 0; i < targetInRange.Length; i++)
                {
                    if (!targetInRange[i])
                        continue;
                    absorbObject absorb;
                    if (absorb = targetInRange[i].gameObject.GetComponent<absorbObject>())
                    {
                        if (absorb != null)
                        {
                            absorb.inAbsorbArea = false;
                        }
                    }
                }
            }
            targetInRange = null;
        }

        return false;
    }

    public bool Skill()
    {
        if ((state.CURRENT_STATE == StateMachine.State.Idle || state.CURRENT_STATE == StateMachine.State.Moving))
        {
            if (Input.GetKeyDown(KeyCode.Q) && playerController.skill2count > 0f)
            {
                if (playerController.BulletGauge < playerController.Skills[0].GetComponent<PlayerAttack>().Cost || ShotDelay > 0)
                {
                    return false;
                }
                state.facingAngle = Utils.GetMouseAngle(transform.position);
                playerController.inSpineEvent = true;

                state.CURRENT_STATE = StateMachine.State.Skill;
                playerController.SkillIndex = 0;
                playerController.skill2count--;
                
            }
            else if (Input.GetKeyDown(KeyCode.E) && playerController.skill1CurCooltime < 0f)
            {
                if (playerController.BulletGauge < playerController.TotalStatus.sk1Cost.value || ShotDelay > 0)
                {
                    return false;
                }
                state.CURRENT_STATE = StateMachine.State.Skill2;
                playerController.PreesAttack = true;
                playerController.addBullet(-playerController.TotalStatus.sk1Cost.value);
                playerController.SkillIndex = 1;
                SkillDelay = playerController.TotalStatus.sk1ResistTime.value;
                playerController.skill1CurCooltime = playerController.TotalStatus.sk1CoolDown.value;
            }
        }
        else
        {
            if (state.CURRENT_STATE == StateMachine.State.Skill)
            {
                if (playerController.SkillIndex == 0 && simpleSpineAnimator.Track.IsComplete)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
            }
            else if (state.CURRENT_STATE == StateMachine.State.Skill2 && SkillDelay <= 0)
            {
                playerController.PreesAttack = false;
                state.CURRENT_STATE = StateMachine.State.Idle;
            }
        }

        return true;
    }
    public void FindVisibleTargets()
    {
        if (targetInRange != null)
        {
            for (int i = 0; i < targetInRange.Length; i++)
            {
                if (!targetInRange[i])
                    continue;
                absorbObject absorb;
                if (absorb = targetInRange[i].gameObject.GetComponent<absorbObject>())
                {
                    if (absorb != null)
                    {
                        absorb.inAbsorbArea = false;
                    }
                }
            }
        }
        targetInRange = Physics2D.OverlapCircleAll(playerController.muzzleEnd.position, playerController.TotalStatus.absorbRange.value, 1 << 20);

        for (int i = 0; i < targetInRange.Length; i++)
        {

            Vector2 dirToTarget = (targetInRange[i].bounds.center - playerController.muzzleEnd.position).normalized;
            if (Vector3.Angle(toMousedirection, dirToTarget) <= playerController.TotalStatus.absorbAngle.value / 2)
            {
                absorbObject absorb = targetInRange[i].gameObject.GetComponent<absorbObject>();

                if (!Physics2D.Raycast(playerController.muzzleEnd.position, dirToTarget, playerController.TotalStatus.absorbRange.value, 1 << 22))
                {
                    if (absorb != null)
                    {
                        absorb.inAbsorbArea = true;
                    }
                    Debug.DrawLine(playerController.muzzleEnd.position, targetInRange[i].transform.position, Color.green);
                }


               
            }
        }
    }

    public void OnDrawGizmos()
    {
#if UNITY_EDITOR

        UnityEditor.Handles.DrawWireArc(playerController.muzzleEnd.position, transform.forward, transform.right, 360, playerController.TotalStatus.absorbRange.value);

        Vector3 viewAngleA = DirFromAngle(-playerController.TotalStatus.absorbAngle.value / 2, false);
        Vector3 viewAngleB = DirFromAngle(playerController.TotalStatus.absorbAngle.value / 2, false);

        UnityEditor.Handles.DrawLine(playerController.muzzleEnd.position, playerController.muzzleEnd.position + viewAngleA * playerController.TotalStatus.absorbRange.value);
        UnityEditor.Handles.DrawLine(playerController.muzzleEnd.position, playerController.muzzleEnd.position + viewAngleB * playerController.TotalStatus.absorbRange.value);

#endif

    }

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += state.facingAngle;
        }

        return new Vector3(Mathf.Cos((angleDegrees) * Mathf.Deg2Rad), Mathf.Sin((angleDegrees) * Mathf.Deg2Rad), 0);
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        
        //float val2 = Mathf.Round((trackEntry.AnimationTime) * Spine.skeleton.Data.Fps);
        //float val1 = Mathf.Round((e.Time * Spine.skeleton.Data.Fps));
        //Debug.Log(1 < Mathf.Abs(val1 - val2));
        //Debug.Log(val2);

        //Debug.Log("----------------------------");
        if (e.Data.Name == "shot")
        {
            //if (1 < Mathf.Abs(val1 - val2))
            //    return;

            if (!playerController.inSpineEvent)
                return;

            playerController.inSpineEvent = false;

            Vector3 spawnPos = playerController.muzzleEnd.position;
            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Attacking:
                    playerController.addBullet(-playerController.TotalStatus.bulletCost.value);
                    playerController.playerSound.PlayPlayerSound(playerController.playerSound.pcShoot);
                    if (playerController.TotalStatus.bulletCount.value <= 1)
                    {
                        Instantiate(playerController.Attack
                       , spawnPos
                       , Quaternion.Euler(new Vector3(0, 0, state.facingAngle))
                       ).GetComponent<PlayerAttack>().getStaus(playerController.TotalStatus);
                    }
                    else
                    {
                        int bCount = playerController.TotalStatus.bulletCount.value;
                        float anglet = state.facingAngle - (10 * (bCount - 1)) / 2;
                        for (int i = 0; i < bCount; i++)
                        {
                            Instantiate(playerController.Attack
                            , spawnPos
                            , Quaternion.Euler(new Vector3(0, 0, anglet))
                            ).GetComponent<PlayerAttack>().getStaus(playerController.TotalStatus);
                            anglet += 10;
                        }
                    }
                    break;
                case StateMachine.State.Skill:
                    playerController.addBullet(-playerController.TotalStatus.sk2Cost.value);
                    playerController.playerSound.PlayPlayerSound(playerController.playerSound.pcLargeSkill);
                    Instantiate(playerController.Skills[playerController.SkillIndex],
                        spawnPos,
                        Quaternion.Euler(new Vector3(0, 0, state.facingAngle))
                        ).GetComponent<PlayerAttack>().getStaus(playerController.TotalStatus);

                    rb.AddForce((rb.position - (Vector2)playerController.muzzleEnd.position).normalized * 4000f);
                    break;
                case StateMachine.State.Ultimate:
                    playerController.UltObj.GetComponent<UltimateManager>().UltimateShot();
                    //playerController.Camerawork();
                    break;
                default:
                    break;
            }



        }
    }
}
