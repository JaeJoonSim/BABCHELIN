using System;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;

public class MainMenuIllust : MonoBehaviour
{
    public Transform SpineTransform;
    private SkeletonAnimation spineAnimation;
    public int AnimationTrack = 0;
    private SkeletonAnimation _anim;
    private SkeletonAnimation anim
    {
        get
        {
            if (_anim == null)
            {
                _anim = this.transform.GetChild(0).GetComponent<SkeletonAnimation>();
            }
            return _anim;
        }
    }
    public AnimationReferenceAsset StartAnim;
    public AnimationReferenceAsset IdleAnim;

    public GameObject mainCanvas;


    // Start is called before the first frame update
    void Start()
    {
        anim.AnimationState.SetAnimation(AnimationTrack, StartAnim, loop: false);
        anim.AnimationState.AddAnimation(AnimationTrack, IdleAnim, loop: true, 0);

        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();

        //mainCanvas.SetActive(false);

        spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "idle start")
        {
            //mainCanvas.SetActive(true);
        }
    }
}
