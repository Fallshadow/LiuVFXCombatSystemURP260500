using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState
{
    Idle,
    WindUp,
    Impact,
    Cooldown
}

public class MeeleFighter : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private List<AttackData> attacks;

    BoxCollider swordCollider;
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    private Animator _animator;
    public AttackState AttackState { get; private set; }
    private bool doCombo;
    private int comboCount = 0;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        leftHandCollider = _animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
        rightHandCollider = _animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
        leftFootCollider = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
        rightFootCollider = _animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();

        if (sword != null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();
        }

        DisableAllColliders();
    }

    public bool InAction { get; private set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
        else if (AttackState == AttackState.Impact || AttackState == AttackState.Cooldown)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        AttackState = AttackState.WindUp;
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

            if (AttackState == AttackState.WindUp)
            {
                if (progressTime >= attacks[comboCount].ImpactStartTime)
                {
                    AttackState = AttackState.Impact;
                    EnableHitbox(attacks[comboCount]);
                }
            }
            else if (AttackState == AttackState.Impact)
            {
                if (progressTime >= attacks[comboCount].ImpactEndTime)
                {
                    AttackState = AttackState.Cooldown;
                    DisableAllColliders();
                }
            }
            else if (AttackState == AttackState.Cooldown)
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

        AttackState = AttackState.Idle;
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

    void EnableHitbox(AttackData attack)
    {
        switch (attack.Hitbox)
        {
            case AttackHitbox.LeftFoot:
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
                rightFootCollider.enabled = true;
                break;
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.Sword:
                swordCollider.enabled = true;
                break;
            default:
                break;
        }
    }

    void DisableAllColliders()
    {
        if (swordCollider != null) swordCollider.enabled = false;
        if (leftHandCollider != null) leftHandCollider.enabled = false;
        if (rightHandCollider != null) rightHandCollider.enabled = false;
        if (leftFootCollider != null) leftFootCollider.enabled = false;
        if (rightFootCollider != null) rightFootCollider.enabled = false;
    }
}