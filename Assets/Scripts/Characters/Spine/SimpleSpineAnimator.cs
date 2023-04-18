using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleSpineAnimator : BaseMonoBehaviour
{
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
    public bool AutomaticallySetFacing = true;
    public AnimationReferenceAsset DefaultLoop;
    public List<SpineChartacterAnimationData> Animations = new List<SpineChartacterAnimationData>();

    [Header("Idle")]
    public AnimationReferenceAsset Idle;
    public AnimationReferenceAsset NorthIdle;

    [Space, Header("Move")]
    public AnimationReferenceAsset StartMoving;
    public AnimationReferenceAsset Moving;
    public AnimationReferenceAsset MovingBack;
    public AnimationReferenceAsset NorthMoving;
    public AnimationReferenceAsset SouthMoving;
    public AnimationReferenceAsset StopMoving;

    [Space, Header("Attack")]
    public AnimationReferenceAsset Attack;
    public AnimationReferenceAsset NorthAttack;

    [Space, Header("Action")]
    public AnimationReferenceAsset Dodge;
    public AnimationReferenceAsset Absorb;
    public AnimationReferenceAsset Hit;
    public AnimationReferenceAsset Dead;

    private TrackEntry Track;
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
                UpdateAnimFromState();
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
                anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
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
            case StateMachine.State.Dead:
                if (Dead != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Dead, loop: false);
                }
                break;
            default:
                if (Idle != null && anim.AnimationState != null)
                {
                    anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
                }
                break;
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
            }
        }
        else
        {
            Track = anim.AnimationState.GetCurrent(0);
        }
        Dir = 1;
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

    private void Update()
    {
        if (!(state != null))
        {
            return;
        }
        CurrentState = state.CURRENT_STATE;
        if (AutomaticallySetFacing)
        {
            Dir = ((state.facingAngle > 140f && state.facingAngle < 220) ? 1 : (-1)) * ((!ReverseFacing) ? (-1) : 1);
        }

        if (NorthIdle != null && CurrentState == StateMachine.State.Idle)
        {
            if (state.facingAngle > 40f && state.facingAngle < 140f)
            {
                if (Track != null && Track.Animation != NorthIdle.Animation)
                {
                    Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthIdle, loop: true);
                }
            }
            else if (Track != null && Track.Animation != Idle.Animation)
            {
                Track = anim.AnimationState.SetAnimation(AnimationTrack, Idle, loop: true);
            }
        }

        if(NorthAttack != null && CurrentState == StateMachine.State.Attacking)
        {
            if (state.facingAngle > 40f && state.facingAngle < 140f)
            {
                if (Track != null && Track.Animation != NorthAttack.Animation)
                {
                    Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthAttack, loop: true);
                }
            }
            else if (Track != null && Track.Animation != Attack.Animation)
            {
                Track = anim.AnimationState.SetAnimation(AnimationTrack, Attack, loop: true);
            }
        }

        SpineChartacterAnimationData animationData = GetAnimationData(StateMachine.State.Moving);
        if (animationData != null && !(animationData.Animation == animationData.DefaultAnimation) && !ForceDirectionalMovement)
        {
            return;
        }

        if (MovingBack != null && CurrentState == StateMachine.State.Moving)
        {
            if (MovingBack != null && CurrentState == StateMachine.State.Moving)
            {
                if (state.facingAngle > 140 && state.facingAngle < 270f && playerController.xDir > 0)
                {
                    if (Track != null && Track.Animation != MovingBack.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, MovingBack, loop: true);
                    }
                }
                else if (state.facingAngle > 140 && state.facingAngle < 270f && playerController.xDir < 0)
                {
                    if (Track != null && Track.Animation != Moving.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                    }
                }
                if ((state.facingAngle <= 140 || state.facingAngle >= 270f) && playerController.xDir < 0)
                {
                    if (Track != null && Track.Animation != MovingBack.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, MovingBack, loop: true);
                    }
                }
                else if ((state.facingAngle <= 140 || state.facingAngle >= 270f) && playerController.xDir > 0)
                {
                    if (Track != null && Track.Animation != Moving.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                    }
                }
                else if (MovingBack == null)
                {
                    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
                }
            }
        }

        if (MovingBack != null && CurrentState == StateMachine.State.Moving)
        {
            if (state.facingAngle < 90f && state.facingAngle > 270f && playerController.xDir < 0)
            {
                if (Track != null && Track.Animation != MovingBack.Animation)
                {
                    Track = anim.AnimationState.SetAnimation(AnimationTrack, MovingBack, loop: true);
                }
            }
        }

        if (NorthMoving != null && CurrentState == StateMachine.State.Moving)
        {
            if (state.facingAngle > 40f && state.facingAngle < 140f)
            {
                if (state.facingAngle > 70f && state.facingAngle < 110f)
                {
                    if (Track.Animation != NorthMoving.Animation)
                    {
                        Track = anim.AnimationState.SetAnimation(AnimationTrack, NorthMoving, loop: true);
                    }
                }
            }
            //else if ((SouthMoving == null || (SouthMoving != null && state.facingAngle < 220f && state.facingAngle > 320f)) && Track.Animation != Moving.Animation)
            //{
            //    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
            //}
        }
        if (SouthMoving != null && CurrentState != StateMachine.State.Moving)
        {
            if (state.facingAngle > 220f && state.facingAngle < 320f)
            {
                Track = anim.AnimationState.SetAnimation(AnimationTrack, SouthMoving, loop: true);

            }
        }
        
        //else if ((NorthMoving == null || (NorthMoving != null && (state.facingAngle < 40f || state.facingAngle > 140f))) && Track.Animation != Moving.Animation)
        //{
        //    Track = anim.AnimationState.SetAnimation(AnimationTrack, Moving, loop: true);
        //}
    }
}
