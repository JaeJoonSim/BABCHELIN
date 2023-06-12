using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlay : BaseMonoBehaviour
{
    private VideoPlayer vp;

    private void Start()
    {
    }

    private void OnEnable()
    {
        vp = GetComponent<VideoPlayer>();
        vp.Play();
    }
}
