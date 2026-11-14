using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, CombatMove, Attack }

public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public List<MeeleFighter> TargetsInRange { get; set; } = new List<MeeleFighter>();
    public MeeleFighter Target { get; set; }
    public float CombatMovementTimer { get; set; }
    public StateMachine<EnemyController> StateMachine { get; private set; }
    
    private Dictionary<EnemyState, State<EnemyController>> stateDict;
    
    public NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }
    public MeeleFighter Fighter { get; private set; }
    
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Fighter = GetComponent<MeeleFighter>();
        
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle] = GetComponent<EnemyIdleState>();
        stateDict[EnemyState.CombatMove] = GetComponent<EnemyCombatMoveState>();
        stateDict[EnemyState.Attack] = GetComponent<EnemyAttackState>();

        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyState.Idle]);
    }

    private Vector3 prevPos;
    
    private void Update()
    {
        StateMachine.Execute();
        var deltaPos = transform.position - prevPos;
        var velocity = deltaPos /  Time.deltaTime;
        
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        Animator.SetFloat("ForwardSpeed", forwardSpeed / NavAgent.speed, 0.2f, Time.deltaTime);
        
        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        Animator.SetFloat("StrafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

        prevPos = transform.position;
    }

    public void ChangeState(EnemyState state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }

    public bool IsInState(EnemyState state)
    {
        return StateMachine.CurrentState == stateDict[state];
    }
}
