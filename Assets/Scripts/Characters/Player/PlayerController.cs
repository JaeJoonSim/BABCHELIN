using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : BaseMonoBehaviour
{
    [HideInInspector] UnitObject unitObject;
    private StateMachine state;
    public StateMachine State
    {
        get { return state; }
        set { state = value; }
    }
    private CircleCollider2D circleCollider2D;
    private Health health;

    public Transform SpineTransform;

    [Header("BaseStatus")]
    public status BaseStatus;
    [Header("ItemStatus")]
    public status ItemStatusAdd;
    public status ItemStatusPercent;
    [Header("BuffStatus")]
    public status BuffStatus;
    [Header("TotalStatus")]
    public status TotalStatus;


    public static float MinInputForMovement = 0.3f;

    //[Header("이동 체크")]
    [HideInInspector] public float forceDir;
    [HideInInspector] public float speed;
    [HideInInspector] public float xDir;
    [HideInInspector] public float yDir;

    [Header("회피")]
    public bool showDodge = false;
    [DrawIf("showDodge", true)] public float DodgeTimer;
    [DrawIf("showDodge", true)] public float DodgeAngle = 0f;
    [DrawIf("showDodge", true)] public float DodgeDuration = 0.3f;
    [DrawIf("showDodge", true)] public float DodgeMaxDuration = 0.5f;
    [DrawIf("showDodge", true)] private float DodgeCollisionDelay;

    [Header("흡수")]
    public bool showAbsorb = false;

    [DrawIf("showAbsorb", true)] public ParticleSystem absorbEffet;

    [Header("공격")]
    public bool showAttack = false;
    [DrawIf("showAttack", true)] public int BulletGauge;

    [DrawIf("showAttack", true)] public GameObject Attack;
    [DrawIf("showAttack", true)] public int SkillIndex;
    public GameObject[] Skills;

    public Transform muzzle;
    public Transform muzzleEnd;
    public Transform muzzleBone;
    public Transform GrinderControl;


    public GameObject BulletUI;
    private float fadeTime = 0;

    private float VZ;
    private float Z;

    private void Start()
    {
        getTotalstatus();
        unitObject = base.gameObject.GetComponent<UnitObject>();
        state = base.gameObject.GetComponent<StateMachine>();
        circleCollider2D = base.gameObject.GetComponent<CircleCollider2D>();
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
        getTotalstatus();
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver || state.CURRENT_STATE == StateMachine.State.Pause || state.CURRENT_STATE == StateMachine.State.Dead)
        {
            SpineTransform.localPosition = Vector3.zero;
            speed += (0f - speed) / 3f * GameManager.DeltaTime;
            return;
        }

        speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        speed = Mathf.Max(speed, 0f);
        unitObject.vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        unitObject.vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        facingAngle();


        if (absorbEffet != null && state.CURRENT_STATE != StateMachine.State.Absorbing)
        {
            absorbEffet.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (state.CURRENT_STATE != StateMachine.State.Skill2 && Skills[1].activeSelf)
        {
            Skills[1].SetActive(false);
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
                speed += (TotalStatus.movSpd - speed) / 3f * GameManager.DeltaTime;
                break;

            case StateMachine.State.Dodging:
                Z = 0f;
                SpineTransform.localPosition = Vector3.zero;
                forceDir = DodgeAngle;
                if (DodgeCollisionDelay < 0f)
                {
                    speed = Mathf.Lerp(speed, TotalStatus.dodgeSpeed, 2f * Time.deltaTime);
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
                    //absorbEffet.transform.position = new Vector3(GrinderControl.position.x, GrinderControl.position.y, -0.3f);
                    //absorbEffet.transform.rotation = Quaternion.Euler(state.facingAngle, -90, 0);
                    absorbEffet.Play(true);
                }

                //이동
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd - speed) / 3f * GameManager.DeltaTime;
                }
                else
                {
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                }
                break;

            case StateMachine.State.Skill:
                Z = 0f;
                SpineTransform.localPosition = Vector3.zero;
                speed += (0f - speed) / 3f * GameManager.DeltaTime;
                break;
            case StateMachine.State.Skill2:
                //이동
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd - speed) / 3f * GameManager.DeltaTime;
                }
                else
                {
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                }

                if (!Skills[1].activeSelf)
                {
                    Skills[1].SetActive(true);
                }
                //Skills[1].transform.position = new Vector3(GrinderControl.position.x, GrinderControl.position.y, -0.3f);
                //Skills[1].transform.rotation = Quaternion.Euler(0, 0, state.facingAngle);

                break;
            case StateMachine.State.Attacking:
                //이동
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd - speed) / 3f * GameManager.DeltaTime;
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
                    speed += (TotalStatus.movSpd - speed) / 3f * GameManager.DeltaTime;
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



        // 탄환 회복
        if (BulletGauge < TotalStatus.bulletMin && !IsInvoking("RestoreBullet"))
            InvokeRepeating("RestoreBullet", 0f, TotalStatus.bulletRegenTime);

    }

    private void facingAngle()
    {
        if (state.CURRENT_STATE != StateMachine.State.Dodging &&
            (state.CURRENT_STATE == StateMachine.State.Attacking ||
            state.CURRENT_STATE == StateMachine.State.Absorbing ||
            state.CURRENT_STATE == StateMachine.State.Skill ||
            state.CURRENT_STATE == StateMachine.State.Skill2
            ))
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
                    DodgeAngle = 270;
                }
            }
            else
            {
                if (xDir < 0)
                {
                    DodgeAngle = 180;
                }
                else if (xDir > 0)
                {
                    DodgeAngle = 0;
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
            state.facingAngle = DodgeAngle;
            muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;
        }
        muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
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

    private void getTotalstatus()
    {
        //var total = struct2cell(BaseStatus);
        //TotalStatus.hpMax = BaseStatus.hpMax + ItemStatusAdd.hpMax +(BaseStatus.hpMax / 100  * ItemStatusPercent.hpMax);
        //TotalStatus.hpRegen = BaseStatus.hpRegen + ItemStatusAdd.hpRegen + (BaseStatus.hpRegen / 100 * ItemStatusPercent.hpRegen);
        //TotalStatus.def = BaseStatus.def + ItemStatusAdd.def + (BaseStatus.def / 100 * ItemStatusPercent.def);
        Type structType = typeof(status);
        FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(int))
            {
                int valueA = (int)field.GetValue(BaseStatus);

                int valueB = (int)field.GetValue(ItemStatusAdd);
                int valueC = (int)field.GetValue(ItemStatusPercent);
                int sum = valueA + valueB + (valueA / 100 * valueC);

                field.SetValueDirect(__makeref(TotalStatus), sum);

            }
            else if (field.FieldType == typeof(float))
            {
                float valueA = (float)field.GetValue(BaseStatus);

                float valueB = (float)field.GetValue(ItemStatusAdd);
                float valueC = (float)field.GetValue(ItemStatusPercent);
                float sum = valueA + valueB + (valueA / 100 * valueC);
                field.SetValueDirect(__makeref(TotalStatus), sum);
            }
        }
    }

    public void addBullet(int add)
    {
        BulletGauge += add;

        if (BulletGauge > TotalStatus.bulletMax)
        {
            BulletGauge = TotalStatus.bulletMax;
        }
        if (BulletGauge <= 0)
        {
            BulletGauge = 0;
        }

    }

    private void RestoreBullet()
    {
        
        BulletGauge += TotalStatus.bulletRegen;

        if (BulletGauge > TotalStatus.bulletMin)
        {
            Debug.Log("cnrk");
            BulletGauge = TotalStatus.bulletMin;
            CancelInvoke("RestoreBullet");
        }
    }

    private IEnumerator Delay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
