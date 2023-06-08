using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineEventListener : MonoBehaviour
{
    private SimpleSpineEventListener spineEventListener;
    private StateMachine state;

    [Header("Particle")]
    public ParticleSystem Moving;
    public ParticleSystem Landing;

    private void Start()
    {
        state = GetComponentInParent<StateMachine>();

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
        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Landing:
                if (Landing != null)
                    Landing.Play();
                break;
        }

    }
}
