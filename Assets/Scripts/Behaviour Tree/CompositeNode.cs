using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BTNode
{
    [HideInInspector] public List<BTNode> children = new List<BTNode>();
}
