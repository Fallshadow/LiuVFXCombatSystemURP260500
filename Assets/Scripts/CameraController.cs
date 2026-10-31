using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] Vector2 framingOffset = Vector2.zero;
    [SerializeField] float distance = 5;

    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float minVerticalAngle = -45;
    [SerializeField] private float maxVerticalAngle = 45;

    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    private float _invertXVal;
    private float _invertYVal;
    
    
    private float _rotationX;
    private float _rotationY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _invertXVal = (invertX) ? -1 : 1;
        _invertYVal = (invertY) ? -1 : 1;
        
        _rotationX += Input.GetAxis("Mouse Y") * _invertYVal * rotationSpeed;
        _rotationX =  Mathf.Clamp(_rotationX, minVerticalAngle, maxVerticalAngle);
        _rotationY += Input.GetAxis("Mouse X") * _invertXVal * rotationSpeed;
        
        var targetPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);
        var targetRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
        transform.position = targetPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }
}
