using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { Idle, Chase }

public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public List<MeeleFighter> TargetsInRange { get; set; } = new List<MeeleFighter>();
    public MeeleFighter Target { get; set; }
    public StateMachine<EnemyController> StateMachine { get; private set; }

    private Dictionary<EnemyState, State<EnemyController>> stateDict;

    private void Start()
    {
        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle] = GetComponent<IdleState>();
        stateDict[EnemyState.Chase] = GetComponent<ChaseState>();

        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyState.Idle]);
    }

    private void Update()
    {
        StateMachine.Execute();
    }

    public void ChangeState(EnemyState state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }
}
