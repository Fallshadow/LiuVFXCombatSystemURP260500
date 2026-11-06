using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleFighter : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator =  GetComponent<Animator>();
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
        yield return new WaitForSeconds(animState.length);
        InAction = false;
    }
}
