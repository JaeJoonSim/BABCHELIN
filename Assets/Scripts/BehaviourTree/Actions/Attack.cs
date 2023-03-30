using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

[System.Serializable]
public class Attack : ActionNode
{
    protected override void OnStart()
    {
        if (context.animator != null)
        {
            context.animator.SetTrigger("IsAttack");
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            return State.Running;
        }
        return State.Success;
    }
}
