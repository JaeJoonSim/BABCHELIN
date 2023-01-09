using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField]
    [Tooltip("PC �ȱ��� �ִ� �̵� �ӵ�")]
    private float pcWalkMSpd;
    public float PcWalkMSpd
    {
        get { return pcWalkMSpd; }
        set { pcWalkMSpd = value; }
    }

    [SerializeField]
    [Tooltip("PC �ٱ��� �ִ� �̵� �ӵ�")]
    private float pcRunMSpd;
    public float PcRunMSpd
    {
        get { return pcRunMSpd; }
        set { pcRunMSpd = value; }
    }

    [SerializeField]
    [Tooltip("PC ���� �ӵ�")]
    private float pcJumpHeight;
    public float PcJumpHeight
    {
        get { return pcJumpHeight; }
        set { pcJumpHeight = value; }
    }

    [SerializeField]
    [Tooltip("PC ���� ��Ÿ��")]
    private float pcJumpDelay;
    public float PcJumpDelay
    {
        get { return pcJumpDelay; }
        set { pcJumpDelay = value; }
    }

    /// <summary>
    /// True = Walk, False = Run
    /// </summary>
    [SerializeField]
    [Tooltip("�ȱ�/�ٱ� ���Ľ� Ʈ����")]
    private bool pcWRTrigger;
    public bool PcWRTrigger
    {
        get { return pcWRTrigger; }
        set { pcWRTrigger = value; }
    }

    #endregion

    #region Private Value
    private Rigidbody rb;

    private float groundDistance = .1f;

    #region Player Input Fields
    private float inputX;
    private float inputZ;
    #endregion

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        PlayerMove();

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(new Vector3(0, pcJumpHeight, 0), ForceMode.Impulse);
        }
    }

    private void PlayerMove()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 vel = new Vector3(inputX, 0, inputZ);
        
        vel = (pcWRTrigger) ? vel *= pcWalkMSpd : vel *= pcRunMSpd;

        rb.velocity = vel;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance);
    }
}
