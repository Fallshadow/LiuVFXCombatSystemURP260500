using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);
    public static EnemyManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private List<EnemyController> enemiesInRange = new List<EnemyController>();
    private float notAttackingTimer = 2f;

    public void AddEnemyInRange(EnemyController enemy)
    {
        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    private void Update()
    {
        if (enemiesInRange.Count == 0) return;
        
        if (!enemiesInRange.Any(e => e.IsInState(EnemyState.Attack)))
        {
            if (notAttackingTimer > 0)
            {
                notAttackingTimer -= Time.deltaTime;
            }
            else
            {
                var attackingEnemy = SelectEnemyForAttack();
                attackingEnemy.ChangeState(EnemyState.Attack);
                notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x, timeRangeBetweenAttacks.y);
            }
        }
    }

    EnemyController SelectEnemyForAttack()
    {
        return enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
    }
}
