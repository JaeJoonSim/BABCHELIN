using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public float DodgeDelay;

    public StateMachine _state;
    public SkeletonAnimation Spine;
    public SimpleSpineAnimator simpleSpineAnimator;

    public Material originalMaterial;
    public Material BW_Material;

    [HideInInspector] public CircleCollider2D circleCollider2D;
    private Skin PlayerSkin;

    public bool GoToAndStopping;
    public bool IdleOnEnd;
    public GameObject LookToObject;
    private Action GoToCallback;
    
    private float maxDuration = -1f;
    private float startMoveTimestamp;

    public bool HoldingAttack;
    public bool DodgeQueued;
    public bool AllowDodging = true;

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        simpleSpineAnimator = GetComponentInChildren<SimpleSpineAnimator>();
    }

    private void Update()
    {
        if(state.CURRENT_STATE != StateMachine.State.Dodging)
        {
            DodgeDelay -= Time.deltaTime;
        }
        DodgeRoll();
        PreviousPosition = base.transform.position;
    }

    public bool DodgeRoll()
    {
        if (!AllowDodging || state.CURRENT_STATE == StateMachine.State.Dead)
        {
            return false;
        }
        StateMachine.State cURRENT_STATE = state.CURRENT_STATE;
        
        if(DodgeDelay <= 0f && Input.GetKey(KeyCode.LeftShift))
        {
            DodgeQueued = true;
        }

        if(state.CURRENT_STATE != StateMachine.State.Dodging && (DodgeQueued || (DodgeDelay <= 0f && Input.GetKey(KeyCode.LeftShift))))
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
}
