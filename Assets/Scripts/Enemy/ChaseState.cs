using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    [SerializeField] private float distanceToStand = 3f;
    
    private EnemyController enemy;
    
    public override void Enter(EnemyController owner)
    {
        enemy =  owner;

        enemy.NavAgent.stoppingDistance = 3f;
    }

    public override void Execute()
    {
        enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        enemy.Animator.SetFloat("MoveAmount", enemy.NavAgent.velocity.magnitude / enemy.NavAgent.speed);
    }

    public override void Exit()
    {
        
    }
}
