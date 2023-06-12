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
    public AudioClip pcAbsorb2;
    public AudioClip pcBuffGet;
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

    private void Update()
    {
        if (state.CURRENT_STATE == StateMachine.State.Absorbing)
        {
            if (!IsInvoking("absorb"))
            {
                InvokeRepeating("absorb", 0, 11.5f);
            }

            if (!IsInvoking("absorb2"))
                InvokeRepeating("absorb2", 0, 4.5f);
        }
        else
        {
            if(IsInvoking("absorb"))
                CancelInvoke("absorb");
            if (IsInvoking("absorb2"))
                CancelInvoke("absorb2");
        }
               

    }

    void absorb()
    {
        audioSource.clip = pcAbsorb;
        audioSource.PlayOneShot(audioSource.clip);
    }
    void absorb2()
    {
        audioSource.clip = pcAbsorb2;
        audioSource.PlayOneShot(audioSource.clip);
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
        else if (ac == "pcBuffGet")
        {
            if (pcBuffGet != null)
            {
                audioSource.clip = pcBuffGet;
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
        // Debug.Log(e.Data.Name);

        if (e.Data.Name == "land")
        {

            PlayPlayerSound(pcWalkCookie);
        }
    }

}
