using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimationBehaviour : StateMachineBehaviour
{
    public AnimationClip motion;

    [Header("스파인 모션 레이어")]
    public int layer = 0;
    public float timeScale = 1.0f;

    private string animationClip;
    private bool loop;

    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spineAnimationState;
    private Spine.TrackEntry trackEntry;

    private void Awake()
    {
        if (motion != null)
            animationClip = motion.name;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = animator.GetComponentInChildren<SkeletonAnimation>();
            spineAnimationState = skeletonAnimation.state;
        }
        
        if(animationClip != null)
        {
            loop = stateInfo.loop;
            trackEntry = spineAnimationState.SetAnimation(layer, animationClip, loop);
            trackEntry.TimeScale = timeScale;
        }
    }
}
