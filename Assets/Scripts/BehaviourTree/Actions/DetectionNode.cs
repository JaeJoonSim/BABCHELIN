using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

[System.Serializable]
public class DetectionNode : ActionNode
{
    public float detectionRadius;
    public LayerMask targetLayer;

    protected override void OnStart()
    {
        blackboard.isDetected = false;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(context.transform.position, detectionRadius, targetLayer);
        if (hitColliders.Length > 0)
        {
            blackboard.target = hitColliders[0].gameObject;
            blackboard.isDetected = true;
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
