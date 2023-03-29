using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

[System.Serializable]
public class DetectionNode : ActionNode
{
    public LayerMask layer;
    public float detectionRadius;
    public bool isDetected;

    protected override void OnStart()
    {
        isDetected = false;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(context.transform.position, detectionRadius, layer);
        if (hitColliders.Length > 0)
        {
            isDetected = true;
            Debug.Log("°¨Áö");
            return State.Success;
        }

        return State.Failure;
    }

    public override void OnDrawGizmos()
    {
        if(drawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(context.transform.position, detectionRadius);
        }
    }
}
