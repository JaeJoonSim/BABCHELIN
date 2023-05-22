using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseMonoBehaviour
{
    [HideInInspector] UnitObject unitObject;
    private StateMachine state;
    public StateMachine State { get { return state; } }
    private CircleCollider2D circleCollider2D;
    private Health health;

    public Transform SpineTransform;

    [Header("이동")]
    [HideInInspector] public float defaultRunSpeed = 5.5f;
    public float runSpeed = 5.5f;
    public static float MinInputForMovement = 0.3f;

    //[Header("이동 체크")]
    [HideInInspector] public float forceDir;
    [HideInInspector] public float speed;
    [HideInInspector] public float xDir;
    [HideInInspector] public float yDir;

    [Header("회피")]
    public bool showDodge = false;
    [DrawIf("showDodge", true)] public float DodgeTimer;
    [DrawIf("showDodge", true)] public float DodgeSpeed = 12f;
    [DrawIf("showDodge", true)] public float DodgeAngle = 0f;
    [DrawIf("showDodge", true)] public float DodgeDuration = 0.3f;
    [DrawIf("showDodge", true)] public float DodgeMaxDuration = 0.5f;
    [DrawIf("showDodge", true)] public float DodgeDelay = 0.3f;
    [DrawIf("showDodge", true)] private float DodgeCollisionDelay;

    [Header("흡수")]
    public bool showAbsorb = false;
    [DrawIf("showAbsorb", true)] public float SuctionSpeed = 1f;
    [DrawIf("showAbsorb", true)] public float SuctionAngle = 30f;
    [DrawIf("showAbsorb", true)] public float SuctionRange = 10f;
    [DrawIf("showAbsorb", true)] public float SuctionDelay = 1f;
    [DrawIf("showAbsorb", true)] public ParticleSystem absorbEffet;

    [Header("공격"),Space]

    public int BulletGauge;
    public int maxBulletGauge;
    public GameObject Attack;

    public float AttackSpeed;

    public Transform muzzleBone;
    public Transform GrinderControl;

    public GameObject BulletUI;
    private float fadeTime = 0;

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
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver || state.CURRENT_STATE == StateMachine.State.Pause || state.CURRENT_STATE == StateMachine.State.Dead)
        {
            return;
        }

        //xDir = Input.GetAxisRaw("Horizontal");
        //yDir = Input.GetAxisRaw("Vertical");

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }
        speed = Mathf.Max(speed, 0f);
        unitObject.vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        unitObject.vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        //if (state.CURRENT_STATE != StateMachine.State.Dodging && state.CURRENT_STATE != StateMachine.State.Dead)
        //    state.facingAngle = Utils.GetMouseAngle(transform.position);

        // Later TODO...
        if (state.CURRENT_STATE != StateMachine.State.Dodging && (state.CURRENT_STATE == StateMachine.State.Attacking || state.CURRENT_STATE == StateMachine.State.Absorbing))
        {
            muzzleBone.position = Utils.GetMousePosition();
            state.facingAngle = Utils.GetMouseAngle(transform.position);

            if (BulletUI.GetComponent<CanvasGroup>().alpha < 1)
            {
                fadeTime = 0;
                BulletUI.GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        else
        {
            if (yDir > 0)
            {
                if (xDir < 0)
                    DodgeAngle = 135;
                else if (xDir > 0)
                    DodgeAngle = 45;
                else
                {
                    state.facingAngle = 90;
                    DodgeAngle = 90f;
                }
            }
            else if (yDir < 0)
            {
                if (xDir < 0)
                    DodgeAngle = 225;
                else if (xDir > 0)
                    DodgeAngle = 315;
                else
                {
                    state.facingAngle = 270;
                    DodgeAngle = 270;
                }
            }
            else
            {
                if (xDir < 0)
                {
                    muzzleBone.position = transform.position + new Vector3(-1, 0, 0);
                    state.facingAngle = 180;
                    DodgeAngle = 180;
                }
                else if (xDir > 0)
                {
                    muzzleBone.position = transform.position + new Vector3(1, 0, 0);
                    state.facingAngle = 0;
                    DodgeAngle = 0;
                }
                else
                {
                }
            }
            if (BulletUI.activeSelf)
            {
                fadeTime += Time.deltaTime;

                if (fadeTime >= 1f)
                {
                    BulletUI.GetComponent<CanvasGroup>().alpha -= (Time.deltaTime * 2);
                }
            }
        }


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
                forceDir = DodgeAngle;
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
            case StateMachine.State.Absorbing:
                if (absorbEffet != null)
                {
                    absorbEffet.transform.position = GrinderControl.position;
                    absorbEffet.transform.rotation = Quaternion.Euler(state.facingAngle, -90, 0);
                    absorbEffet.Play(true);
                }


                //이동
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
            case StateMachine.State.Attacking:
                //이동
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

            case StateMachine.State.Loading:

                //이동
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
                if (circleCollider2D == true) circleCollider2D.enabled = false;
                break;

            case StateMachine.State.Pause:
                xDir = 0;
                yDir = 0;
                speed = 0;
                break;
        }

    }

    private void OnHit(GameObject Attacker, Vector3 AttackLocation, Health.AttackType type)
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
        if (type == Health.AttackType.Normal)
        {
            CameraManager.shakeCamera(10f, 0f - state.facingAngle);

            GameManager.GetInstance().HitStop();
        }
    }

    public void addBullet(int add)
    {
        BulletGauge += add;

        if (BulletGauge > maxBulletGauge)
        {
            BulletGauge = maxBulletGauge;
        }
        if (BulletGauge <= 0)
        {
            BulletGauge = 0;
        }

    }

    private IEnumerator Delay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
