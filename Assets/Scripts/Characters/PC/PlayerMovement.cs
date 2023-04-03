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
    [Tooltip("구르기 속도")]
    private float pcRollingSpeed;
    public float PcRollingSpeed
    {
        get { return pcRollingSpeed; }
        set { pcRollingSpeed = value; }
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
    public Rigidbody Rigid
    {
        get { return rb; }
    }
    private Animator anim;

    private Vector3 moveDir;
    private float groundDistance = .2f;
    public bool isGrounded;
    public bool isKnockedBack;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerInput();
        PlayerAnimation();
        Jump();
        Rolling();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        if(!UIManagerScript.OnUI)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
                moveDir = Vector3.zero;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                moveDir /= 10;

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
    }

    private void PlayerInput()
    {
        if (!isKnockedBack)
        {
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.z = Input.GetAxisRaw("Vertical");
            moveDir.Normalize();
        }
        else
        {
            moveDir = Vector3.zero;
        }
    }

    private void Jump()
    {
        CheckGround();
        if (Input.GetButtonDown("Jump") && isGrounded && Time.timeScale != 0 &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Landing") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            rb.AddForce(Vector3.up * pcJumpHeight, ForceMode.VelocityChange);
        }
    }

    private void Rolling()
    {
        CheckGround();
        if (Input.GetKeyUp(KeyCode.LeftShift) && isGrounded && Time.timeScale != 0)
        {
            anim.SetTrigger("isRolling");
            rb.AddForce(moveDir * pcRollingSpeed, ForceMode.VelocityChange);
        }
    }

    private void CheckGround()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position + (Vector3.up * 0.1f), Vector3.down * groundDistance, Color.black);
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, groundDistance, whatIsGround))
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
            if (pcWRTrigger == false)
                anim.SetBool("isRun", true);
            else if (pcWRTrigger == true)
                anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isRun", false);
            anim.SetBool("isWalk", false);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && Time.timeScale != 0)
            anim.SetTrigger("isJump");

        if (isGrounded)
            anim.SetBool("isGround", true);
        else
            anim.SetBool("isGround", false);
    }
}
