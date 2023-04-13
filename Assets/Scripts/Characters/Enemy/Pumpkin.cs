using System;
using UnityEngine;
using UnityEngine.AI;

public class Pumpkin : UnitObject
{
    public Transform SpineTransform;

    [SerializeField] Transform target;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float AttackDistance = 2f;

    [Space]
    [SerializeField] float AttackDelay = 2f;
    public float AttackDuration = 0.3f;
    [SerializeField] float AttackTimer;

    private Health playerHealth;
    private NavMeshAgent agent;
    private bool isPlayerInRange;

    [Space]

    public float forceDir;
    public float xDir;
    public float yDir;

    private void Start()
    {
        playerHealth = target.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public override void Update()
    {
        base.Update();
        FollowTarget();

        if (state.CURRENT_STATE == StateMachine.State.Moving)
        {
            speed *= Mathf.Clamp01(new Vector2(xDir, yDir).magnitude);
        }
        speed = Mathf.Max(speed, 0f);
        vx = speed * Mathf.Cos(forceDir * ((float)Math.PI / 180f));
        vy = speed * Mathf.Sin(forceDir * ((float)Math.PI / 180f));

        switch (state.CURRENT_STATE)
        {
            case StateMachine.State.Idle:
                SpineTransform.localPosition = Vector3.zero;
                speed += (0f - speed) / 3f * GameManager.DeltaTime;

                break;

            case StateMachine.State.Moving:
                AttackTimer = 0f;
                if (Time.timeScale == 0f)
                {
                    break;
                }
                forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir));

                state.facingAngle = Utils.GetAngle(base.transform.position, base.transform.position + new Vector3(vx, vy));
                state.LookAngle = state.facingAngle;
                speed += (agent.speed - speed) / 3f * GameManager.DeltaTime;
                break;

            case StateMachine.State.Attacking:
                SpineTransform.localPosition = Vector3.zero;
                forceDir = state.facingAngle;
                AttackTimer += Time.deltaTime;

                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                if (distanceToPlayer > AttackDistance)
                {
                    AttackTimer = 0f;
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
                else if (AttackTimer < 0.1f)
                {
                    state.facingAngle = (forceDir = Utils.GetAngle(Vector3.zero, new Vector3(xDir, yDir)));
                }
                else if (AttackTimer >= AttackDuration && AttackTimer < AttackDuration + AttackDelay)
                {
                    state.CURRENT_STATE = StateMachine.State.Idle;
                }
                else if (AttackTimer >= AttackDuration + AttackDelay)
                {
                    AttackTimer = 0f;
                    state.CURRENT_STATE = StateMachine.State.Attacking;
                }
                break;
        }
    }

    private void FollowTarget()
    {
        if (state.CURRENT_STATE != StateMachine.State.Attacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= detectionRange)
            {
                isPlayerInRange = true;
            }
            else
            {
                isPlayerInRange = false;
            }

            if (isPlayerInRange && state.CURRENT_STATE != StateMachine.State.Attacking)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                xDir = Mathf.Clamp(directionToTarget.x, -1f, 1f);
                yDir = Mathf.Clamp(directionToTarget.y, -1f, 1f);

                agent.SetDestination(target.position);
                state.CURRENT_STATE = StateMachine.State.Moving;
                if (distanceToPlayer <= AttackDistance)
                {
                    state.CURRENT_STATE = StateMachine.State.Attacking;
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                state.ChangeToIdleState();
                agent.isStopped = true;
                xDir = 0f;
                yDir = 0f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }
}