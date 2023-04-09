using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseMonoBehaviour
{
    private StateMachine state;
    private CircleCollider2D circleCollider2D;

    public Transform SpineTransform;

    [HideInInspector] public float defaultRunSpeed = 5.5f;
    public static float MinInputForMovement = 0.3f;

    public float forceDir;
    public float speed;
    public float xDir;
    public float yDir;

    private float VZ;
    private float Z;

    private void Start()
    {
        state = base.gameObject.GetComponent<StateMachine>();
        circleCollider2D = base.gameObject.GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f && state.CURRENT_STATE != StateMachine.State.GameOver && state.CURRENT_STATE != StateMachine.State.FinalGameOver)
        {
            return;
        }

        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Idle:
                Z = 0f;
                SpineTransform.localPosition = Vector3.zero;
                speed += (0f - speed) / 3f * GameManager.DeltaTime;
                if(Mathf.Abs(xDir) > MinInputForMovement || Mathf.Abs(yDir) > MinInputForMovement)
                {
                    state.CURRENT_STATE = StateMachine.State.Moving;
                }
                break;
            case StateMachine.State.Moving:
                if(Time.timeScale == 0f)
                {
                    break;
                }
                forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));

                break;
        }

    }
}
