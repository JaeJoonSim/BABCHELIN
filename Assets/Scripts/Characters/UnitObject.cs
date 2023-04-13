using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitObject : BaseMonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public StateMachine state;
    [HideInInspector] public Health health;
    [HideInInspector] public float vx;
    [HideInInspector] public float vy;
    [HideInInspector] public float moveVX;
    [HideInInspector] public float moveVY;

    [HideInInspector] public float speed;
    [HideInInspector] public float maxSpeed = 0.05f;
    private Vector2 positionLastFrame;

    public bool UseDeltaTime { get; set; } = true;
    private Vector3 previousPosition = Vector3.zero;

    protected bool isDead = false;

    public virtual void Awake()
    {
        health = GetComponent<Health>();
        state = GetComponent<StateMachine>();
        rb = base.gameObject.GetComponent<Rigidbody2D>();
    }

    public virtual void OnEnable()
    {
        health.OnHit += OnHit;
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

    public virtual void OnDisable()
    {
        health.OnHit -= OnHit;
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

    public virtual void OnHit(GameObject Attacker, Vector3 AttackLocation)
    {
        CameraManager.instance.ShakeCameraForDuration(0.6f, 0.8f, 0.3f, StackShakes: false);
    }
}
