using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    public float CurrentHealth;

    [SerializeField] FloatingHealthBar healthBar;   // UI bar above player

    private void Awake()
    {
        // Auto-find floating UI health bar
        if (healthBar == null)
            healthBar = GetComponentInChildren<FloatingHealthBar>();

        if (healthBar != null)
            healthBar.Initialize(transform);
        else
            Debug.LogWarning("FloatingHealthBar missing on Player!");
    }

    void Start()
    {
        CurrentHealth = maxHealth;

        if (healthBar != null)
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);
    }

    // ---------------------------
    //      DAMAGE SYSTEM
    // ---------------------------
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0;

        if (healthBar != null)
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("PLAYER DIED");
        // Option 1: Disable player
        //gameObject.SetActive(false);

        // Option 2: Respawn if needed
        //StartCoroutine(Respawn());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10f);      // same system as bot
            Destroy(collision.gameObject);
        }
    }

    // OPTIONAL respawn system
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);
        CurrentHealth = maxHealth;

        if (healthBar != null)
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);

        gameObject.SetActive(true);
    }
}