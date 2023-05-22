using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEffect : BaseMonoBehaviour
{
    public ParticleSystem jump;
    public ParticleSystem tailWhip;
    public ParticleSystem failExplode;
    public ParticleSystem GimmickSpin;

    private SkeletonAnimation anim;
    private StateMachine state;

    private void Start()
    {
        anim = GetComponentInChildren<SkeletonAnimation>();
        state = GetComponent<StateMachine>();

        anim.AnimationState.Event += OnSpineEvent;
    }

    private void Update()
    {
        if (state.CURRENT_STATE == StateMachine.State.PhaseChange)
        {
            if (!GimmickSpin.isPlaying)
                GimmickSpin.Play();
        }
        else
            GimmickSpin.Stop();
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Jump:
                if (e.Data.Name == "bump")
                {
                    if (jump != null)
                    {
                        jump.Play();
                    }
                }
                break;
            case StateMachine.State.Tailing:
                if (e.Data.Name == "effect_tail_whip")
                {
                    if (tailWhip != null)
                    {
                        tailWhip.Play();
                    }
                }
                break;
            case StateMachine.State.Throwing:
                if (e.Data.Name == "cream_throw")
                {
                    
                }
                break;
            case StateMachine.State.Farting:
                if (e.Data.Name == "effeck_fart")
                {
                    
                }
                break;
            case StateMachine.State.InstantKill:
                if (e.Data.Name == "effect_explode")
                {
                    if (failExplode != null)
                        failExplode.Play();
                }
                break;
        }
    }
}
