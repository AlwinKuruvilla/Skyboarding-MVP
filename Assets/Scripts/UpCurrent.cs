using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpCurrent : MonoBehaviour
{
    [SerializeField] private float _currentForce = 5f;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            Vector3 velocity = rb.velocity;
            velocity += transform.up * Time.deltaTime * _currentForce;
            rb.velocity = velocity;
            
            Debug.Log(rb.gameObject.name);
        }
    }
}
