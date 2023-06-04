using FMOD;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSounds : MonoBehaviour
{
    private AudioSource audioSource;
    private SkeletonAnimation anim;
    private StateMachine state;
    private Health health;
    private Skunk skunk;

    public AudioClip skunk_Land;
    public AudioClip skunk_Spin;
    public AudioClip skunk_Fart;
    public AudioClip skunk_Explosion;
    public AudioClip skunk_1_Hit;
    public AudioClip skunk_Spin_Hit;
    public AudioClip skunk_2_Hit;
    public AudioClip skunk_Shield_Hit;
    public AudioClip skunk_Shield;
    public AudioClip skunk_Charge;
    public AudioClip skunk_Death;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<SkeletonAnimation>();
        state = GetComponent<StateMachine>();
        health = GetComponent<Health>();
        skunk = GetComponent<Skunk>();

        anim.AnimationState.Event += OnSpineEvent;
        health.OnDamaged += OnDamaged;
        health.OnDie += Die;
    }

    private void Die()
    {
        PlayBossSound(skunk_Death);
    }

    private void OnDamaged(GameObject attacker, Vector3 attackLocation, float damage, Health.AttackType type)
    {
        if (skunk.currentPhase == 1)
        {
            if (state.CURRENT_STATE == StateMachine.State.PhaseChange)
            {
                PlayBossSound(skunk_Spin_Hit);
            }
            else
            {
                PlayBossSound(skunk_1_Hit);
            }
        }
        else if (skunk.currentPhase == 2)
        {
            if (skunk.isShieldActive)
            {
                PlayBossSound(skunk_Shield_Hit);
            }
            else
            {
                PlayBossSound(skunk_2_Hit);
            }
        }
    }

    public void PlayBossSound(AudioClip ac)
    {
        if (ac != null)
        {
            audioSource.clip = ac;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    public void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Jump:
                if (e.Data.Name == "bump")
                {
                    PlayBossSound(skunk_Land);
                }
                break;
            case StateMachine.State.Tailing:
                if (e.Data.Name == "effect_tail_whip")
                {

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
                    PlayBossSound(skunk_Fart);
                }
                break;
            case StateMachine.State.PhaseChange:
                if (e.Data.Name == "change_gimmik_spin")
                {
                    PlayBossSound(skunk_Spin);
                }
                break;
            case StateMachine.State.InstantKill:
                if (e.Data.Name == "effect_explode")
                {
                    //PlayBossSound(skunk_Explosion);
                }
                break;
            case StateMachine.State.FartShield:
                if (e.Data.Name == "effeck_fart")
                {
                    PlayBossSound(skunk_Shield);
                }
                break;
            case StateMachine.State.Outburst:
                if (e.Data.Name == "run")
                {
                    PlayBossSound(skunk_Charge);
                }
                break;
            case StateMachine.State.Dead:

                break;
        }
    }
}
