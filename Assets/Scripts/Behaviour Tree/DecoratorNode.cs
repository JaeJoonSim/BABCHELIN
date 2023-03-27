using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorNode : BTNode
{
    [HideInInspector] public BTNode child;
}
