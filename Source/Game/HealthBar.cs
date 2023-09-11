using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFiller;

    public int maxHealth, currentHealth;
    public float invicibilityTime;
    float invicibilityCooldown;

    public GameObject graphics;

    Animator animator;
    Player player;

    
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        invicibilityCooldown -= Time.deltaTime;

        if (invicibilityCooldown > 0 && invicibilityCooldown % 0.4f < 0.2f)
            graphics.SetActive(false);
        else
            graphics.SetActive(true);

    }

    public void TakeDamage(int damage)
    {
        if (invicibilityCooldown > 0) return;
        
        if (gameObject.tag == "Player")
            if (player.isDashing || player.lerpSpeed >= 0.1f)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }

        healthFiller.fillAmount = (float)currentHealth / (float)maxHealth;

        invicibilityCooldown = invicibilityTime;
    }

    public void HealDamagePercent(float percent) 
    {
        currentHealth += (int)((float)maxHealth * percent);
        healthFiller.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    private void Death()
    {
        animator.SetTrigger("Dead");
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

}
