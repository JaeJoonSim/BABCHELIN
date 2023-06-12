using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System.Security.Policy;
using System;
using Unity.Mathematics;

public class PlayerSound : MonoBehaviour
{
    private AudioSource audioSource;
    private SkeletonAnimation anim;
    private StateMachine state;
    private Health health;

    public AudioClip pcWalkCookie;
    public AudioClip pcRolling;
    public AudioClip pcAbsorb;
    public AudioClip pcBuffGet;//
    public AudioClip pcHit;
    public AudioClip pdLand;//
    public AudioClip pcDeath;
    public AudioClip pcShoot;
    public AudioClip pcSmallSkill;
    public AudioClip pcLargeSkill;
    public AudioClip pcTotemGet;

    private float delay;

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

    private void Die()
    {
        PlayPlayerSound(pcDeath);
    }

    private void OnDamaged(GameObject attacker, Vector3 attackLocation, float damage, Health.AttackType type)
    {
        PlayPlayerSound(pcHit);
    }

    public void PlayPlayerSound(AudioClip ac)
    {
        if (ac != null)
        {
            audioSource.clip = ac;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    public void PlayPlayerSound(string ac)
    {
        if (ac == "Land")
        {
            if (pdLand != null)
            {
                audioSource.clip = pdLand;
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
        
    }

    public void StopSound()
    {
        audioSource.Stop();
        //audioSource.Play();
    }

    public void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {

        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Moving:
                if (e.Data.Name == "land")
                {
                    PlayPlayerSound(pcWalkCookie);
                }
                break;

        }

    }

}
