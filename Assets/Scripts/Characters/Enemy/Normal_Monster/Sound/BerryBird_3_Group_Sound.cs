using FMOD;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBird_3_Group_Sound : MonoBehaviour
{
    private AudioSource audioSource;
    private SkeletonAnimation anim;
    private StateMachine state;
    private Health health;

    public AudioClip hit;
    public AudioClip notice;
    public AudioClip death;

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

    public void PlaySound(AudioClip ac)
    {
        if (ac != null)
        {
            audioSource.clip = ac;
            audioSource.PlayOneShot(audioSource.clip);
            //if (!loop)
            //{
            //    audioSource.PlayOneShot(audioSource.clip);
            //}
            //else
            //{
            //    audioSource.Play();
            //}
        }
    }

    public void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Notice:
                PlaySound(notice);
                break;
            case StateMachine.State.HitLeft:
                break;
            case StateMachine.State.HitRight:
                break;
            case StateMachine.State.Dead:
                break;
            default:
                audioSource.Stop();
                break;
        }
    }

    private void Die()
    {
        PlaySound(death);
    }

    private void OnDamaged(GameObject attacker, Vector3 attackLocation, float damage, Health.AttackType type)
    {
        PlaySound(hit);
    }

}
