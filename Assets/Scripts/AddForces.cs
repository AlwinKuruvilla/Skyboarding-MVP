using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AddForces : MonoBehaviour
{
    public float moveForce;
    public float moveDampening = 2f;
    public float turnTorque;
    public float pitchTorque;
    public float rollTorque;
    
    [SerializeField] private float _pitchForce = 50f;
    [SerializeField] private float _rollForce = 50f;
    
    private Rigidbody rb;
    private void Start()
    {
        //get hoverboard rb from current gameobject
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //change Yaw
        rb.AddTorque(turnTorque * transform.up, ForceMode.Acceleration);
        
        //change Pitch
        rb.AddTorque(pitchTorque * transform.right * _pitchForce, ForceMode.Force);
        
        //change Roll
        rb.AddTorque(rollTorque * transform.forward * _rollForce, ForceMode.Force);
    }
}
