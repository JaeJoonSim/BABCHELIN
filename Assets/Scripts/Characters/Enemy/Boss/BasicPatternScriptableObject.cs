using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewBasicPattern", menuName = "Patterns/BasicPattern")]
public class BasicPatternScriptableObject : PatternScriptableObject
{
    public override void ExecutePattern(Skunk skunk)
    {
        Debug.Log("Basic : " + patternName);
    }
}