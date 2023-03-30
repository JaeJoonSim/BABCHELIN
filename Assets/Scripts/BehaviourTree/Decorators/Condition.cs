using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

[System.Serializable]
public class Condition : DecoratorNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (CheckCondition())
        {
            Node.State childStatus = child.Update();
            return childStatus;
        }
        else
        {
            return Node.State.Failure;
        }
    }

    private bool CheckCondition()
    {


        return false;
    }
}
