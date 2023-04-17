using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseMonoBehaviour
{
    [HideInInspector] UnitObject unitObject;
    private StateMachine state;
    private CircleCollider2D circleCollider2D;
    private Health health;

    public Transform SpineTransform;

    [Header("이동")]
    [HideInInspector] public float defaultRunSpeed = 5.5f;
    public float runSpeed = 5.5f;
    public static float MinInputForMovement = 0.3f;

    [Header("이동 체크")]
    public float forceDir;
    public float speed;
    public float xDir;
    public float yDir;

    [Space, Header("구르기")]
    public float DodgeTimer;
    public float DodgeSpeed = 12f;
    public float DodgeDuration = 0.3f;
    public float DodgeMaxDuration = 0.5f;
    public float DodgeDelay = 0.3f;
    private float DodgeCollisionDelay;

    [Header("흡수")]
    public float SuctionSpeed = 1f;
    public float SuctionAngle = 30f;
    public float SuctionRange = 10f;
    public float SuctionDelay = 1f;
    public ParticleSystem absorbEffet;

    [Header("공격")]
    public GameObject[] Attack;
    public int CurAttack;
    public float[] AttackSpeed;
    public ParticleSystem[] AttackEffet;


    public Transform muzzle;
    public Transform muzzleBone;
    public Transform GrinderControl;

    private float VZ;
    private float Z;

    private void Start()
    {
        unitObject = base.gameObject.GetComponent<UnitObject>();
        state = base.gameObject.GetComponent<StateMachine>();
        circleCollider2D = base.gameObject.GetComponent<CircleCollider2D>();
        defaultRunSpeed = runSpeed;
    }

    private void OnEnable()
    {
        health = base.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.OnHit += OnHit;
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver)
        {
            return;
        }

        xDir = Input.GetAxis("Horizontal");
        yDir = Input.GetAxis("Vertical");
        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }
        speed = Mathf.Max(speed, 0f);
        unitObject.vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        unitObject.vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        if (state.CURRENT_STATE != StateMachine.State.Dodging && state.CURRENT_STATE != StateMachine.State.Dead)
            state.facingAngle = Utils.GetMouseAngle(transform.position); 

        //if (state.CURRENT_STATE != StateMachine.State.Dodging && (state.CURRENT_STATE == StateMachine.State.Attacking || state.CURRENT_STATE == StateMachine.State.Absorbing))
        //    state.facingAngle = Utils.GetMouseAngle(transform.position);

        muzzleBone.position = muzzle.GetChild(0).position;

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
                    state.ChangeToIdleState();
                    break;
                }
                forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                if (unitObject.vx != 0f || unitObject.vy != 0f)
                {
                    //state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(unitObject.vx, unitObject.vy));
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

            case StateMachine.State.Attacking:
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (runSpeed - speed) / 3f * GameManager.DeltaTime;
                }
                else
                {
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                }
                break;

            case StateMachine.State.Dead:
                break;
        }

    }

    private void OnHit(GameObject Attacker, Vector3 AttackLocation)
    {
        if (!health.isInvincible)
        {
            return;
        }

        if (Attacker == null)
        {
            state.facingAngle = Utils.GetAngle(base.transform.position, AttackLocation);
        }
        else
        {
            state.facingAngle = Utils.GetAngle(base.transform.position, Attacker.transform.position);
        }
        forceDir = state.facingAngle + 180f;
        CameraManager.shakeCamera(10f, 0f - state.facingAngle);

        GameManager.GetInstance().HitStop();
    }

    private IEnumerator Delay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
