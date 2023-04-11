using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitObject : BaseMonoBehaviour
{
    [HideInInspector] Rigidbody2D rb;
    [HideInInspector] StateMachine state;
    [HideInInspector] public float vx;
    [HideInInspector] public float vy;
     public float moveVX;
    public float moveVY;

    [HideInInspector] public float speed;
    public float maxSpeed = 0.05f;
    public float SpeedMultiplier = 1f;
    private Vector2 positionLastFrame;

    public bool UseDeltaTime { get; set; } = true;

    private Vector3 previousPosition = Vector3.zero;

    public virtual void Awake()
    {
        state = GetComponent<StateMachine>();
        rb = base.gameObject.GetComponent<Rigidbody2D>();
    }

    public virtual void Update()
    {
        float num2 = (UseDeltaTime ? GameManager.DeltaTime : GameManager.UnscaledDeltaTime);
        Move();
    }

    protected virtual void FixedUpdate()
    {
        if(!(rb == null))
        {
            float num = (UseDeltaTime ? GameManager.DeltaTime : GameManager.UnscaledDeltaTime);
            if (float.IsNaN(moveVX) || float.IsInfinity(moveVX))
            {
                moveVX = 0f;
            }
            if (float.IsNaN(moveVY) || float.IsInfinity(moveVY))
            {
                moveVY = 0f;
            }
            Vector2 position = rb.position + new Vector2(vx, vy) * Time.deltaTime + new Vector2(moveVX, moveVY) * num;
            rb.MovePosition(position);
            positionLastFrame = position;
        }
    }

    protected void Move()
    {
        if (!float.IsNaN(state.facingAngle))
        {
            if (float.IsNaN(speed) || float.IsInfinity(speed))
            {
                speed = 0f;
            }
            speed = Mathf.Clamp(speed, 0f, maxSpeed);
            moveVX = speed * Mathf.Cos(state.facingAngle * ((float)Math.PI / 180f));
            moveVY = speed * Mathf.Sin(state.facingAngle * ((float)Math.PI / 180f));
            previousPosition = base.transform.position;
        }
    }
}
