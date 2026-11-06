using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 500f;
    
    [Header("Ground Check Group")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    
    float ySpeed;
    
    Quaternion targetRotation;
    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    
    MeeleFighter meeleFighter;
    
    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        meeleFighter =  GetComponent<MeeleFighter>();
    }

    void Update()
    {
        if (meeleFighter.InAction)
        {
            animator.SetFloat("MoveAmount", 0);
            return;
        }
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.PlanarRotation * moveInput;

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        GroundCheck();
        if (isGrounded)
        {
            // 贴紧地面
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
        var velocity = moveDir * moveSpeed;
        velocity.y = ySpeed;
        
        
        characterController.Move(velocity * Time.deltaTime);
        
        if (moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        animator.SetFloat("MoveAmount", moveAmount, 0.2f, Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
