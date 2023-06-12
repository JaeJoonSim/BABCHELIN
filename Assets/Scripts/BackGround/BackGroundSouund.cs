using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSouund : MonoBehaviour
{
    static private BackGroundSouund instance;
    static public BackGroundSouund Instance { get { return instance; } }

    private AudioSource audioSource;
    public AudioClip chestOpen;
    public AudioClip chestDisappear;
    public AudioClip objectDestroy;
    public AudioClip objectAbsorb;
    public AudioClip objectAbsorbInPlayer;
    public AudioClip objectApearDown;
    public AudioClip objectApearUp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }
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

    public void PlaySound(string ac)
    {
        switch (ac)
        {
            case "chestOpen":
                if (chestOpen != null)
                {
                    audioSource.clip = chestOpen;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;
            case "chestDisappear":
                if (chestDisappear != null)
                {
                    audioSource.clip = chestDisappear;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;
            case "objectDestroy":
                if (objectDestroy != null)
                {
                    audioSource.clip = objectDestroy;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;
            case "objectAbsorb":
                if (objectAbsorb != null)
                {
                    audioSource.clip = objectAbsorb;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;
            case "objectAbsorbInPlayer":
                if (objectAbsorb != null)
                {
                    audioSource.clip = objectAbsorb;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;              
            case "objectApearDown":
                if (objectApearDown != null)
                {
                    audioSource.clip = objectApearDown;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;
            case "objectApearUp":
                if (objectApearUp != null)
                {
                    audioSource.clip = objectApearUp;
                    audioSource.PlayOneShot(audioSource.clip);
                }
                break;
            default:
                break;
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
}
