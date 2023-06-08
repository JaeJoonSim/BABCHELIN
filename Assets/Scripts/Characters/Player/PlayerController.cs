using System;
using System.Collections;
using System.Collections.Generic;
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
    public SimpleSpineAnimator simpleSpineAnimator;

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

    [Header("ÀÌµ¿ Ã¼Å©")]
    public float forceDir;
    public float speed;
    public float xDir;
    public float yDir;

    [Header("È¸ÇÇ")]
    public bool showDodge = false;
    [DrawIf("showDodge", true)] public float dodgeTime;
    [DrawIf("showDodge", true)] public float dodgeSpeed;
    [DrawIf("showDodge", true)] public float DodgeAngle = 0f;
    [DrawIf("showDodge", true)] public float DodgeDuration = 0.3f;



    [Header("Èí¼ö")]
    public bool showAbsorb = false;

    [DrawIf("showAbsorb", true)] public ParticleSystem absorbEffet;

    [Header("°ø°Ý")]
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


    [Header("±Ã±Ø±â")]
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
        simpleSpineAnimator = GetComponentInChildren<SimpleSpineAnimator>();
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

        if (state.CURRENT_STATE != StateMachine.State.Dodging)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
            speed = Mathf.Max(speed, 0f);
        }

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
                if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    state.CURRENT_STATE = StateMachine.State.Moving;
                }
                break;

            case StateMachine.State.Moving:
                if (Mathf.Abs(xDir) < MinInputForMovement && Mathf.Abs(yDir) < MinInputForMovement)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                    break;
                }
                break;

            case StateMachine.State.Dodging:
                if (UltObj.activeSelf)
                    UltObj.SetActive(false);
                SpineTransform.localPosition = Vector3.zero;

                if (dodgeTime < TotalStatus.dodgeTime.value)
                {
                    speed = dodgeSpeed;
                    dodgeTime += Time.deltaTime;
                    state.facingAngle = forceDir;
                    muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
                    muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;
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

                break;

            case StateMachine.State.Skill:
                speed = 0;
                muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
                muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;
                break;
            case StateMachine.State.Skill2:
                if (!Skills[1].activeSelf)
                {
                    Skills[1].SetActive(true);
                }
                break;
            case StateMachine.State.Attacking:
                break;

            case StateMachine.State.Ultimate:
                speed = 0;
                state.facingAngle = 270f;
                muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
                muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;

                if (simpleSpineAnimator.Track.IsComplete)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
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



        // ÅºÈ¯ È¸º¹
        if (BulletGauge < TotalStatus.bulletMin.value && !IsInvoking("RestoreBullet"))
            InvokeRepeating("RestoreBullet", 0f, TotalStatus.bulletRegenTime.value);

    }

    public void facingAngle()
    {
        if (state.CURRENT_STATE == StateMachine.State.HitLeft ||
               state.CURRENT_STATE == StateMachine.State.HitRight ||
               state.CURRENT_STATE == StateMachine.State.Dodging ||
               state.CURRENT_STATE == StateMachine.State.Skill ||
               state.CURRENT_STATE == StateMachine.State.Ultimate)

        {
            return;
        }


        if (Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
        {
            speed += (TotalStatus.movSpd.value - speed) / 3f * GameManager.DeltaTime;
            forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));
        }
        else
        {
            speed = 0;
            forceDir = DodgeAngle;
        }



        if (PreesAttack)
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
            state.facingAngle = forceDir;
            muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;
            if (BulletUI.activeSelf)
            {
                fadeTime += Time.deltaTime;

                if (fadeTime >= 1f)
                {
                    BulletUI.GetComponent<CanvasGroup>().alpha -= (Time.deltaTime * 2);
                }
            }
        }

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

        muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);

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

    public void addItem()
    {
        Debug.Log(ItemStatusAdd.variables["ultRestore"].value);
        ItemStatusAdd.ReSaveFieldsToVariables();
        ItemStatusPercent.ReSaveFieldsToVariables();
        Debug.Log(ItemStatusAdd.variables["ultRestore"].value);
        foreach (var item in TotemManager.Instance.isAdd.Values)
        {
            if (item.Stat1 != "")
            {
                Type valueType = BaseStatus.variables[item.Stat1].GetType();
                if (valueType == typeof(Stat<int>) || valueType == typeof(Stat<float>))
                {
                    if (item.Val1 < 1)
                    {
                        ItemStatusPercent.variables[item.Stat1].value += item.Val1 * 100;
                    }
                    else
                    {
                        ItemStatusAdd.variables[item.Stat1].value += item.Val1;
                    }
                }
                else if (valueType == typeof(Stat<bool>))
                {
                    if (item.Val1 > 0)
                        ItemStatusAdd.variables[item.Stat1].value = true;
                    else
                        ItemStatusAdd.variables[item.Stat1].value = false;
                }
            }
            if (item.Stat2 != "")
            {
                Type valueType = BaseStatus.variables[item.Stat2].GetType();
                if (valueType == typeof(Stat<int>) || valueType == typeof(Stat<float>))
                {
                    if (item.Val2 < 1)
                    {
                        ItemStatusPercent.variables[item.Stat2].value += item.Val2 * 100;
                    }
                    else
                    {
                        ItemStatusAdd.variables[item.Stat2].value += item.Val2;
                    }
                }
                else if (valueType == typeof(Stat<bool>))
                {
                    if (item.Val2 > 0)
                        ItemStatusAdd.variables[item.Stat1].value = true;
                    else
                        ItemStatusAdd.variables[item.Stat1].value = false;
                }
            }
        }
    }

    private IEnumerator Delay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
