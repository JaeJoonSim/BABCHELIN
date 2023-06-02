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
    [Tooltip("회피 딜레이")]

    public float DodgeDelay;
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
        if (state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            if (state.CURRENT_STATE != StateMachine.State.Dodging)
            {
                DodgeDelay -= Time.deltaTime;
            }

            if (state.CURRENT_STATE == StateMachine.State.Skill2)
            {
                SkillDelay -= Time.deltaTime;
            }


            ShotDelay -= Time.deltaTime;

            DodgeRoll();
            Shot();
            Absorb();
            Skill();
        }

        PreviousPosition = base.transform.position;
    }

    public void move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            playerController.xDir = input.x;
            playerController.yDir = input.y;
        }
    }

    public void mouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("mouse down");
        }
        else if (context.canceled)
        {
            //Debug.Log("mouse UP");
        }
    }

    public void UltimateIdx(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("mouse down");
        }
        else if (context.canceled)
        {
            //Debug.Log("mouse UP");
        }
    }

    void getMouseInfo()
    {
        toMousedirection = Utils.GetMouseDirection(transform.position);
    }

    public bool DodgeRoll()
    {
        if (!AllowDodging)
        {
            return false;
        }

        StateMachine.State cURRENT_STATE = state.CURRENT_STATE;

        if (DodgeDelay <= 0f && Input.GetKey(KeyCode.LeftShift))
        {
            DodgeQueued = true;
        }

        if (state.CURRENT_STATE != StateMachine.State.Dodging && (DodgeQueued || (DodgeDelay <= 0f && Input.GetKey(KeyCode.LeftShift))))
        {
            DodgeQueued = false;
            state.CURRENT_STATE = StateMachine.State.Dodging;
            DodgeDelay = playerController.TotalStatus.dodgeCoolDown;
            playerController.dodgeSpeed = playerController.TotalStatus.dodgeDistance / playerController.TotalStatus.dodgeTime;
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
            playerController.PreesAttack = true;
            if (playerController.BulletGauge < playerController.Attack.GetComponent<PlayerAttack>().Cost || ShotDelay > 0)
            {
                return false;
            }
            state.CURRENT_STATE = StateMachine.State.Attacking;
            ShotDelay = 1 / (playerController.TotalStatus.atkSpd / 100f);
        }
        else if (Input.GetMouseButton(0) &&
            state.CURRENT_STATE == StateMachine.State.Attacking)
        {
            playerController.PreesAttack = true;
            if (ShotDelay <= 0 || simpleSpineAnimator.Track.IsComplete)
            {
                state.CURRENT_STATE = StateMachine.State.Idle;
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            playerController.PreesAttack = false;
            if (state.CURRENT_STATE == StateMachine.State.Attacking)
                if (simpleSpineAnimator.Track.IsComplete)
                    state.CURRENT_STATE = StateMachine.State.Idle;
        }

        return true;
    }
    public bool Absorb()
    {
        if (Input.GetMouseButton(1) &&
            (state.CURRENT_STATE == StateMachine.State.Idle || state.CURRENT_STATE == StateMachine.State.Moving))
        {
            
            if (state.CURRENT_STATE != StateMachine.State.Absorbing)
                state.CURRENT_STATE = StateMachine.State.Absorbing;

            
        }
        else if(Input.GetMouseButton(1) &&
            (state.CURRENT_STATE == StateMachine.State.Absorbing))
        {
            getMouseInfo();
            FindVisibleTargets();
        }
        else if (state.CURRENT_STATE == StateMachine.State.Absorbing && !Input.GetMouseButton(1))
        {
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (playerController.BulletGauge < playerController.Skills[0].GetComponent<PlayerAttack>().Cost || ShotDelay > 0)
                {
                    return false;
                }
                state.CURRENT_STATE = StateMachine.State.Skill;
                
                playerController.SkillIndex = 0;
            }           
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerController.BulletGauge < playerController.TotalStatus.sk1Cost || ShotDelay > 0)
                {
                    return false;
                }
                state.CURRENT_STATE = StateMachine.State.Skill2;
                playerController.addBullet(-playerController.TotalStatus.sk1Cost);
               playerController.SkillIndex = 1;
                SkillDelay = playerController.TotalStatus.sk1CoolDown;
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
        targetInRange = Physics2D.OverlapCircleAll(playerController.muzzleEnd.position, playerController.TotalStatus.absorbRange, 1 << 20);

        for (int i = 0; i < targetInRange.Length; i++)
        {

            Vector2 dirToTarget = (targetInRange[i].bounds.center - playerController.muzzleEnd.position).normalized;
            Debug.Log(Vector3.Angle(toMousedirection, dirToTarget));
            if (Vector3.Angle(toMousedirection, dirToTarget) <= playerController.TotalStatus.absorbAngle / 2)
            {
                absorbObject absorb = targetInRange[i].gameObject.GetComponent<absorbObject>();
                if (absorb != null)
                {
                    absorb.inAbsorbArea = true;
                }
                Debug.DrawLine(playerController.muzzleEnd.position, targetInRange[i].transform.position, Color.green);
            }
        }
    }

    public void OnDrawGizmos()
    {
#if UNITY_EDITOR

        UnityEditor.Handles.DrawWireArc(playerController.muzzleEnd.position, transform.forward, transform.right, 360, playerController.TotalStatus.absorbRange);

        Vector3 viewAngleA = DirFromAngle(-playerController.TotalStatus.absorbAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(playerController.TotalStatus.absorbAngle / 2, false);

        UnityEditor.Handles.DrawLine(playerController.muzzleEnd.position, playerController.muzzleEnd.position + viewAngleA * playerController.TotalStatus.absorbRange);
        UnityEditor.Handles.DrawLine(playerController.muzzleEnd.position, playerController.muzzleEnd.position + viewAngleB * playerController.TotalStatus.absorbRange);

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
        if (e.Data.Name == "shot")
        {
            Vector3 spawnPos = playerController.muzzleEnd.position;
            spawnPos.z = -0.1f;
            switch (state.CURRENT_STATE)
            {

                case StateMachine.State.Attacking:
                    if (trackEntry.TrackTime > 0.02)
                        return;


                    //float anglet = state.facingAngle - 15;
                    playerController.addBullet(-playerController.TotalStatus.bulletCost);
                    Instantiate(playerController.Attack, spawnPos, Quaternion.Euler(new Vector3(0, 0, state.facingAngle))).GetComponent<PlayerAttack>();
                    //for (int i = 0; i < 2; i++)
                    //{
                    //    anglet += 30 / 2;
                    //    Instantiate(playerController.Attack, spawnPos, Quaternion.Euler(new Vector3(0, 0, anglet)));
                    //}
                    break;
                case StateMachine.State.Skill:

                    if (trackEntry.TrackTime > 0.525)
                        return;
                    playerController.addBullet(-
                        Instantiate(playerController.Skills[playerController.SkillIndex], spawnPos, Quaternion.Euler(new Vector3(0, 0, state.facingAngle))).GetComponent<PlayerAttack>().Cost);
                    rb.AddForce(Utils.GetMouseDirectionReverse(rb.position) * 4000f);
                    break;
                case StateMachine.State.ultimate:
                    break;
                default:
                    break;
            }



        }
    }
}
