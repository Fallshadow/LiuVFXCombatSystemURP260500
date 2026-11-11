using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, CombatMove }

public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public List<MeeleFighter> TargetsInRange { get; set; } = new List<MeeleFighter>();
    public MeeleFighter Target { get; set; }
    public StateMachine<EnemyController> StateMachine { get; private set; }
    
    private Dictionary<EnemyState, State<EnemyController>> stateDict;
    
    public NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }
    
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle] = GetComponent<IdleState>();
        stateDict[EnemyState.CombatMove] = GetComponent<CombatMoveState>();

        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyState.Idle]);
    }

    private void Update()
    {
        StateMachine.Execute();
        Animator.SetFloat("MoveAmount", NavAgent.velocity.magnitude / NavAgent.speed);
    }

    public void ChangeState(EnemyState state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }
}
