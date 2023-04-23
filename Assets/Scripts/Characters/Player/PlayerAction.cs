using Spine;
using Spine.Unity;
using System;
using UnityEngine;


public class PlayerAction : BaseMonoBehaviour
{
    public delegate void PlayerEvent();
    public delegate void GoToEvent(Vector3 targetPosition);

    public static PlayerAction Instance;

    public GameObject CameraBone;
    private GameObject _CameraBone;
    private PlayerController _playerController;
    private UnitObject _unitObject;

    [Header("µô·¹ÀÌ")]
    //È¸ÇÇ µô·¹ÀÌ
    public float DodgeDelay;
    //°ø°Ý µô·¹ÀÌ
    public float ShotDelay;
    //Èí¼ö µô·¹ÀÌ
    public float SuctionDelay;


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

    private float maxDuration = -1f;
    private float startMoveTimestamp;

    public bool DodgeQueued;
    public bool AllowDodging = true;

    [Space]
    public bool HoldingAttack;

    //°ø°Ý°ú Èí¼ö¿¡ »ç¿ëÇÒ º¯¼ö
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
        targetInRange = null;

        Spine.AnimationState.Event += OnSpineEvent;
    }

    private void Update()
    {
        if (state.CURRENT_STATE != StateMachine.State.Dodging)
        {
            DodgeDelay -= Time.deltaTime;
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            DodgeRoll();
            ChangeAttack();
            Shot();
            Absorb();

            ShotDelay -= Time.deltaTime;

        }
     
        PreviousPosition = base.transform.position;
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
            PlayerAction.OnDodge?.Invoke();
            return true;
        }
        return false;
    }
    public bool ChangeAttack()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0)
        {
            playerController.CurAttack = (++playerController.CurAttack) % 3;
        }
        else if (wheelInput < 0)
        {
            playerController.CurAttack--;
            if (playerController.CurAttack < 0)
            {
                playerController.CurAttack = 2;
            }
        }

        return true;
    }
    public bool Shot()
    {
        if (state.CURRENT_STATE != StateMachine.State.Dodging && Input.GetMouseButton(0))
        {
            getMouseInfo();
            if (playerController.BulletGauge < 20)
            {
                if (state.CURRENT_STATE == StateMachine.State.Attacking)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }

                return false;
            }
            if(state.CURRENT_STATE != StateMachine.State.Attacking && Time.timeScale != 0)
                state.CURRENT_STATE = StateMachine.State.Attacking;

                ShotDelay = playerController.AttackSpeed[playerController.CurAttack];

        }
        else if (state.CURRENT_STATE == StateMachine.State.Attacking && !Input.GetMouseButtonUp(0))
        {
            state.CURRENT_STATE = StateMachine.State.Idle;
        }



        return true;
    }
    public bool Absorb()
    {
        if (state.CURRENT_STATE != StateMachine.State.Dodging && Input.GetMouseButton(1))
        {
            getMouseInfo();
            if (state.CURRENT_STATE != StateMachine.State.Absorbing)
                state.CURRENT_STATE = StateMachine.State.Absorbing;


            FindVisibleTargets();
        }
        else if (state.CURRENT_STATE == StateMachine.State.Absorbing && Input.GetMouseButtonUp(1))
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
                Debug.DrawLine(playerController.GrinderControl.position, targetInRange[i].transform.position, Color.green);
            }
        }
    }

    public void OnDrawGizmos()
    {
#if UNITY_EDITOR

        UnityEditor.Handles.DrawWireArc(playerController.GrinderControl.position, transform.forward, transform.right, 360, playerController.SuctionRange);

        Vector3 viewAngleA = DirFromAngle(-playerController.SuctionAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(playerController.SuctionAngle / 2, false);

        UnityEditor.Handles.DrawLine(playerController.GrinderControl.position, playerController.GrinderControl.position + viewAngleA * playerController.SuctionRange);
        UnityEditor.Handles.DrawLine(playerController.GrinderControl.position, playerController.GrinderControl.position + viewAngleB * playerController.SuctionRange);

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
            playerController.addBullet(-10);
            float anglet = state.facingAngle - 15;
            Instantiate(playerController.Attack[playerController.CurAttack], transform.position, Quaternion.Euler(new Vector3(0, 0, anglet)));
            for (int i = 0; i < 2; i++)
            {
                anglet += 30 / 2;
                Instantiate(playerController.Attack[playerController.CurAttack], transform.position, Quaternion.Euler(new Vector3(0, 0, anglet)));
            }


        }
    }
}
