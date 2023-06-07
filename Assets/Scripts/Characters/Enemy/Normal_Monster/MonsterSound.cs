using FMOD;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSound : MonoBehaviour
{
    private AudioSource audioSource;
    private SkeletonAnimation anim;
    private StateMachine state;
    private Health health;

    public AudioClip spawn;
    public AudioClip attack;
    public AudioClip dash;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip notice;
    public AudioClip hit;
    public AudioClip defend;
    public AudioClip death;

    private int soundCount;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<SkeletonAnimation>();
        state = GetComponent<StateMachine>();
        health = GetComponent<Health>();

        anim.AnimationState.Event += OnSpineEvent;
        health.OnDamaged += OnDamaged;
        health.OnDie += Die;
    }

    private void Update()
    {
        
    }

    private void Die()
    {
        PlaySound(death);
    }

    private void OnDamaged(GameObject attacker, Vector3 attackLocation, float damage, Health.AttackType type)
    {
        PlaySound(hit);
    }

    public void PlaySound(AudioClip ac)
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
            case StateMachine.State.Attacking:
                PlaySound(attack);
                break;
            case StateMachine.State.Dash:
                PlaySound(dash);
                break;
            case StateMachine.State.Jump:
                if (e.Data.Name == "jump")
                {
                    PlaySound(jump);
                }
                else if (e.Data.Name == "land")
                {
                    PlaySound(land);
                }
                break;
            case StateMachine.State.Defend:
                PlaySound(defend);
                break;
            case StateMachine.State.Notice:
                PlaySound(notice);
                break;
            case StateMachine.State.Spawn:
                PlaySound(spawn);
                break;
            case StateMachine.State.FartShield:
                break;
            case StateMachine.State.Outburst:
                break;
            case StateMachine.State.Dead:

                break;
        }
    }
}
