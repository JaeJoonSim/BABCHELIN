using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleSpineAnimator : BaseMonoBehaviour
{
    public enum direction3
    {
        up = 0,
        down = 1,
        Middle = 2
    }
    public direction3 DirectionState;

    public float curAnimTime;

    public delegate void SpineEvent(string EventName);

    [Serializable]
    public class SpineChartacterAnimationData
    {
        public StateMachine.State State;

        [HideInInspector]
        public AnimationReferenceAsset DefaultAnimation;

        public AnimationReferenceAsset Animation;
        public AnimationReferenceAsset AddAnimation;

        public bool DisableMixDuration;

        public bool Looping;

        public void InitDefaults()
        {
            DefaultAnimation = Animation;
        }
    }

    public int AnimationTrack = 0;
    public int SecondaryTrack = 1;
    private StateMachine state;
    private PlayerController playerController;
    private SkeletonAnimation _anim;
    private Skunk skunk;
    public bool AutomaticallySetFacing = true;
    public AnimationReferenceAsset DefaultLoop;
    public List<SpineChartacterAnimationData> Animations = new List<SpineChartacterAnimationData>();

    public bool North = false;
    public bool South = false;
    public bool LeftRight = false;

    [Header("Idle")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Idle;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthIdle;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthIdle;

    [Space, Header("Move")]

    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Moving;
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset MovingBack;

    [DrawIf("North", true)] public AnimationReferenceAsset NorthMoving;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthMovingBack;

    [DrawIf("South", true)] public AnimationReferenceAsset SouthMoving;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthMovingBack;
    [Space]
    public AnimationReferenceAsset StartMoving;
    public AnimationReferenceAsset StopMoving;

    [Space, Header("Attack")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Attack;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthAttack;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthAttack;

    [Space, Header("Absorb")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Absorb;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthAbsorb;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthAbsorb;

    [Space, Header("Skill")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Skill;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthSkill;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthSkill;

    [Space, Header("Skill2")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Skill2;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthSkill2;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthSkill2;

    [Space, Header("Loading")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset Loading;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthLoading;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthLoading;

    [Space, Header("Skunk Settings")]

    public AnimationReferenceAsset Crack;
    public AnimationReferenceAsset NonCrack;

    [Space]
    [Header("Phase1")]
    public AnimationReferenceAsset Jump;
    public AnimationReferenceAsset TailWhip;
    public AnimationReferenceAsset CreamThrow;
    public AnimationReferenceAsset fart;
    public AnimationReferenceAsset SpinStart;
    public AnimationReferenceAsset Destroyed;
    public AnimationReferenceAsset Spin;
    public AnimationReferenceAsset GimmickFailExplode;

    [Header("Phase2")]
    public AnimationReferenceAsset phase2Idle;
    public AnimationReferenceAsset FartShield;
    public AnimationReferenceAsset ChargeRun;

    [Space, Header("Other")]
    public AnimationReferenceAsset Dodge;
    public AnimationReferenceAsset Hit;
    public AnimationReferenceAsset Dead;

    [Space, Header("MixIdle")]
    [DrawIf("LeftRight", true)] public AnimationReferenceAsset MixIdle;
    [DrawIf("North", true)] public AnimationReferenceAsset NorthMixIdle;
    [DrawIf("South", true)] public AnimationReferenceAsset SouthMixIdle;



    private TrackEntry track;
    public TrackEntry Track
    {
        get { return track; }
        set
        {
            track = value;
            track.AttachmentThreshold = 1f;
            track.MixDuration = 0f;
            
        }
    }
    private TrackEntry secondTrack;
    public TrackEntry SecondTrack
    {
        get { return secondTrack; }
        set
        {
                secondTrack = value;
                SecondTrack.AttachmentThreshold = 1f;
                SecondTrack.MixDuration = 0f;
        }
    }
    private StateMachine.State cs;
    private TrackEntry t;

    public bool isFillWhite;

    public bool UpdateAnimsOnStateChange = true;

    private MaterialPropertyBlock BlockWhite;

    private MeshRenderer meshRenderer;

    private int fillAlpha;

    private int fillColor;

    private Color WarningColour = new Color(0.7490196f, 35f / 51f, 0.1372549f, 1f);

    private bool FlashingRed;

    private Coroutine cFlashFillRoutine;

    private float FlashRedMultiplier = 0.01f;

    private float xScaleSpeed;

    private float yScaleSpeed;

    private float moveSquish;

    private float xScale;

    private float yScale;

    private bool flipX;

    public bool ReverseFacing;

    public bool StartOnDefault = true;

    public int _Dir;

    public bool ForceDirectionalMovement;

    

    private SkeletonAnimation anim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponent<SkeletonAnimation>();
            }
            return _anim;
        }
    }

    public bool IsVisible
    {
        get
        {
            if (meshRenderer == null)
            {
                return true;
            }
            return meshRenderer.isVisible;
        }
    }

    private StateMachine.State CurrentState
    {
        get
        {
            return cs;
        }
        set
        {
            if (cs != value)
            {
                cs = value;
                if (playerController == null)
                {
                    UpdateAnimFromState();
                }

            }
        }
    }

    public int Dir
    {
        get
        {
            return _Dir;
        }
        set
        {
            if (_Dir != value)
            {
                //if (state != null && state.CURRENT_STATE == StateMachine.State.Moving && TurnAnimation != null)
                //{
                //    anim.AnimationState.SetAnimation(AnimationTrack, TurnAnimation, loop: true);
                //    anim.AnimationState.AddAnimation(AnimationTrack, Moving, loop: true, 0f);
                //}
                _Dir = value;
                if (playerController != null)
                {
                    if (_Dir < 0)
                        playerController.muzzleBone.position = transform.position + new Vector3(1, 0, 0);
                    else if (_Dir > 0)
                        playerController.muzzleBone.position = transform.position + new Vector3(-1, 0, 0);
                }

                anim.skeleton.ScaleX = Dir;
            }
        }
    }

    public event SpineEvent OnSpineEvent;

    public void Initialize(bool overwrite)
    {
        anim.Initialize(overwrite);
    }

    public SpineChartacterAnimationData GetAnimationData(StateMachine.State State)
    {
        foreach (SpineChartacterAnimationData animation in Animations)
        {
            if (animation.State == State)
            {
                return animation;
            }
        }
        return null;
    }

    private void UpdateAnimFromState()
    {
        if (anim == null || !UpdateAnimsOnStateChange)
        {
            return;
        }
        cs = state.CURRENT_STATE;
        if (Animations.Count > 0)
        {
            foreach (SpineChartacterAnimationData animation in Animations)
            {
                if (animation.State != cs)
                {
                    continue;
                }
                if (!(animation.Animation != null))
                {
                    return;
                }
                if (animation.State == StateMachine.State.Idle)
                {
                    if (StopMoving != null)
                    {
                        anim.AnimationState.SetAnimation(AnimationTrack, StopMoving, loop: false);
                        Track = anim.AnimationState.AddAnimation(AnimationTrack, animation.Animation, loop: true, 0f);
                    }
                    else
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, animation.Animation, loop: true);
                    }
                    return;
                }
                if (animation.State == StateMachine.State.Moving)
                {
                    if (StartMoving != null)
                    {
                        anim.AnimationState.SetAnimation(AnimationTrack, StartMoving, loop: false);
                        Track = anim.AnimationState.AddAnimation(AnimationTrack, animation.Animation, loop: true, 0f);
                    }
                    else
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, animation.Animation, animation.Looping);
                    }
                    return;
                }
                if (animation.AddAnimation == null)
                {
                    Track = anim.AnimationState.SetAnimation(AnimationTrack, animation.Animation, animation.Looping);
                    if (animation.DisableMixDuration)
                    {
                        Track.MixDuration = 0f;
                    }
                    return;
                }
                Track = anim.AnimationState.SetAnimation(AnimationTrack, animation.Animation, loop: false);
                if (animation.DisableMixDuration)
                {
                    Track.MixDuration = 0f;
                }
                anim.AnimationState.AddAnimation(AnimationTrack, animation.AddAnimation, animation.Looping, 0f);
                return;
            }
            Track = anim.AnimationState.SetAnimation(AnimationTrack, DefaultLoop, loop: true);
            return;
        }
        switch (cs)
        {
            case StateMachine.State.Idle:
                if (skunk != null)
                {
                    SetCrackAnim();
                    if (skunk.currentPhase == 1)
                        anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                    else
                        anim.AnimationState.SetAnimation(AnimationTrack, phase2Idle, loop: true);
                }
                else
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                }
                break;
            case StateMachine.State.Moving:
                if (StartMoving != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, StartMoving, loop: true);
                    anim.AnimationState.AddAnimation(AnimationTrack, Moving, loop: true, 0.2f);
                }
                else
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                }
                break;
            case StateMachine.State.Dodging:
                anim.AnimationState.SetAnimation(AnimationTrack, Dodge, loop: true);
                break;
            case StateMachine.State.Attacking:
                if (Attack != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Attack, loop: true);
                }
                break;
            case StateMachine.State.Absorbing:
                if (Absorb != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Absorb, loop: true);
                }
                break;
            case StateMachine.State.HitLeft:
            case StateMachine.State.HitRight:
                if (Hit != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Hit, loop: false);
                }
                break;
            case StateMachine.State.Patrol:
                if (StartMoving != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, StartMoving, loop: true);
                    anim.AnimationState.AddAnimation(AnimationTrack, Moving, loop: true, 0.2f);
                }
                else
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                }
                break;
            case StateMachine.State.Runaway:
                if (StartMoving != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, StartMoving, loop: true);
                    anim.AnimationState.AddAnimation(AnimationTrack, Moving, loop: true, 0.2f);
                }
                else
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                }
                break;

            case StateMachine.State.Jump:
                if (Jump != null)
                {
                    SetCrackAnim();
                    anim.AnimationState.SetAnimation(AnimationTrack, Jump, loop: false);
                }
                break;
            case StateMachine.State.Tailing:
                if (TailWhip != null)
                {
                    SetCrackAnim();
                    anim.AnimationState.SetAnimation(AnimationTrack, TailWhip, loop: false);
                    anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, TailWhip.Animation.Duration);
                }
                break;
            case StateMachine.State.Throwing:
                if (CreamThrow != null)
                {
                    SetCrackAnim();
                    anim.AnimationState.SetAnimation(AnimationTrack, CreamThrow, loop: false);
                    anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, CreamThrow.Animation.Duration);
                }
                break;
            case StateMachine.State.Farting:
                if (fart != null)
                {
                    SetCrackAnim();
                    anim.AnimationState.SetAnimation(AnimationTrack, fart, loop: false);
                    anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, fart.Animation.Duration);
                }
                break;
            case StateMachine.State.Stun:
                if(Destroyed != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Destroyed, loop: false);
                    if (skunk.destructionCount <= 1)
                        anim.AnimationState.SetAnimation(SecondaryTrack, Crack, loop: true);
                    anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, fart.Animation.Duration);
                }
                break;
            case StateMachine.State.InstantKill:
                if (GimmickFailExplode != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, GimmickFailExplode, loop: false);
                }
                break;
            case StateMachine.State.PhaseChange:
                if (SpinStart != null)
                {
                    SetCrackAnim();
                    anim.AnimationState.SetAnimation(AnimationTrack, SpinStart, loop: false);
                    if (Spin != null)
                    {
                        anim.AnimationState.AddAnimation(AnimationTrack, Spin, loop: true, SpinStart.Animation.Duration);
                    }
                    else
                    {
                        anim.AnimationState.AddAnimation(AnimationTrack, Idle, loop: true, SpinStart.Animation.Duration);
                    }
                }
                break;
            case StateMachine.State.FartShield:
                if (FartShield != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, FartShield, loop: false);
                    anim.AnimationState.AddAnimation(AnimationTrack, phase2Idle, loop: true, FartShield.Animation.Duration);
                }
                break;
            case StateMachine.State.Outburst:
                if(ChargeRun != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, ChargeRun, loop: false);
                    anim.AnimationState.AddAnimation(AnimationTrack, phase2Idle, loop: true, ChargeRun.Animation.Duration);
                }
                break;

            case StateMachine.State.Dead:
                if (Dead != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Dead, loop: false);
                }
                break;
            case StateMachine.State.Pause:
                anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                break;
            default:
                if (Idle != null && anim.AnimationState != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                }
                break;
        }

    }

    private void UpdateAnimFromFacing()
    {
        if (SecondTrack != null)
        {
            if (cs == StateMachine.State.Attacking || 
                cs == StateMachine.State.Absorbing || 
                cs == StateMachine.State.Loading ||
                cs == StateMachine.State.Skill2 
                )
            {
                //공격중 이동 x
                if (playerController.xDir == 0 && playerController.yDir == 0)
                {
                    if (DirectionState == direction3.up && NorthMixIdle != null)
                    {
                        if (SecondTrack.Animation != NorthMixIdle.Animation)
                        {
                            SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, NorthMixIdle, loop: false);
                        }
                    }
                    else if (DirectionState == direction3.down && SouthMixIdle != null)
                    {
                        if (SecondTrack.Animation != SouthMixIdle.Animation)
                        {
                            SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, SouthMixIdle, loop: false);
                        }
                    }
                    else
                    {
                        if (SecondTrack.Animation != MixIdle.Animation)
                        {
                            SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, MixIdle, loop: false);
                        }
                    }
                }
                else
                {
                    if (DirectionState == direction3.up )
                    {
                        if (playerController.yDir > 0 && NorthMoving != null)
                        {
                            if (SecondTrack.Animation != NorthMoving.Animation)
                            {
                                SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, NorthMoving, loop: true);
                            }
                        }
                        else if (playerController.yDir < 0 && NorthMovingBack != null)
                        {
                            if (SecondTrack.Animation != NorthMovingBack.Animation)
                            {
                                SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, NorthMovingBack, loop: true);
                            }
                        }
                        else
                        {
                            if (SecondTrack.Animation != NorthMoving.Animation)
                            {
                                SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, NorthMoving, loop: true);
                            }
                        }
                    }
                    else if (DirectionState == direction3.down)
                    {
                        if (playerController.yDir < 0 && SouthMoving != null)
                        {
                            if (SecondTrack.Animation != SouthMoving.Animation)
                            {
                                SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, SouthMoving, loop: true);
                            }
                        }
                        else if (playerController.yDir > 0 && SouthMovingBack != null)
                        {
                            if (SecondTrack.Animation != SouthMovingBack.Animation)
                            {
                                SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, SouthMovingBack, loop: true);
                            }
                        }
                        else
                        {
                            if (SecondTrack.Animation != SouthMoving.Animation)
                            {
                                SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, SouthMoving, loop: true);
                            }
                        }
                    }
                    else if (DirectionState == direction3.Middle)
                    {
                        if (Dir == -1)
                        {
                            if (playerController.xDir > 0)
                            {
                                if (SecondTrack.Animation != MovingBack.Animation)
                                {
                                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, MovingBack, loop: true);
                                }
                            }
                            else if (playerController.xDir <= 0)
                            {
                                if (SecondTrack.Animation != Moving.Animation)
                                {
                                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, Moving, loop: true);
                                }
                            }
                            else
                            {
                                if (SecondTrack.Animation != Moving.Animation)
                                {
                                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, Moving, loop: true);
                                }
                            }

                        }
                        else if (Dir == 1)
                        {
                            if (playerController.xDir < 0)
                            {
                                if (SecondTrack.Animation != MovingBack.Animation)
                                {
                                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, MovingBack, loop: true);
                                }
                            }
                            else if (playerController.xDir >= 0)
                            {
                                if (SecondTrack.Animation != Moving.Animation)
                                {
                                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, Moving, loop: true);
                                }
                            }
                            else
                            {
                                if (SecondTrack.Animation != Moving.Animation)
                                {
                                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, Moving, loop: true);
                                }
                            }
                        }
                        else if (MovingBack == null)
                        {
                            SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, Moving, loop: true);
                        }
                    }

                }
            }
            else
            {
                if (anim.AnimationState.Tracks.Count - 1 == SecondaryTrack)
                {
                    anim.AnimationState.ClearTrack(SecondaryTrack);
                }
            }
        }

        if (Track != null)
        {
            curAnimTime = 0;

            switch (state.CURRENT_STATE)
            {
                case StateMachine.State.Idle:

                    if (DirectionState == direction3.up && NorthIdle != null)
                    {
                        if (Track.Animation != NorthIdle.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthIdle, loop: true);
                        }
                    }
                    else if (DirectionState == direction3.down && SouthIdle != null)
                    {
                        if (Track.Animation != SouthIdle.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthIdle, loop: true);
                        }
                    }
                    else
                    {
                        if (Track.Animation != Idle.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                        }
                    }

                    break;
                case StateMachine.State.Moving:

                    if (DirectionState == direction3.up && NorthMoving != null)
                    {
                        if (Track.Animation != NorthMoving.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthMoving, loop: true);
                        }
                    }
                    else if (DirectionState == direction3.down && SouthMoving != null)
                    {
                        if (Track.Animation != SouthMoving.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthMoving, loop: true);
                        }
                    }
                    else if (playerController != null)
                    {
                        if (Dir == -1)
                        {
                            if (playerController.xDir > 0)
                            {
                                if (Track.Animation != MovingBack.Animation)
                                {
                                    Track = anim.AnimationState.SetAnimation(AnimationTrack, MovingBack, loop: true);
                                }
                            }
                            else if (playerController.xDir <= 0)
                            {
                                if (Track.Animation != Moving.Animation)
                                {
                                    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                                }
                            }
                            else
                            {
                                if (Track.Animation != Moving.Animation)
                                {
                                    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                                }
                            }

                        }
                        else if (Dir == 1)
                        {
                            if (playerController.xDir < 0)
                            {
                                if (Track.Animation != MovingBack.Animation)
                                {
                                    Track = anim.AnimationState.SetAnimation(AnimationTrack, MovingBack, loop: true);
                                }
                            }
                            else if (playerController.xDir >= 0)
                            {
                                if (Track.Animation != Moving.Animation)
                                {
                                    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                                }
                            }
                            else
                            {
                                if (Track.Animation != Moving.Animation)
                                {
                                    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                                }
                            }
                        }
                        else if (MovingBack == null)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                        }
                    }

                    break;
                case StateMachine.State.Attacking:
                    
                    if (DirectionState == direction3.up && NorthAttack != null)
                    {
                        if (Track.Animation != NorthAttack.Animation)
                        {
                            if (Track.Animation == Attack.Animation || Track.Animation == NorthAttack.Animation || Track.Animation == SouthAttack.Animation)
                                curAnimTime = Track.TrackTime % Track.Animation.Duration;
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthAttack, loop: false);
                            Track.TrackTime = curAnimTime;
                        }
                    }
                    else if (DirectionState == direction3.down && SouthAttack != null)
                    {
                        if (Track.Animation != SouthAttack.Animation)
                        {
                            if (Track.Animation == Attack.Animation || Track.Animation == NorthAttack.Animation || Track.Animation == SouthAttack.Animation)
                                curAnimTime = Track.TrackTime % Track.Animation.Duration;
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthAttack, loop: false);
                            Track.TrackTime = curAnimTime;
                        }
                    }
                    else
                    {
                        if (Track.Animation != Attack.Animation)
                        {
                            if (Track.Animation == Attack.Animation || Track.Animation == NorthAttack.Animation || Track.Animation == SouthAttack.Animation)
                                curAnimTime = Track.TrackTime % Track.Animation.Duration;
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, Attack, loop: false);
                            Track.TrackTime = curAnimTime;
                        }
                    }

                    break;
                case StateMachine.State.Skill:
                    if (DirectionState == direction3.up && NorthSkill != null)
                    {
                        if (Track.Animation != NorthSkill.Animation)
                        {
                            if (Track.Animation == Skill.Animation || Track.Animation == NorthSkill.Animation || Track.Animation == SouthSkill.Animation)
                                curAnimTime = Track.TrackTime % Track.Animation.Duration;
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthSkill, loop: false);
                            Track.TrackTime = curAnimTime;
                        }
                    }
                    else if (DirectionState == direction3.down && SouthSkill != null)
                    {
                        if (Track.Animation != SouthSkill.Animation)
                        {
                            if (Track.Animation == Skill.Animation || Track.Animation == NorthSkill.Animation || Track.Animation == SouthSkill.Animation)
                                curAnimTime = Track.TrackTime % Track.Animation.Duration;
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthSkill, loop: false);
                            Track.TrackTime = curAnimTime;
                        }
                    }
                    else
                    {
                        if (Track.Animation != Skill.Animation)
                        {
                            if (Track.Animation == Skill.Animation || Track.Animation == NorthSkill.Animation || Track.Animation == SouthSkill.Animation)
                                curAnimTime = Track.TrackTime % Track.Animation.Duration;
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, Skill, loop: false);
                            Track.TrackTime = curAnimTime;
                        }
                    }
                    break;
                case StateMachine.State.Skill2:
                    if (DirectionState == direction3.up && NorthSkill2 != null)
                    {
                        if (Track.Animation != NorthSkill2.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthSkill2, loop: false);
                        }
                    }
                    else if (DirectionState == direction3.down && SouthSkill2 != null)
                    {
                        if (Track.Animation != SouthSkill2.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthSkill2, loop: false);
                        }
                    }
                    else
                    {
                        if (Track.Animation != Skill2.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, Skill2, loop: false);
                        }
                    }
                    break;
                case StateMachine.State.Loading:

                    if (DirectionState == direction3.up && NorthLoading != null)
                    {
                        if (Track.Animation != NorthLoading.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthLoading, loop: true);
                        }
                    }
                    else if (DirectionState == direction3.down && SouthLoading != null)
                    {
                        if (Track.Animation != SouthLoading.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthLoading, loop: true);
                        }
                    }
                    else
                        if (Track.Animation != Loading.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Loading, loop: true);
                    }


                    break;
                case StateMachine.State.Dodging:
                    if (Track.Animation != Dodge.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Dodge, loop: true);
                    }
                    break;
                case StateMachine.State.CustomAnimation:
                    break;
                case StateMachine.State.Absorbing:
                    if (DirectionState == direction3.up && NorthAbsorb != null)
                    {
                        if (Track.Animation != NorthAbsorb.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthAbsorb, loop: true);
                        }
                    }
                    else if (DirectionState == direction3.down && SouthAbsorb != null)
                    {
                        if (Track.Animation != SouthAbsorb.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthAbsorb, loop: true);
                        }
                    }
                    else
                    {
                        if (Track.Animation != Absorb.Animation)
                        {
                            Track = anim.AnimationState.SetAnimation(AnimationTrack, Absorb, loop: true);
                        }
                    }
                    break;
                case StateMachine.State.HitLeft:
                case StateMachine.State.HitRight:
                    if (Hit != null && Track.Animation != Hit.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Hit, loop: false);
                    }
                    break;
                case StateMachine.State.Dead:
                    if (Dead != null && Track.Animation != Dead.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Dead, loop: false);
                    }
                    break;
                case StateMachine.State.Pause:
                    if (Idle != null && Track.Animation != Idle.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                    }
                   
                    break;
                default:
                    if (Idle != null && Track.Animation != Idle.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                    }
                    break;
            }


        }


    }

    public string CurrentAnimation()
    {
        return anim.AnimationName;
    }

    public TrackEntry Animate(string Animation, int Track, bool Loop)
    {
        if (anim.AnimationState.Data.SkeletonData.FindAnimation(Animation) != null)
        {
            return anim.AnimationState.SetAnimation(Track, Animation, Loop);
        }
        return null;
    }

    public TrackEntry Animate(string Animation, int Track, bool Loop, float MixTime)
    {
        if (anim.AnimationState.Data.SkeletonData.FindAnimation(Animation) != null)
        {
            t = anim.AnimationState.SetAnimation(Track, Animation, Loop);
            t.MixTime = MixTime;
            return t;
        }
        return null;
    }

    public void AddAnimate(string Animation, int Track, bool Loop, float Delay)
    {
        anim.AnimationState.AddAnimation(Track, Animation, Loop, Delay);
    }

    public void ClearTrackAfterAnimation(int Track)
    {
        anim.AnimationState.AddEmptyAnimation(Track, 0.1f, 0f);
    }

    public void SetAttachment(string slotName, string attachmentName)
    {
        anim.skeleton.SetAttachment(slotName, attachmentName);
    }

    public void SetSkin(string Skin)
    {
        anim.skeleton.SetSkin(Skin);
        anim.skeleton.SetSlotsToSetupPose();
    }

    public void SetColor(Color color)
    {
        anim.skeleton.SetColor(color);
    }

    public void FlashMeWhite()
    {
        if (Time.frameCount % 5 == 0)
        {
            FlashWhite(!isFillWhite);
        }
    }

    public void FlashWhite(bool toggle)
    {
        if (BlockWhite == null)
        {
            BlockWhite = new MaterialPropertyBlock();
        }
        BlockWhite.SetColor(fillColor, WarningColour);
        BlockWhite.SetFloat(fillAlpha, toggle ? 0.33f : 0f);
        meshRenderer.SetPropertyBlock(BlockWhite);
        isFillWhite = toggle;
    }

    public void FillWhite(bool toggle)
    {
        if (!(meshRenderer == null))
        {
            if (BlockWhite == null)
            {
                BlockWhite = new MaterialPropertyBlock();
            }
            BlockWhite.SetColor(fillColor, Color.white);
            BlockWhite.SetFloat(fillAlpha, toggle ? 1f : 0f);
            meshRenderer.SetPropertyBlock(BlockWhite);
            isFillWhite = toggle;
        }
    }

    public void FillColor(Color color, float Alpha = 1f)
    {
        if (!(meshRenderer == null))
        {
            if (BlockWhite == null)
            {
                BlockWhite = new MaterialPropertyBlock();
            }
            BlockWhite.SetColor(fillColor, color);
            BlockWhite.SetFloat(fillAlpha, Alpha);
            meshRenderer.SetPropertyBlock(BlockWhite);
        }
    }

    public void FlashFillRed(float opacity = 0.5f)
    {
        FlashingRed = true;
        if (cFlashFillRoutine != null)
        {
            StopCoroutine(cFlashFillRoutine);
        }
        cFlashFillRoutine = StartCoroutine(FlashOnHitRoutine(opacity));
    }

    private IEnumerator FlashOnHitRoutine(float opacity)
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.receiveShadows = false;
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.SetPropertyBlock(propertyBlock);
        SetColor(new Color(1f, 1f, 1f, opacity));
        yield return new WaitForSeconds(6f * FlashRedMultiplier);
        SetColor(new Color(0f, 0f, 0f, opacity));
        yield return new WaitForSeconds(3f * FlashRedMultiplier);
        SetColor(new Color(1f, 0f, 0f, opacity));
        yield return new WaitForSeconds(2f * FlashRedMultiplier);
        SetColor(new Color(0f, 0f, 0f, opacity));
        yield return new WaitForSeconds(2f * FlashRedMultiplier);
        SetColor(new Color(1f, 0f, 0f, opacity));
        yield return new WaitForSeconds(2f * FlashRedMultiplier);
        SetColor(new Color(1f, 1f, 1f, 1f));
        FlashingRed = false;
        meshRenderer.receiveShadows = true;
        meshRenderer.shadowCastingMode = ShadowCastingMode.On;
    }

    private IEnumerator DoFlashFillRed()
    {
        if (meshRenderer == null)
        {
            yield break;
        }
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.SetPropertyBlock(block);
        block.SetColor(fillColor, Color.red);
        block.SetFloat(fillAlpha, 1f);
        meshRenderer.SetPropertyBlock(block);
        yield return new WaitForSeconds(0.1f);
        float Progress = 1f;
        while (true)
        {
            float num;
            Progress = (num = Progress - 0.05f);
            if (num >= 0f)
            {
                if (Progress <= 0f)
                {
                    Progress = 0f;
                }
                block.SetFloat(fillAlpha, Progress);
                meshRenderer.SetPropertyBlock(block);
                yield return null;
                continue;
            }
            break;
        }
    }

    public void FlashRedTint()
    {
        StopCoroutine(DoFlashTintRed());
        StartCoroutine(DoFlashTintRed());
    }

    private IEnumerator DoFlashTintRed()
    {
        float Progress = 0f;
        while (true)
        {
            float num;
            Progress = (num = Progress + 0.05f);
            if (!(num <= 1f))
            {
                break;
            }
            SetColor(Color.Lerp(Color.red, Color.white, Progress));
            yield return null;
        }
        SetColor(Color.white);
    }

    public void FlashFillBlack()
    {
        StopCoroutine(DoFlashFillBlack());
        StartCoroutine(DoFlashFillBlack());
    }

    private IEnumerator DoFlashFillBlack()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.SetPropertyBlock(block);
        SetColor(Color.black);
        yield return new WaitForSeconds(0.1f);
        SetColor(Color.white);
        float Progress = 1f;
        while (Progress > 0f)
        {
            Progress -= Time.deltaTime;
            block.SetFloat(fillAlpha, Progress);
            meshRenderer.SetPropertyBlock(block);
            yield return null;
        }
    }

    public void FlashFillGreen()
    {
        StopCoroutine(DoFlashFillGreen());
        StartCoroutine(DoFlashFillGreen());
    }

    private IEnumerator DoFlashFillGreen()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.SetPropertyBlock(block);
        SetColor(Color.green);
        yield return new WaitForSeconds(0.1f);
        SetColor(Color.white);
        float Progress = 1f;
        while (Progress > 0f)
        {
            Progress -= Time.deltaTime;
            block.SetFloat(fillAlpha, Progress);
            meshRenderer.SetPropertyBlock(block);
            yield return null;
        }
    }

    public void ChangeStateAnimation(StateMachine.State state, string NewAnimation)
    {
        SpineChartacterAnimationData animationData = GetAnimationData(state);
        AnimationReferenceAsset animationReferenceAsset = ScriptableObject.CreateInstance<AnimationReferenceAsset>();
        animationReferenceAsset.Animation = anim.skeleton.Data.FindAnimation(NewAnimation);
        animationReferenceAsset.name = NewAnimation;
        animationData.Animation = animationReferenceAsset;
        if (CurrentState == state && this.state.CURRENT_STATE == state)
        {
            UpdateAnimFromState();
        }
    }

    public AnimationReferenceAsset GetAnimationReference(string AnimationName)
    {
        return new AnimationReferenceAsset
        {
            Animation = anim.skeleton.Data.FindAnimation(AnimationName),
            name = AnimationName
        };
    }

    public void ResetAnimationsToDefaults()
    {
        foreach (SpineChartacterAnimationData animation in Animations)
        {
            animation.Animation = animation.DefaultAnimation;
        }
        UpdateAnimFromState();
    }

    public float Duration()
    {
        return anim.AnimationState.GetCurrent(AnimationTrack).Animation.Duration;
    }

    private void Awake()
    {
        state = GetComponentInParent<StateMachine>();
        playerController = GetComponentInParent<PlayerController>();
        skunk = GetComponentInParent<Skunk>();
        UpdateIdleAndMoving();
        if (StartOnDefault && anim != null)
        {
            if (DefaultLoop == null && Idle == null)
            {
                return;
            }
            if (anim.AnimationState.Data.SkeletonData.FindAnimation((DefaultLoop != null) ? DefaultLoop.Animation.Name : Idle.Animation.Name) != null)
            {
                Track = anim.AnimationState.SetAnimation(AnimationTrack, (DefaultLoop != null) ? DefaultLoop : Idle, loop: true);
                if (playerController != null)
                    SecondTrack = anim.AnimationState.SetAnimation(SecondaryTrack, (DefaultLoop != null) ? DefaultLoop : Idle, loop: true);
            }
        }
        else
        {
            Track = anim.AnimationState.GetCurrent(0);
            if (playerController != null)
                SecondTrack = anim.AnimationState.GetCurrent(0);
        }
        meshRenderer = GetComponent<MeshRenderer>();
        fillColor = Shader.PropertyToID("_FillColor");
        fillAlpha = Shader.PropertyToID("_FillAlpha");
    }

    private void OnEnable()
    {
        anim.AnimationState.Event += SpineEventHandler;
    }

    private void OnDisable()
    {
        anim.AnimationState.Event -= SpineEventHandler;
    }

    public void SetDefault(StateMachine.State State, string Animation)
    {
        foreach (SpineChartacterAnimationData animation in Animations)
        {
            if (animation.State == State)
            {
                animation.Animation = GetAnimationReference(Animation);
                animation.DefaultAnimation = GetAnimationReference(Animation);
            }
        }
    }

    public void UpdateIdleAndMoving()
    {
        foreach (SpineChartacterAnimationData animation in Animations)
        {
            animation.InitDefaults();
            if (animation.State == StateMachine.State.Moving)
            {
                Moving = animation.Animation;
            }
            if (animation.State == StateMachine.State.Idle)
            {
                Idle = animation.Animation;
            }
        }
    }

    private void OnDestroy()
    {
        anim.AnimationState.Event -= SpineEventHandler;
    }

    private void SpineEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        if (this.OnSpineEvent != null)
        {
            this.OnSpineEvent(e.Data.Name);
        }
    }

    private void SetCrackAnim()
    {
        if (skunk.destructionCount < 1 && skunk.currentPhase == 1)
        {
            anim.AnimationState.SetAnimation(SecondaryTrack, Crack, loop: true);
        }
        else if (skunk.destructionCount >= 1 && skunk.currentPhase == 1)
        {
            anim.AnimationState.SetAnimation(SecondaryTrack, NonCrack, loop: true);
        }

        if (skunk.currentPhase == 2)
        {
            anim.AnimationState.SetAnimation(SecondaryTrack, NonCrack, loop: true);
        }
    }

    private void Update()
    {
        if (!(state != null))
        {
            return;
        }

        if (AutomaticallySetFacing)
        {
            if (45 <= state.facingAngle && state.facingAngle <= 135)
                DirectionState = direction3.up;
            else if (225 <= state.facingAngle && state.facingAngle <= 315)
                DirectionState = direction3.down;
            else if (135 < state.facingAngle && state.facingAngle < 225)
            {
                Dir = -1;
                DirectionState = direction3.Middle;
            }
            else if (state.facingAngle < 45 || state.facingAngle > 315)
            {
                Dir = 1;
                DirectionState = direction3.Middle;
            }
        }



        SpineChartacterAnimationData animationData = GetAnimationData(StateMachine.State.Moving);
        if (animationData != null && !(animationData.Animation == animationData.DefaultAnimation) && !ForceDirectionalMovement)
        {
            return;
        }



        CurrentState = state.CURRENT_STATE;


        if (playerController != null)
        {
            UpdateAnimFromFacing();
        }
    }
}
