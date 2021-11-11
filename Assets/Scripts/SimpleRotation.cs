using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [Header("Rotation Speeds")] 
    [Tooltip("the speed of rotation on the x axis; set to 0 for no rotation")] public float xAxisRotationSpeed = 0;
    [Tooltip("the speed of rotation on the y axis; set to 0 for no rotation")] public float yAxisRotationSpeed = 0;
    [Tooltip("the speed of rotation on the z axis; set to 0 for no rotation")] public float zAxisRotationSpeed = 0;

    public GameObject objectToRotateRef;
    
    void FixedUpdate()
    {
        objectToRotateRef.transform.Rotate(xAxisRotationSpeed, yAxisRotationSpeed, zAxisRotationSpeed);
    }
}
