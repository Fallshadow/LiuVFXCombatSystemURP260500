using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState { Idle, WindUp, Impact, Cooldown }

public class MeeleFighter : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private List<AttackData> attacks;
    
    BoxCollider swordCollider;
    
    private Animator _animator;
    private AttackState attackState;
    private bool doCombo;
    private int comboCount = 0;
    
    private void Awake()
    {
        _animator =  GetComponent<Animator>();
    }

    private void Start()
    {
        if (sword != null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();
            swordCollider.enabled = false;
        }
    }

    public bool InAction { get; private set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
        else if (attackState == AttackState.Impact || attackState == AttackState.Cooldown)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        attackState = AttackState.WindUp;
        // 当前动作 百分比 20% 过渡
        _animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        // 等一帧再去获取下一个动作时长才对
        yield return null;
        
        var animState = _animator.GetNextAnimatorStateInfo(1);
        float timer = 0f;
        
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float progressTime = timer / animState.length;

            if (attackState == AttackState.WindUp)
            {
                if (progressTime >= attacks[comboCount].ImpactStartTime)
                {
                    attackState = AttackState.Impact;
                    swordCollider.enabled = true;
                }
            }
            else if(attackState == AttackState.Impact)
            {
                if (progressTime >= attacks[comboCount].ImpactEndTime)
                {
                    attackState = AttackState.Cooldown;
                    swordCollider.enabled = false;
                }
            }
            else if (attackState == AttackState.Cooldown)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                }
            }
            
            yield return null;
        }

        attackState = AttackState.Idle;
        comboCount = 0;
        InAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox" && !InAction)
        {
            StartCoroutine(PlayHitReaction());
        }
    }
    
    IEnumerator PlayHitReaction()
    {
        InAction = true;
        // 当前动作 百分比 20% 过渡
        _animator.CrossFade("Impact", 0.2f);
        // 等一帧再去获取下一个动作时长才对
        yield return null;
        
        var animState = _animator.GetNextAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length * 0.8f);
        InAction = false;
    }
}
