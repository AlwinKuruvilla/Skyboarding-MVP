using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpCurrent : MonoBehaviour
{
    [SerializeField] private float _currentForce = 5f;
    private void OnTriggerEnter(Collider other)
    {
        TryGetComponent(out Rigidbody rigidbody);
        Rigidbody rb = rigidbody;

        if (rb != null)
        {
            rb.AddForce(transform.up * Time.deltaTime * _currentForce, ForceMode.Acceleration);
        }
    }
}
