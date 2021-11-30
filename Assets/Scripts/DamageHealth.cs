using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;


public class DamageHealth : MonoBehaviour
{
    public Slider healthSliderRef;
    public Image healthImageRef;
    public Collider hitBoxCollider;

    public float healthDecreaseAmount;
    private float m_healthImageFillDecreaseAmount;

    private int m_playerIndexNumber; // index number variable for layer
    
    // Start is called before the first frame update
    void Start()
    {
        if (healthSliderRef == null && healthImageRef == null)
        {
            Debug.LogError("Health bar slider reference not assigned on " + gameObject.name + " under DamageHealth script");
        }

        m_playerIndexNumber = LayerMask.NameToLayer("Structures"); // sets index number to index value of "Structures" layer)
        m_healthImageFillDecreaseAmount = healthDecreaseAmount / 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer.Equals(m_playerIndexNumber)) // checks for to see if the collision was with "Structures" layer object
        {
            if (healthSliderRef != null)
            {
               healthSliderRef.value -= 2; 
               Debug.Log("health damaged - health slider decreased by " + healthDecreaseAmount);
            }
            else
            {
                healthImageRef.fillAmount -= m_healthImageFillDecreaseAmount;
                Debug.Log("health damaged - health slider decreased by " + m_healthImageFillDecreaseAmount);
            }
        }
    }
}
