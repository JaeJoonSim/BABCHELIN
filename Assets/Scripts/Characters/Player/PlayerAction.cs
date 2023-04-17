using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerAction : BaseMonoBehaviour
{
    public delegate void PlayerEvent();
    public delegate void GoToEvent(Vector3 targetPosition);

    public static PlayerAction Instance;

    public GameObject CameraBone;
    private GameObject _CameraBone;
    private PlayerController _playerController;
    private UnitObject _unitObject;

    [Header("������")]
    //ȸ�� ������
    public float DodgeDelay;
    //���� ������
    public float ShotDelay;
    //���� ������
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

    //���ݰ� ������ ����� ����
    public Vector3 toMousedirection;
    public float playerAngle = 0;
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
    }

    private void Update()
    {
        if (state.CURRENT_STATE != StateMachine.State.Dodging)
        {
            DodgeDelay -= Time.deltaTime;
        }

        if (state.CURRENT_STATE != StateMachine.State.Dead)
        {
            getMouseInfo();
            DodgeRoll();
            ChangeAttack();
            Shot();
            ShotDelay -= Time.deltaTime;
            Absorb();
        }


        PreviousPosition = base.transform.position;
    }

    void getMouseInfo()
    {
        playerAngle = Utils.GetMouseAngle(transform.position);
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
            playerController.CurAttack = (++playerController.CurAttack)%3;
            Debug.Log(playerController.CurAttack);
        }
        else if (wheelInput < 0)
        {
            playerController.CurAttack--;
            if (playerController.CurAttack < 0)
            {
                playerController.CurAttack = 2;
            }
            Debug.Log(playerController.CurAttack);
        }

        return true; 
    }
    public bool Shot()
    {
        if (state.CURRENT_STATE != StateMachine.State.Dodging && Input.GetMouseButton(0))
        {

            if (ShotDelay <= 0f)
            {
                Instantiate(playerController.Attack[playerController.CurAttack], transform.position, Quaternion.Euler(new Vector3(0, 0, playerAngle)));
                ShotDelay = playerController.AttackSpeed[playerController.CurAttack];
            }

        }

        return true;
    }
    public bool Absorb()
    {

        if (state.CURRENT_STATE != StateMachine.State.Dodging && Input.GetMouseButton(1))
        {
            FindVisibleTargets();
        }
        else
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
            targetInRange = null;
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
        targetInRange = Physics2D.OverlapCircleAll(transform.position, playerController.SuctionRange, 1 << 20);

        for (int i = 0; i < targetInRange.Length; i++)
        {

            Vector2 dirToTarget = (targetInRange[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(toMousedirection, dirToTarget) <= playerController.SuctionAngle / 2)
            {
                absorbObject absorb = targetInRange[i].gameObject.GetComponent<absorbObject>();
                if (absorb != null)
                {
                    absorb.inAbsorbArea = true;
                }
                Debug.DrawLine(transform.position, targetInRange[i].transform.position, Color.green);
            }
        }

    }

    public void OnDrawGizmos()
    {
#if UNITY_EDITOR

        UnityEditor.Handles.DrawWireArc(transform.position, transform.forward, transform.right, 360, playerController.SuctionRange);

        Vector3 viewAngleA = DirFromAngle(-playerController.SuctionAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(playerController.SuctionAngle / 2, false);

        UnityEditor.Handles.DrawLine(transform.position, transform.position + viewAngleA * playerController.SuctionRange);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + viewAngleB * playerController.SuctionRange);

#endif

    }

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += playerAngle;
        }

        return new Vector3(Mathf.Cos((angleDegrees) * Mathf.Deg2Rad), Mathf.Sin((angleDegrees) * Mathf.Deg2Rad), 0);
    }
}
