using Spine;
using Spine.Unity;
using UnityEngine;

public class UnscaledTimeAnimation : BaseMonoBehaviour
{
    private SkeletonGraphic skeletonGraphic;

    public AnimationReferenceAsset Idle;
    public AnimationReferenceAsset emotion;

    private void Awake()
    {
        skeletonGraphic = GetComponent<SkeletonGraphic>();
    }

    private void OnEnable()
    {
        skeletonGraphic.AnimationState.SetAnimation(0, Idle, loop: true);
        if (emotion != null)
            skeletonGraphic.AnimationState.SetAnimation(1, emotion, loop: false);
    }

    private void LateUpdate()
    {
        if (skeletonGraphic != null && skeletonGraphic.AnimationState != null)
        {
            skeletonGraphic.AnimationState.Update(Time.unscaledDeltaTime);
            skeletonGraphic.AnimationState.Apply(skeletonGraphic.Skeleton);
            skeletonGraphic.LateUpdate();
        }
    }
}
