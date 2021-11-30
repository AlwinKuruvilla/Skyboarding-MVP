using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image healthImage;
    public float maxHealth;
    public double healthFill; 

    private void Start()
    {
        healthSlider = this.gameObject.GetComponent<Slider>();
        if (healthSlider == null && healthImage == null)
        {
            Debug.LogError("health bar graphic not defined in HealthBar.cs on " + gameObject.name);
        }

        if (maxHealth > 100 || maxHealth < 0)
        {
            Debug.LogError("max health pn HealthBar.cs on " + gameObject.name + " should be no more than 100 and no less than 0");
        }

        SetMaxHealth(maxHealth);
    }

    public void SetMaxHealth (float health) // sets max slider value of health bar
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthImage.fillAmount = health;
    }

    public void SetHealth (float health) // sets slider value of health bar
    {
        healthSlider.value = health;
        healthImage.fillAmount = health;
    }
}
