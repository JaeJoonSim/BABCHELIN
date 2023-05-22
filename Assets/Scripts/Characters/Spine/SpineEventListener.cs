using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineEventListener : MonoBehaviour
{
    private SimpleSpineEventListener spineEventListener;

    [Header("Particle")]
    public ParticleSystem Moving;

    private void Start()
    {
        if (spineEventListener == null)
        {
            spineEventListener = base.GetComponent<SimpleSpineEventListener>();
        }
        if (spineEventListener != null)
        {
            spineEventListener.OnSpineEvent += OnSpineEvent;
        }
    }

    public void OnSpineEvent(string EventName)
    {
        switch (EventName)
        {
            case "land":
                if (Moving != null)
                    Moving.Play();
                break;
        }
    }
}
