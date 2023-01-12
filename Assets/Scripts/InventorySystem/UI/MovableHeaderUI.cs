using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    [Tooltip("¿Ãµøµ… UI")]
    private Transform targetTr;
    public Transform TargetTr
    {
        get { return targetTr; }
        set { targetTr = value; }
    }

    private Vector2 beginPoint;
    private Vector2 moveBegin;

    private void Awake()
    {
        if (targetTr == null)
            targetTr = transform.parent;
    }
    
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPoint = targetTr.position;
        moveBegin = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        targetTr.position = beginPoint + (eventData.position - moveBegin);
    }
}
