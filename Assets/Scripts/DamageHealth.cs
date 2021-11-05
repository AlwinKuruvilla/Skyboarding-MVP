using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageHealth : MonoBehaviour
{
    public Slider healthSliderRef;
    public Collider hitBoxCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        if (healthSliderRef == null)
        {
            Debug.LogError("Health bar slider reference not assigned on " + gameObject.name + " under DamageHealth script");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        healthSliderRef.value -= 5;
    }
}
