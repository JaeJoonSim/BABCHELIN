using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseMonoBehaviour
{
    [HideInInspector] UnitObject unitObject;
    private StateMachine state;
    private CircleCollider2D circleCollider2D;

    public Transform SpineTransform;

    [Header("�̵�")]
    [HideInInspector] public float defaultRunSpeed = 5.5f;
    public float runSpeed = 5.5f;
    public static float MinInputForMovement = 0.3f;

    [Header("�̵� üũ")]
    public float forceDir;
    public float speed;
    public float xDir;
    public float yDir;

    [Space, Header("������")]
    public float DodgeTimer;
    public float DodgeSpeed = 12f;
    public float DodgeDuration = 0.3f;
    public float DodgeMaxDuration = 0.5f;
    public float DodgeDelay = 0.3f;
    private float DodgeCollisionDelay;

    private float VZ;
    private float Z;

    private void Start()
    {
        unitObject = base.gameObject.GetComponent<UnitObject>();
        state = base.gameObject.GetComponent<StateMachine>();
        circleCollider2D = base.gameObject.GetComponent<CircleCollider2D>();
        defaultRunSpeed = runSpeed;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver)
        {
            return;
        }

        xDir = Input.GetAxis("Horizontal");
        yDir = Input.GetAxis("Vertical");
        if(state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }
        speed = Mathf.Max(speed, 0f);
        unitObject.vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        unitObject.vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Idle:
                Z = 0f;
                SpineTransform.localPosition = Vector3.zero;
                speed += (0f - speed) / 3f * GameManager.DeltaTime;
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    state.CURRENT_STATE = StateMachine.State.Moving;
                }
                break;
                
            case StateMachine.State.Moving:
                if (Time.timeScale == 0f)
                {
                    break;
                }
                if (Mathf.Abs(xDir) < MinInputForMovement && Mathf.Abs(yDir) < MinInputForMovement)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                    break;
                }
                forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                if(unitObject.vx != 0f || unitObject.vy != 0f)
                {
                    state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(unitObject.vx, unitObject.vy));
                }
                state.LookAngle = state.facingAngle;
                speed += (runSpeed - speed) / 3f * GameManager.DeltaTime;
                break;

            case StateMachine.State.Dodging:
                Z = 0f;
                SpineTransform.localPosition = Vector3.zero;
                forceDir = state.facingAngle;
                if (DodgeCollisionDelay < 0f)
                {
                    speed = Mathf.Lerp(speed, DodgeSpeed, 2f * Time.deltaTime);
                }
                DodgeCollisionDelay -= Time.deltaTime;
                DodgeTimer += Time.deltaTime;
                if (DodgeTimer < 0.1f && (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement))
                {
                    state.facingAngle = (forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir)));
                }
                if ((!Input.GetKey(KeyCode.LeftShift) && DodgeTimer > DodgeDuration) || DodgeTimer > DodgeMaxDuration)
                {
                    DodgeTimer = 0f;
                    DodgeCollisionDelay = 0f;
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
                break;
        }

    }

    private IEnumerator Delay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
