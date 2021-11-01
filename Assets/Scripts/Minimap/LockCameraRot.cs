using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// copied from Ben's XRB week 7 module
public class LockCameraRot : MonoBehaviour
{
    [SerializeField] private Transform _rotationRef;

    private Vector3 _initialRotation;
    private Vector3 _initialYPosition;

    private void Start()
    {
        _initialRotation = transform.eulerAngles;
        _initialYPosition = new Vector3(0, transform.position.y, 0);
    }

    private void LateUpdate()
    {
        Vector3 newPos = _rotationRef.transform.position + _initialYPosition;
        transform.position = newPos;
        
        Vector3 newRot = new Vector3(0, 0, -_rotationRef.eulerAngles.y) + _initialRotation;
        transform.rotation = Quaternion.Euler(newRot);
    }
}
