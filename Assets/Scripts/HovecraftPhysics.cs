using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovecraftPhysics : MonoBehaviour
{
    [SerializeField] private List<Transform> _raycastSpheres;
    float hoverHeight = 4.0f;
    float hoverForce = 1000.0f;
    float hoverDamp = 10f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        foreach (var sphere in _raycastSpheres)
        {
            RaycastHit hit;
            Ray downRay = new Ray(sphere.position, Vector3.down);
            
            Debug.DrawRay(sphere.position, Vector3.down, Color.red);
            
            if (Physics.Raycast(downRay, out hit))
            {
                float hoverError = hoverHeight - hit.distance;

                if (hoverError > 0)
                {
                    float upwardSpeed = rb.velocity.y;
                    float lift = hoverForce - upwardSpeed * hoverDamp;
                    rb.AddForceAtPosition(Vector3.up * lift / hit.distance, sphere.position);   
                }
            }
        }
    }
}
