using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Fields
    [Header("PC Movement")]
    [SerializeField]
    [Tooltip("PC 걷기의 최대 이동 속도")]
    private float pcWalkMSpd;
    public float PcWalkMSpd
    {
        get { return pcWalkMSpd; }
        set { pcWalkMSpd = value; }
    }

    [SerializeField]
    [Tooltip("PC 뛰기의 최대 이동 속도")]
    private float pcRunMSpd;
    public float PcRunMSpd
    {
        get { return pcRunMSpd; }
        set { pcRunMSpd = value; }
    }

    [Space]
    [Header("PC Jump")]
    [SerializeField]
    [Tooltip("PC 점프 속도")]
    private float pcJumpHeight;
    public float PcJumpHeight
    {
        get { return pcJumpHeight; }
        set { pcJumpHeight = value; }
    }

    [SerializeField]
    [Tooltip("PC 점프 쿨타임")]
    private float pcJumpDelay;
    public float PcJumpDelay
    {
        get { return pcJumpDelay; }
        set { pcJumpDelay = value; }
    }

    [SerializeField]
    [Tooltip("땅 레이어 체크")]
    private LayerMask whatIsGround;
    public LayerMask WhatIsGround
    {
        get { return whatIsGround; }
        set { whatIsGround = value; }
    }

    /// <summary>
    /// True = Walk, False = Run
    /// </summary>
    [Space]
    [Header("Extra")]
    [SerializeField]
    [Tooltip("걷기/뛰기 스탠스 트리거")]
    private bool pcWRTrigger;
    public bool PcWRTrigger
    {
        get { return pcWRTrigger; }
        set { pcWRTrigger = value; }
    }
    
    #endregion

    #region Private Value

    private Rigidbody rb;
    private Animator animator;

    private Vector3 moveDir;
    private float groundDistance = .3f;
    private bool isGrounded;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerAnimation();
        PlayerInput();
        Jump();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        if (moveDir != Vector3.zero)
        {
            if (pcWRTrigger)
            {
                rb.MovePosition(transform.position + moveDir * pcWalkMSpd * Time.deltaTime);
            }
            else
            {
                rb.MovePosition(transform.position + moveDir * pcRunMSpd * Time.deltaTime);
            }
        }
    }

    private void PlayerInput()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();
    }

    private void Jump()
    {
        CheckGround();
        if (Input.GetButtonDown("Jump") && isGrounded && Time.timeScale != 0)
        {
            rb.AddForce(Vector3.up * pcJumpHeight, ForceMode.VelocityChange);
        }
    }

    private void CheckGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, groundDistance, whatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void PlayerAnimation()
    {
        if (moveDir != Vector3.zero)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }
}
