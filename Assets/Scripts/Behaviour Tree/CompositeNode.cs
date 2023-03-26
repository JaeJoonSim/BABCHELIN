using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BTNode
{
    public List<BTNode> children = new List<BTNode>();
}
