using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

[System.Serializable]
public class MoveToPlayer : ActionNode
{
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = false;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart()
    {
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.destination = blackboard.target.transform.position;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;

        if (context.animator != null)
        {
            context.animator.SetBool("IsMoving", true);
        }
    }

    protected override void OnStop()
    {
        if (context.animator != null)
        {
            context.animator.SetBool("IsMoving", false);
        }
    }

    protected override State OnUpdate()
    {
        if (context.agent.pathPending)
        {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance)
        {
            if (context.animator != null)
            {
                context.animator.SetBool("IsMoving", false);
            }

            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }
}
