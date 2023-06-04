using System;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using TMPro;

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

    public GameObject LogoImage;
    private Color logoColor;
    public GameObject[] ElseUI;
    private Color uiColor;

    public GameObject startButton;
    public TMP_Text[] uiText;

    private bool logoOn = false;
    private bool uiOn = false;

    // Start is called before the first frame update
    void Start()
    {
        anim.AnimationState.SetAnimation(AnimationTrack, StartAnim, loop: false);
        anim.AnimationState.AddAnimation(AnimationTrack, IdleAnim, loop: true, 0);

        spineAnimation = SpineTransform.GetComponent<SkeletonAnimation>();


        logoColor = new Color(1, 1, 1, 0);
        uiColor = new Color(1, 1, 1, 0);

        spineAnimation.AnimationState.Event += OnSpineEvent;
    }

    // Update is called once per frame
    void Update()
    {
        AppearLogo();
        AppearUI();

        if(startButton.GetComponent<Image>().color.a < 1)
        {
            startButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            startButton.GetComponent<Button>().interactable = true;
        }
    }

    void AppearLogo()
    {
        LogoImage.GetComponent<Image>().color = logoColor;

        if (logoOn)
        {
            logoColor.a += Time.deltaTime;
        }
    }

    void AppearUI()
    {
        for (int a = 0; a < ElseUI.Length; a++)
        {
            ElseUI[a].GetComponent<Image>().color = uiColor;
        }
        for (int a = 0; a < uiText.Length; a++)
        {
            uiText[a].color = uiColor;
        }

        if (uiOn)
        {
            if(uiColor.a < 1)
            {
                uiColor.a += Time.deltaTime;
            }
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        Debug.Log(e.Data.Name);
        if (e.Data.Name == "title_ui")
        {
            logoOn = true;
            //LogoImage.SetActive(true);
        }
        else if (e.Data.Name == "rest_of_ui")
        {
            uiOn = true;
        }
    }
}
