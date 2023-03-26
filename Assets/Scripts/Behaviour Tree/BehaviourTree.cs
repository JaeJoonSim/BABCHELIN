using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public BTNode rootNode;
    public BTNode.State treeState = BTNode.State.Running;
    
    public BTNode.State Update()
    {
        if (rootNode.state == BTNode.State.Running)
            treeState = rootNode.Update();

        return treeState;
    }
}
