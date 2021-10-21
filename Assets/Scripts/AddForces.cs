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
    private Rigidbody rb;
    private void Start()
    {
        //get hoverboard rb from current gameobject
        rb = GetComponent<Rigidbody>();

        //booster
        rb.AddForce(2f*transform.forward, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        //changing velocity directly instead of using addforce allows us to keep the rigidbody drag numbers higher
        //copied from here: https://forum.unity.com/threads/rigidbody-floating-in-airstream.118052/
        Vector3 velocity = rb.velocity;
        velocity += transform.forward * moveForce * Time.deltaTime; // dir = fan direction, ie. transform.up or whatever setup you have there
        velocity -= velocity * moveDampening * Time.deltaTime; // add dampening so that velocity doesn't get out of hand
        rb.velocity = velocity;

        //change Yaw
        rb.AddTorque(turnTorque * transform.up, ForceMode.Impulse);
        
        //change Pitch
        rb.AddTorque(pitchTorque * transform.right, ForceMode.Impulse);
        
        //change Roll
        rb.AddTorque(rollTorque * transform.forward, ForceMode.Impulse);
    }
}
