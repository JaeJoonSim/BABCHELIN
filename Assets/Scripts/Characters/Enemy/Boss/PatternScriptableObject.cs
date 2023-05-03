using UnityEngine;
using System;

public abstract class PatternScriptableObject : ScriptableObject
{
    public string patternName;
    public float duration;
    public Action onPatternStart;
    public Action onPatternEnd;
    public StateMachine.State patternState;

    public virtual void ExecutePattern(Skunk skunk)
    {
        
    }
}
