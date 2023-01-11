using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    [Tooltip("상호작용 위치")]
    private Transform intPoint;
    public Transform InteractionPoint
    {
        get { return intPoint; }
        set { intPoint = value; }
    }

    [SerializeField]
    [Tooltip("상호작용 감지 범위")]
    private float intRange;
    public float IntRange
    {
        get { return intRange; }
        set { intRange = value; }
    }

    [SerializeField]
    [Tooltip("상호작용 오브젝트 마스크")]
    private LayerMask intMask;
    public LayerMask IntMask
    {
        get { return intMask; }
        set { intMask = value; }
    }

    [SerializeField]
    [Tooltip("상호작용 가능한 오브젝트 개수")]
    private int numFound;
    public int NumFound
    {
        get { return numFound; }
        set { numFound = value; }
    }

    [SerializeField]
    [Tooltip("상호작용 UI")]
    private InteractionPromptUI intUI;
    public InteractionPromptUI IntUI
    {
        get { return intUI; }
        set { intUI = value; }
    }

    private readonly Collider[] _colliders = new Collider[10];

    private Interactable interactable;

    private void Update()
    {
        numFound = Physics.OverlapSphereNonAlloc(intPoint.position, intRange, _colliders, intMask);

        if(numFound > 0)
        {
            interactable = _colliders[0].GetComponent<Interactable>();

            if (interactable != null)
            {
                if (!IntUI.IsDisplayed) intUI.SetUp(interactable.InteractionPrompt);

                if (Keyboard.current.eKey.wasPressedThisFrame) interactable.Interact(this);
            }
        }
        else
        {
            if (interactable != null) interactable = null;
            if (IntUI.IsDisplayed) intUI.Close();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(intPoint.position, intRange);
    }
}
