using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularHealthBar : MonoBehaviour
{
    public Image healthBarImage; // Kéo và thả Image từ Inspector
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {

    }

    public void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth;
        healthBarImage.fillAmount = fillAmount;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }
}
