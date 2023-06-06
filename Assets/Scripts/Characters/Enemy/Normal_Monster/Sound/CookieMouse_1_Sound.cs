using FMOD;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieMouse_1_Sound : MonoBehaviour
{
    private AudioSource audioSource;
    private SkeletonAnimation anim;
    private StateMachine state;
    private Health health;

    public AudioClip hit;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<SkeletonAnimation>();
        state = GetComponent<StateMachine>();
        health = GetComponent<Health>();

        anim.AnimationState.Event += OnSpineEvent;
        health.OnDamaged += OnDamaged;
        health.OnDie += Die;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public void PlaySound(AudioClip ac, bool loop)
    {
        if (ac != null)
        {
            audioSource.clip = ac;
            if(!loop)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            else
            {
                audioSource.Play();
            }
        }
    }

    public void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Attacking:
                break;
            case StateMachine.State.Dead:

                break;
        }
    }

    private void Die()
    {

    }

    private void OnDamaged(GameObject attacker, Vector3 attackLocation, float damage, Health.AttackType type)
    {
        PlaySound(hit, loop:false);
    }

}
