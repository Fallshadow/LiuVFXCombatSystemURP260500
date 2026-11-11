using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AICombatStates { Idle, Chase, Circling }

public class CombatMoveState : State<EnemyController>
{
    [SerializeField] private float circlingSpeed = 20f;
    [SerializeField] private float distanceToStand = 3f;
    [SerializeField] private float adjustDistanceThreshold = 1f;
    [SerializeField] private Vector2 idleTimeRange = new Vector2(2f, 5f);
    [SerializeField] private Vector2 circlingTimeRange = new Vector2(3f, 6f);

    [SerializeField, ReadOnly] private float idleCountDownTimer = 0f;
    [SerializeField, ReadOnly] private float circlingCountDownTimer = 0f;
    [SerializeField, ReadOnly] private int circlingDir = 1;// left 1 right -1
    [SerializeField, ReadOnly] private AICombatStates state;
    private EnemyController enemy;
    
    public override void Enter(EnemyController owner)
    {
        enemy =  owner;

        enemy.NavAgent.stoppingDistance = 3f;
    }

    public override void Execute()
    {
        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) >
            distanceToStand + adjustDistanceThreshold)
        {
            StartChase();
        }
        
        if (state == AICombatStates.Idle)
        {
            if (idleCountDownTimer <= 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    StartIdle();
                }
                else
                {
                    StartCircling();
                }
            }
        }
        else if (state == AICombatStates.Chase)
        {
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <=
                distanceToStand + 0.03f)
            {
                StartIdle();
                return;
            }
            enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        else if (state == AICombatStates.Circling)
        {
            if (circlingCountDownTimer <= 0)
            {
                StartIdle();
                return;
            }

            var vecToTarget = enemy.transform.position - enemy.Target.transform.position;
            var rotatedPos = Quaternion.Euler(0, circlingSpeed * circlingDir * Time.deltaTime, 0) * vecToTarget;
            
            enemy.NavAgent.Move(rotatedPos - vecToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);
        }

        if (idleCountDownTimer > 0)
        {
            idleCountDownTimer -= Time.deltaTime;
        }
        
        if (circlingCountDownTimer > 0)
        {
            circlingCountDownTimer -= Time.deltaTime;
        }
    }

    void StartChase()
    {
        state = AICombatStates.Chase;
        enemy.Animator.SetBool("CombatMode", false);
    }

    void StartIdle()
    {
        state = AICombatStates.Idle;
        idleCountDownTimer = Random.Range(idleTimeRange.x, idleTimeRange.y);
        enemy.Animator.SetBool("CombatMode", true);
    }

    void StartCircling()
    {
        state = AICombatStates.Circling;
        enemy.NavAgent.ResetPath();
        circlingCountDownTimer = Random.Range(circlingTimeRange.x, circlingTimeRange.y);

        circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    public override void Exit()
    {
        
    }
}
