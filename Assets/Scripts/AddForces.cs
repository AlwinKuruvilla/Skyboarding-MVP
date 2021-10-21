using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AddForces : MonoBehaviour
{
    public float moveForce;
    public float turnTorque;
    public float pitchTorque;
    public float rollTorque;
    private Rigidbody hb;
    private void Start()
    {
        //get hoverboard rb from current gameobject
        hb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        hb.AddForce(moveForce * transform.forward);
        
        //change Yaw
        hb.AddTorque(turnTorque * transform.up);
        
        //change Pitch
        hb.AddTorque(pitchTorque * transform.right);
        
        //change Roll
        hb.AddTorque(rollTorque * transform.forward);
    }
}
