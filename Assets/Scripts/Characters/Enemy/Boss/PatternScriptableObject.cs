using UnityEngine;
using System;
using Spine.Unity;

public abstract class PatternScriptableObject : ScriptableObject
{
    public string patternName;
    public float duration;
    public Action onPatternStart;
    public Action onPatternEnd;
    public StateMachine.State patternState;
    public AnimationReferenceAsset anim;

    public virtual void ExecutePattern(Skunk skunk)
    {
        if (anim != null)
        {
            var animation = anim.Animation;
            if (animation != null)
            {
                duration = animation.Duration;
            }
        }
    }
}
