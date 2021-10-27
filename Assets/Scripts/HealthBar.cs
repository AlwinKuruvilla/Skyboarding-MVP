using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;

    private void Start()
    {
        healthSlider = this.gameObject.GetComponent<Slider>();
        if (healthSlider == null)
        {
            Debug.LogError("health bar slider not defined on HealthBar.cs");
        }
    }

    public void SetMaxHealth (int health) // sets max slider value of health bar
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void SetHealth (int health) // sets slider value of health bar
    {
        healthSlider.value = health;
    }
}
