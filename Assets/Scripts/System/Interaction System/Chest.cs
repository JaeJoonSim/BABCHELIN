using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    public UnityEvent offInteraction;
    #endregion

    private void Update()
    {
        OffInteract();
    }

    public bool OnInteract(Interactor interactor)
    {
        if (!UIManagerScript.OnUI)
        {
            onInteraction.Invoke();
            Debug.Log($"{gameObject.name} : OnInteracted with Chest");
            UIManagerScript.OnUI = true;
        }
        return true;
    }

    public void OffInteract()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            offInteraction.Invoke();
            Debug.Log("Close Chest");
            UIManagerScript.OnUI = false;
        }
    }
}
