using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum State
    {
        Idle = 0,
        Moving = 1,
        Attacking = 2,
        Defending = 3,
        Dodging = 4,
        InActive = 5,
        HitLeft = 6,
        HitRight = 7,
        HitRecover = 8,
        CustomAnimation = 9,
        GameOver = 10,
        FinalGameOver = 11,
        Dieing = 12,
        Dead = 13,
        Absorbing = 14,
        Runaway = 15,
        Patrol = 16,
        Loading = 17,
        Pause = 18,
        Jump = 19,
        Farting = 20,
        FartShield = 21,
        Tailing = 22,
        Throwing = 23,
        Hide = 24,
        Hidden = 25,
        Delay = 26,
        InstantKill = 27,
        JumpDelay = 28,
        Stun = 29,
        PhaseChange = 30,
        Appearance = 31,
        Outburst = 32,
        Defend = 33,
        StopMoving = 34,
        Skill = 35,
        Skill2 = 36,
        ultimate = 37,
        Dash = 38,
        DashDelay = 39,
        DefendDelay = 40,
        Landing = 41,
    }

    public delegate void StateChange(State NewState, State PrevState);
    public bool IsPlayer = false;
    public StateChange OnStateChange;
    [SerializeField] private State currentState;
    [SerializeField] private State previousState;
    public float facingAngle;
    public float LookAngle;
    [HideInInspector] public bool isDefending;
    [HideInInspector] public float Timer;

    public State CURRENT_STATE
    {
        get
        { 
            return currentState;
        }
        set
        {
            if (!LockStateChanges)
            {
                Timer = 0f;
                if (OnStateChange != null)
                {
                    OnStateChange(value, currentState);
                }
                previousState = currentState;
                currentState = value;
                if (IsPlayer)
                {
                    Debug.Log(value);
                }
            }
        }
    }

    public State PREVIOUS_STATE
    {
        get
        {
            return previousState;
        }
        set
        {
            if (!LockStateChanges)
            {
                Timer = 0f;
                if (OnStateChange != null)
                {
                    OnStateChange(value, previousState);
                }
                previousState = value;
                if (IsPlayer)
                {
                    Debug.Log(value);
                }
            }
        }

    }

    public bool LockStateChanges { get; set; }

    public void SmoothFacingAngle(float Angle, float Easing)
    {
        facingAngle += Mathf.Atan2(Mathf.Sin((Angle - facingAngle) * ((float)Math.PI / 180f)), Mathf.Cos((Angle - facingAngle) * ((float)Math.PI / 180f))) * 57.29578f / Easing * GameManager.DeltaTime;
    }

    private void LateUpdate()
    {
        facingAngle = Mathf.Repeat(facingAngle, 360f);
        LookAngle = Mathf.Repeat(LookAngle, 360f);
    }

    private void OnDrawGizmos()
    {
        Utils.DrawLine(base.transform.position, base.transform.position + new Vector3(2f * Mathf.Cos(facingAngle * ((float)Math.PI / 180f)), 2f * Mathf.Sin(facingAngle * ((float)Math.PI / 180f))), Color.blue);
        Utils.DrawLine(base.transform.position, base.transform.position + new Vector3(2f * Mathf.Cos(LookAngle * ((float)Math.PI / 180f)), 2f * Mathf.Sin(LookAngle * ((float)Math.PI / 180f))), Color.green);
    }

    private void FacingToLook()
    {
        facingAngle = LookAngle;
    }

    private void LookToFacing()
    {
        LookAngle = facingAngle;
    }

    public void ChangeToHitState(Vector3 attackLocation)
    {
        PREVIOUS_STATE = CURRENT_STATE;
        if (transform.position.x > attackLocation.x)
        {
            CURRENT_STATE = State.HitLeft;
        }
        else
        {
            CURRENT_STATE = State.HitRight;
        }
    }

    public void ChangeToIdleState()
    {
        CURRENT_STATE = State.Idle;
    }

    public void ChangeToPreviousState()
    {
        CURRENT_STATE = PREVIOUS_STATE;
    }
}
