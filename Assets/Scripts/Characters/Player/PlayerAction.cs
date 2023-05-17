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
    //흡수 딜레이
    public float SuctionDelay;
    //장전 딜레이
    public float LoadingDelay;

    public StateMachine _state;
    [SerializeField] private WeaponRadialMenu Radial;



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

    private float maxDuration = -1f;
    private float startMoveTimestamp;

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
        if (state.CURRENT_STATE != StateMachine.State.Dodging)
        {
            DodgeDelay -= Time.deltaTime;
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead && state.CURRENT_STATE != StateMachine.State.Pause)
        {
            DodgeRoll();
            ChangeAttack();

            Radial.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));

            Shot();
            Absorb();
            ShotDelay -= Time.deltaTime;
        }

        if (state.CURRENT_STATE == StateMachine.State.Loading)
        {
            LoadingDelay -= Time.deltaTime;
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
            Debug.Log("mouse down");
        }
        else if (context.canceled)
        {
            Debug.Log("mouse UP");
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
            _ = state.facingAngle;
            playerController.forceDir = ((playerController.xDir != 0f || playerController.yDir != 0f) ? Utils.GetAngle(Vector3.zero, new Vector3(playerController.xDir, playerController.yDir)) : state.facingAngle);
            playerController.speed = playerController.DodgeSpeed * 1.2f;
            state.CURRENT_STATE = StateMachine.State.Dodging;
            DodgeDelay = playerController.DodgeDelay;

            //기존 흡수나 공격 취소
            if (playerController.Attack[0].activeSelf && playerController.Attack[0] != null)
                playerController.Attack[0].SetActive(false);

            if (playerController.absorbEffet != null)
            {
                playerController.absorbEffet.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            PlayerAction.OnDodge?.Invoke();
            return true;
        }
        return false;
    }
    public bool ChangeAttack()
    {

        if (state.CURRENT_STATE == StateMachine.State.Dodging && state.CURRENT_STATE == StateMachine.State.Attacking)
            return false;
        else if (state.CURRENT_STATE == StateMachine.State.Loading && LoadingDelay < 0)
        {
            state.CURRENT_STATE = StateMachine.State.Idle;
            return false;
        }

        if (Input.GetMouseButtonDown(2))
        {
            Radial.Show();
        }
        else if (Input.GetMouseButtonUp(2))
        {
            int temp = Radial.Hide();
            if (temp != -1)
                playerController.CurAttack = temp;

            if (state.CURRENT_STATE != StateMachine.State.Loading && playerController.CurAttack != -1)
            {
                state.CURRENT_STATE = StateMachine.State.Loading;

                LoadingDelay = 0.6667f;

            }
        }
        return true;
    }
    public bool Shot()
    {
        if (state.CURRENT_STATE == StateMachine.State.Attacking && ShotDelay <= 0.15f && playerController.CurAttack != 0)
        {
            ShotDelay = playerController.AttackSpeed[playerController.CurAttack];
            state.CURRENT_STATE = StateMachine.State.Idle;
        }
        else if (state.CURRENT_STATE == StateMachine.State.Attacking
            && playerController.CurAttack == 0
            && !Input.GetMouseButton(0))
        {
            if (playerController.CurAttack == 0 && playerController.Attack[0] != null)
                playerController.Attack[0].SetActive(false);
            state.CURRENT_STATE = StateMachine.State.Idle;
        }
        else if (Input.GetMouseButton(0))
        {
            if (playerController.BulletGauge <= 0)
            {
                if (state.CURRENT_STATE == StateMachine.State.Attacking)
                    state.CURRENT_STATE = StateMachine.State.Idle;
                if (playerController.Attack[0].activeSelf && playerController.Attack[0] != null)
                    playerController.Attack[0].SetActive(false);
                return false;
            }

            if (ShotDelay > 0f ||
            (state.CURRENT_STATE == StateMachine.State.Dodging
            || state.CURRENT_STATE == StateMachine.State.Absorbing
            || state.CURRENT_STATE == StateMachine.State.Attacking))
            {
                return false;
            }

            getMouseInfo();
            if (Time.timeScale != 0)
                state.CURRENT_STATE = StateMachine.State.Attacking;

            ShotDelay = (playerController.CurAttack == 0) ? 0 : simpleSpineAnimator.PlayerAttack[playerController.CurAttack].Animation.Duration;

            if (playerController.CurAttack == 0 && playerController.Attack[0] != null)
                playerController.Attack[0].SetActive(true);


        }


        return true;
    }
    public bool Absorb()
    {
        if (Input.GetMouseButton(1) &&
            (state.CURRENT_STATE != StateMachine.State.Dodging && state.CURRENT_STATE != StateMachine.State.Attacking))
        {
            getMouseInfo();
            if (state.CURRENT_STATE != StateMachine.State.Absorbing)
                state.CURRENT_STATE = StateMachine.State.Absorbing;

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

            if (playerController.absorbEffet != null)
            {
                playerController.absorbEffet.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        return false;
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
        targetInRange = Physics2D.OverlapCircleAll(playerController.GrinderControl.position, playerController.SuctionRange, 1 << 20);

        for (int i = 0; i < targetInRange.Length; i++)
        {

            Vector2 dirToTarget = (targetInRange[i].transform.position - playerController.GrinderControl.position).normalized;
            if (Vector3.Angle(toMousedirection, dirToTarget) <= playerController.SuctionAngle / 2)
            {
                absorbObject absorb = targetInRange[i].gameObject.GetComponent<absorbObject>();
                if (absorb != null)
                {
                    absorb.inAbsorbArea = true;
                }
                //Debug.DrawLine(playerController.GrinderControl.position, targetInRange[i].transform.position, Color.green);
            }
        }
    }

    //    public void OnDrawGizmos()
    //    {
    //#if UNITY_EDITOR

    //        UnityEditor.Handles.DrawWireArc(playerController.GrinderControl.position, transform.forward, transform.right, 360, playerController.SuctionRange);

    //        Vector3 viewAngleA = DirFromAngle(-playerController.SuctionAngle / 2, false);
    //        Vector3 viewAngleB = DirFromAngle(playerController.SuctionAngle / 2, false);

    //        UnityEditor.Handles.DrawLine(playerController.GrinderControl.position, playerController.GrinderControl.position + viewAngleA * playerController.SuctionRange);
    //        UnityEditor.Handles.DrawLine(playerController.GrinderControl.position, playerController.GrinderControl.position + viewAngleB * playerController.SuctionRange);

    //#endif

    //    }

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
            Vector3 spawnPos = playerController.GrinderControl.position;
            spawnPos.z = 0;
            switch (playerController.CurAttack)
            {
                case 1:
                    float anglet = state.facingAngle - 15;
                    playerController.addBullet(-
                    Instantiate(playerController.Attack[playerController.CurAttack], spawnPos, Quaternion.Euler(new Vector3(0, 0, anglet))).GetComponent<PlayerAttack>().Cost);
                    for (int i = 0; i < 2; i++)
                    {
                        anglet += 30 / 2;
                        Instantiate(playerController.Attack[playerController.CurAttack], spawnPos, Quaternion.Euler(new Vector3(0, 0, anglet)));
                    }

                    break;
                case 2:
                    playerController.addBullet(-
                    Instantiate(playerController.Attack[playerController.CurAttack], spawnPos, Quaternion.Euler(new Vector3(0, 0, state.facingAngle))).GetComponent<PlayerAttack>().Cost);
                    rb.AddForce(Utils.GetMouseDirectionReverse(rb.position) * 2000f);
                    break;
                default:
                    break;
            }



        }
    }
}
