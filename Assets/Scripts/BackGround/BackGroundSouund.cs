using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSouund : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip chestOpen;
    public AudioClip chestDisappear;
    public AudioClip objectDestroy;
    public AudioClip objectAbsorb;
    public AudioClip objectApear;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip ac)
    {
        if (ac != null)
        {
            audioSource.clip = ac;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
    //public void PlayPlayerSound(string ac)
    //{
    //    if (ac == "Land")
    //    {
    //        if (pcWalkCookie != null)
    //        {
    //            audioSource.clip = pdLand;
    //            audioSource.PlayOneShot(audioSource.clip);
    //        }
    //    }
    //}

    public void StopSound()
    {
        audioSource.Stop();
        //audioSource.Play();
    }
}
