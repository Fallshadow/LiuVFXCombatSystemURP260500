using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
        Debug.Log("EnemyState Enter ChaseState");
    }

    public override void Execute()
    {
        Debug.Log("EnemyState Execute ChaseState");
    }

    public override void Exit()
    {
        Debug.Log("EnemyState Exit ChaseState");
    }
}
