using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState { Idle, WindUp, Impact, Cooldown }

public class MeeleFighter : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    BoxCollider swordCollider;
    
    private Animator _animator;
    private AttackState attackState;
    
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
            InAction = true;
            
            _animator.CrossFade("Slash", 0.2f);
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        // 当前动作 百分比 20% 过渡
        _animator.CrossFade("Slash", 0.2f);
        // 等一帧再去获取下一个动作时长才对
        yield return null;
        
        var animState = _animator.GetNextAnimatorStateInfo(1);


        float impactStartTime = 0.33f;
        float impactEndTime = 0.55f;
        float timer = 0f;
        attackState = AttackState.WindUp;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float progressTime = timer / animState.length;

            if (attackState == AttackState.WindUp)
            {
                if (progressTime >= impactStartTime)
                {
                    attackState = AttackState.Impact;
                    swordCollider.enabled = true;
                }
            }
            else if(attackState == AttackState.Impact)
            {
                if (progressTime >= impactEndTime)
                {
                    attackState = AttackState.Cooldown;
                    swordCollider.enabled = false;
                }
            }
            else if (attackState == AttackState.Cooldown)
            {
                
            }
            
            yield return null;
        }
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
        yield return new WaitForSeconds(animState.length);
        InAction = false;
    }
}
