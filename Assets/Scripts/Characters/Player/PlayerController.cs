using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
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

    [Header("�̵� üũ")]
    public float forceDir;
    public float speed;
    [HideInInspector] public float xDir;
    [HideInInspector] public float yDir;

    [Header("ȸ��")]
    public bool showDodge = false;
    [DrawIf("showDodge", true)] public float dodgeTime;
    [DrawIf("showDodge", true)] public float dodgeSpeed;
    [DrawIf("showDodge", true)] public float DodgeAngle = 0f;
    [DrawIf("showDodge", true)] public float DodgeDuration = 0.3f;



    [Header("����")]
    public bool showAbsorb = false;

    [DrawIf("showAbsorb", true)] public ParticleSystem absorbEffet;

    [Header("����")]
    public bool showAttack = false;
    [DrawIf("showAttack", true)] public int BulletGauge;
    [DrawIf("showAttack", true)] public bool PreesAttack;
    [DrawIf("showAttack", true)] public GameObject Attack;
    [DrawIf("showAttack", true)] public int SkillIndex;
    public GameObject[] Skills;


    [Header("muzzle")]
    public bool showMuzzle = false;
    [DrawIf("showMuzzle", true)] public Transform muzzle;
    [DrawIf("showMuzzle", true)] public Transform muzzleEnd;
    [DrawIf("showMuzzle", true)] public Transform muzzleBone;
    [DrawIf("showMuzzle", true)] public Transform GrinderControl;


    [Header("�ñر�")]
    public bool showUltimate = false;
    [DrawIf("showUltimate", true)] public GameObject UltObj;
    [DrawIf("showUltimate", true)] public int UltIdx;

    public GameObject BulletUI;
    private float fadeTime = 0;

    private void Start()
    {
        BaseStatus.SaveFieldsToVariables();
        ItemStatusAdd.SaveFieldsToVariables();
        ItemStatusPercent.SaveFieldsToVariables();
        BuffStatus.SaveFieldsToVariables();
        TotalStatus.SaveFieldsToVariables();
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
        //getTotalstatus();
        //Debug.Log((TotalStatus.hpMax.value));
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver || state.CURRENT_STATE == StateMachine.State.Pause || state.CURRENT_STATE == StateMachine.State.Dead)
        {
            SpineTransform.localPosition = Vector3.zero;
            speed += (0f - speed) / 3f * GameManager.DeltaTime;
            return;
        }

        if (state.CURRENT_STATE != StateMachine.State.Dodging)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }
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
                speed += (TotalStatus.movSpd.value - speed) / 3f * GameManager.DeltaTime;
                break;

            case StateMachine.State.Dodging:
                SpineTransform.localPosition = Vector3.zero;

                if (dodgeTime < TotalStatus.dodgeTime.value)
                {
                    forceDir = DodgeAngle;
                    speed = dodgeSpeed;
                    dodgeTime += Time.deltaTime;
                }
                else
                {
                    dodgeTime = 0;
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

                //�̵�
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd.value - speed) / 3f * GameManager.DeltaTime;
                }
                else
                {
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                }
                break;

            case StateMachine.State.Skill:
                SpineTransform.localPosition = Vector3.zero;
                speed += (0f - speed) / 3f * GameManager.DeltaTime;
                break;
            case StateMachine.State.Skill2:
                //�̵�
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd.value - speed) / 3f * GameManager.DeltaTime;
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
                //�̵�
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd.value - speed) / 3f * GameManager.DeltaTime;
                }
                else
                {
                    speed += (0f - speed) / 3f * GameManager.DeltaTime;
                }
                break;

            case StateMachine.State.Loading:

                //�̵�
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
                    state.LookAngle = state.facingAngle;
                    speed += (TotalStatus.movSpd.value - speed) / 3f * GameManager.DeltaTime;
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



        // źȯ ȸ��
        if (BulletGauge < TotalStatus.bulletMin.value && !IsInvoking("RestoreBullet"))
            InvokeRepeating("RestoreBullet", 0f, TotalStatus.bulletRegenTime.value);

    }

    public void facingAngle()
    {
        if (PreesAttack)
        {
            muzzleBone.position = Utils.GetMousePosition();
            state.facingAngle = Utils.GetMouseAngle(transform.position);
            muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);

            if (45 <= state.facingAngle && state.facingAngle <= 135)
                DodgeAngle = 90f;
            else if (225 <= state.facingAngle && state.facingAngle <= 315)
                DodgeAngle = 270f;
            else if (135 < state.facingAngle && state.facingAngle < 225)
            {
                DodgeAngle = 180f;
            }
            else if (state.facingAngle < 45 || state.facingAngle > 315)
            {
                DodgeAngle = 0f;
            }

            if (BulletUI.GetComponent<CanvasGroup>().alpha < 1)
            {
                fadeTime = 0;
                BulletUI.GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        else if (state.CURRENT_STATE == StateMachine.State.HitLeft ||
            state.CURRENT_STATE == StateMachine.State.HitRight)
        {
            return;
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
            state.facingAngle = DodgeAngle;
            muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
            muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;
        }

    }
    private void OnHit(GameObject Attacker, Vector3 AttackLocation, Health.AttackType type)
    {

        if (!health.isInvincible)
        {
            return;
        }



        if (forceDir == 270 || forceDir == 90)
        {
            if (Attacker == null)
            {
                state.facingAngle = Utils.GetAngle(base.transform.position, AttackLocation);
            }
            else
            {
                state.facingAngle = Utils.GetAngle(base.transform.position, Attacker.transform.position);
            }
        }

        if (90 < state.facingAngle && state.facingAngle < 270)
        {
            state.facingAngle = 180;
        }
        else if (state.facingAngle < 90 || state.facingAngle > 270)
        {
            state.facingAngle = 0;
        }

        forceDir = state.facingAngle + 180f;

        muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
        muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;


        if (type == Health.AttackType.Normal)
        {
            CameraManager.shakeCamera(10f, 0f - state.facingAngle);

            GameManager.GetInstance().HitStop();
        }
    }

    private void getTotalstatus()
    {
        foreach (KeyValuePair<string, dynamic> total in BaseStatus.variables)
        {
            if (total.Key != "variables")
            {
                Type valueType = BaseStatus.variables[total.Key].GetType();
                if (valueType == typeof(Stat<int>) || valueType == typeof(Stat<float>))
                {
                    TotalStatus.variables[total.Key].value = 
                        BaseStatus.variables[total.Key].value
                        + ItemStatusAdd.variables[total.Key].value
                        + (BaseStatus.variables[total.Key].value / 100 * ItemStatusPercent.variables[total.Key].value);
                }
                else if (valueType == typeof(Stat<bool>))
                {
                    TotalStatus.variables[total.Key].value =
                        BaseStatus.variables[total.Key].value ||
                        ItemStatusAdd.variables[total.Key].value ||
                        ItemStatusPercent.variables[total.Key].value;
                }
            }
        }
    }

    public void addBullet(int add)
    {
        BulletGauge += add;

        if (BulletGauge > TotalStatus.bulletMax.value)
        {
            BulletGauge = TotalStatus.bulletMax.value;
        }
        if (BulletGauge <= 0)
        {
            BulletGauge = 0;
        }

    }

    private void RestoreBullet()
    {

        BulletGauge += TotalStatus.bulletRegen.value;

        if (BulletGauge > TotalStatus.bulletMin.value)
        {
            BulletGauge = TotalStatus.bulletMin.value;
            CancelInvoke("RestoreBullet");
        }
    }

    public void addUltIdx()
    {
        UltIdx++;
        UltIdx = UltIdx % 2;
    }

    private IEnumerator Delay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
