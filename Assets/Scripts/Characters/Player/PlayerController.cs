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
    public Health health;

    public PlayerSound playerSound;

    public Transform SpineTransform;
    public SimpleSpineAnimator simpleSpineAnimator;

    private new CameraFollowTarget camera;

    public BuffUI buffUI;

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

    [Header("이동 체크")]
    public float forceDir;
    public float speed;
    public float xDir;
    public float yDir;

    [Header("회피")]
    public bool showDodge = false;
    [DrawIf("showDodge", true)] public float dodgeTime;
    [DrawIf("showDodge", true)] public float dodgeSpeed;
    [DrawIf("showDodge", true)] public float DodgeAngle = 0f;
    [DrawIf("showDodge", true)] public float DodgeDuration = 0.3f;
    [DrawIf("showDodge", true)] public float DodgeDelay;


    [Header("흡수")]
    public bool showAbsorb = false;

    [DrawIf("showAbsorb", true)] public ParticleSystem absorbEffet;

    [Header("공격")]
    public bool showAttack = false;
    [DrawIf("showAttack", true)] public int BulletGauge;
    [DrawIf("showAttack", true)] public bool PreesAttack;
    [DrawIf("showAttack", true)] public GameObject Attack;
    [DrawIf("showAttack", true)] public int SkillIndex;
    [DrawIf("showAttack", true)] private bool BulletUnlimit = false;
    public bool Heal = false;
    public GameObject[] Skills;

    [Header("스킬")]
    public bool showSkill = false;
    [DrawIf("showSkill", true)] public float skill1CurCooltime;
    [DrawIf("showSkill", true)] public float skill2CurCooltime;

    [Header("피격")]
    public float hitDelay;
    private float curHitDelay;


    [Header("muzzle")]
    public bool showMuzzle = false;
    [DrawIf("showMuzzle", true)] public Transform muzzle;
    [DrawIf("showMuzzle", true)] public Transform muzzleEnd;
    [DrawIf("showMuzzle", true)] public Transform muzzleBone;
    [DrawIf("showMuzzle", true)] public Transform GrinderControl;


    [Header("궁극기")]
    public bool showUltimate = false;
    [DrawIf("showUltimate", true)] public GameObject UltObj;
    [DrawIf("showUltimate", true)] public int UltIdx;
    [DrawIf("showUltimate", true)] public float UltGauge;
    [DrawIf("showUltimate", true)] public ParticleSystem UltEffet;

    public GameObject BulletUI;
    private float fadeTime = 0;

    [Header("스파인 체크용")]
    public bool inSpineEvent = false;
    public bool inCameraWork = false;


    private void Start()
    {
        BaseStatus.SaveFieldsToVariables();
        ItemStatusAdd.SaveFieldsToVariables();
        ItemStatusPercent.SaveFieldsToVariables();
        BuffStatus.SaveFieldsToVariables();
        TotalStatus.SaveFieldsToVariables();
        unitObject = base.gameObject.GetComponent<UnitObject>();
        state = base.gameObject.GetComponent<StateMachine>();
        circleCollider2D = base.gameObject.GetComponent<CircleCollider2D>();
        simpleSpineAnimator = GetComponentInChildren<SimpleSpineAnimator>();
        camera = Camera.main.GetComponent<CameraFollowTarget>();

        getTotalstatus();
        BulletGauge = TotalStatus.bulletMax.value;
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
        if (absorbEffet != null && state.CURRENT_STATE != StateMachine.State.Absorbing)
        {
            absorbEffet.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);           
        }

        //if (IsInvoking("SoundDelay") && state.CURRENT_STATE != StateMachine.State.Skill2)
        //{
        //    CancelInvoke("SoundDelay");
        //    playerSound.StopSound();
        //}

        if (state.CURRENT_STATE != StateMachine.State.Skill2 && Skills[1].activeSelf)
        {
            Skills[1].SetActive(false);
        }


        getTotalstatus();
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver || state.CURRENT_STATE == StateMachine.State.Pause || state.CURRENT_STATE == StateMachine.State.Dead)
        {
            SpineTransform.localPosition = Vector3.zero;
            speed = 0;
            if (UltObj.activeSelf)
            {
                UltObj.SetActive(false);
            }
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

        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Idle:
                inSpineEvent = false;
                SpineTransform.localPosition = Vector3.zero;
                curHitDelay = 0;

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

                if (dodgeTime <= TotalStatus.dodgeTime.value)
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
                if (!IsInvoking("SoundDelay"))
                    Invoke("SoundDelay", 0.2f);
                if (!Skills[1].activeSelf)
                {
                    Skills[1].SetActive(true);
                }
                break;
            case StateMachine.State.Attacking:
                break;
            case StateMachine.State.Ultimate:
                speed = 0;
                DodgeAngle = state.facingAngle = 270f;
                muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);
                muzzleBone.position = transform.position + (muzzleEnd.position - transform.position).normalized;
                health.untouchable = true;
                if (!IsInvoking("endUntouchable"))
                    Invoke("endUntouchable", 3f);
                if (simpleSpineAnimator.Track.IsComplete)
                {
                    Camerawork();
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
                break;
            case StateMachine.State.HitLeft:
            case StateMachine.State.HitRight:
                curHitDelay += Time.deltaTime;

                if (hitDelay < curHitDelay)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
                break;
            case StateMachine.State.Landing:
                if (BuffStatus.atk.value != 0)
                {
                    buffControl(1005, false);
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
        if (BulletGauge < TotalStatus.bulletMin.value && !IsInvoking("RestoreBullet"))
            InvokeRepeating("RestoreBullet", 0f, TotalStatus.bulletRegenTime.value);

    }

    private void SoundDelay()
    {
        if (state.CURRENT_STATE == StateMachine.State.Absorbing)
        {
            playerSound.PlayPlayerSound(playerSound.pcAbsorb);
        }
        else if (state.CURRENT_STATE == StateMachine.State.Skill2)
        {
            playerSound.PlayPlayerSound(playerSound.pcSmallSkill);
        }


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
        muzzle.rotation = Quaternion.Euler(0, 0, state.facingAngle);


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


    }
    private void OnHit(GameObject Attacker, Vector3 AttackLocation, Health.AttackType type)
    {

        if (!health.isInvincible)
        {
            return;
        }

        playerSound.StopSound();

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

    public void addBuff(int idx, bool toggle, float delay = 0.1f)
    {
        playerSound.PlayPlayerSound("pcBuffGet");
        StartCoroutine(buffControl(idx, toggle, delay));
    }
    public IEnumerator buffControl(int idx, bool toggle, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);

        switch (idx)
        {
            case 1001:
                if (toggle) { }
                else { }
                break;
            case 1002:
                if (toggle)
                {
                    buffUI.addBuff(10, idx);
                    BuffStatus.movSpd.value = TotalStatus.movSpd.value / 100 * 10;
                    StartCoroutine(buffControl(1002, false, 10f));
                }
                else
                {
                    BuffStatus.movSpd.value = 0;
                }

                break;
            case 1003:
                if (toggle) { }
                else { }
                break;
            case 1004:
                if (toggle) { }
                else { }
                break;
            case 1005:
                if (toggle)
                {
                    buffUI.addBuff(1000, idx);
                    BuffStatus.atk.value = TotalStatus.atk.value / 100 * 50;
                }
                else
                {
                    buffUI.removeBuff(idx);
                    BuffStatus.atk.value = 0;
                }
                break;
            case 1006:
                if (toggle)
                {
                    buffUI.addBuff(5, idx);
                    BuffStatus.bulletRange.value = TotalStatus.bulletRange.value;
                    StartCoroutine(buffControl(1006, false, 5f));
                }
                else
                {
                    BuffStatus.bulletRange.value = 0;
                }
                break;
            case 1007:
                if (toggle)
                {
                    buffUI.addBuff(5, idx);
                    BulletUnlimit = true;
                    StartCoroutine(buffControl(1007, false, 5f));
                }
                else
                {
                    BulletUnlimit = false;
                }
                break;
            case 1008:
                if (toggle)
                {
                    buffUI.addBuff(5, idx);
                    Heal = true;
                    StartCoroutine(buffControl(1008, false, 5f));
                }
                else
                {
                    Heal = false;
                }
                break;
            case 1009:
                if (toggle) { }
                else { }
                break;
            default:
                break;
        }
    }

    public void addBullet(int add)
    {
        if (add > 0)
        {
            int Gauge = add + (add * (TotalStatus.absorbRestore.value / 100));
            BulletGauge += Gauge;
        }
        else if(add < 0 && !BulletUnlimit)
        {
            BulletGauge += add;
        }
        
        //Debug.Log("Bullet 회복 = " + Gauge);
        if (BulletGauge > TotalStatus.bulletMax.value)
        {
            BulletGauge = TotalStatus.bulletMax.value;
        }
        if (BulletGauge <= 0)
        {
            BulletGauge = 0;
        }

    }

    public void addUltGauge(int add)
    {
        float Gauge = add + (add * (TotalStatus.ultRestore.value / 100));
        UltGauge += Gauge;
        Debug.Log("궁극기 회복 = " + Gauge);
        if (UltGauge > TotalStatus.UltMax.value)
        {
            UltGauge = 100;
        }
        if (UltGauge <= 0)
        {
            UltGauge = 0;
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
    private void endUntouchable()
    {
        health.untouchable = false;
    }
    public void Camerawork()
    {
        if (camera.targets.Count <= 1)
        {

            camera.SnappyMovement = true;
            camera.AddTarget(gameObject, 1f);
            camera.targetDistance = 5f;
            camera.distance = 5f;
        }
        else
        {
            camera.SnappyMovement = true;
            camera.targets.Clear();
            camera.targetDistance = 17f;
            camera.distance = 17f;
            Invoke("EnableCameraWork", 2f);
        }
    }
    private void EnableCameraWork()
    {
        camera.SnappyMovement = false;
    }
    public void addItem()
    {
        playerSound.PlayPlayerSound(playerSound.pcTotemGet);
        //Debug.Log(ItemStatusAdd.variables["ultRestore"].value);
        ItemStatusAdd.ReSaveFieldsToVariables();
        ItemStatusPercent.ReSaveFieldsToVariables();
        //Debug.Log(ItemStatusAdd.variables["ultRestore"].value);
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
