using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageHealth : MonoBehaviour
{
    public Slider healthSliderRef;
    public Collider hitBoxCollider;

    private int _layerIndexNumber; // index number variable for layer
    
    // Start is called before the first frame update
    void Start()
    {
        if (healthSliderRef == null)
        {
            Debug.LogError("Health bar slider reference not assigned on " + gameObject.name + " under DamageHealth script");
        }

        _layerIndexNumber = LayerMask.NameToLayer("Structures"); // sets index number to index value of "Structures" layer)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer.Equals(_layerIndexNumber)) // checks for to see if the collision was with "Structures" layer object
        {
           healthSliderRef.value -= 2; 
        }
    }
}
