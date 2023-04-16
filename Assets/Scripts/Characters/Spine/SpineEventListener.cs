using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineEventListener : MonoBehaviour
{
    public SimpleSpineEventListener spineEventListener;

    public void OnSpineEvent(string EventName)
    {
        switch (EventName)
        {
            case "land":
                Debug.Log("land");
                break;
        }
    }

    private void Start()
    {
        if (spineEventListener != null)
        {
            spineEventListener.OnSpineEvent += OnSpineEvent;
        }
    }
}
