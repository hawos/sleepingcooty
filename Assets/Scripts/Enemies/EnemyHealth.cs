using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    private int maxHealth;
    private HealthBar healthBar;
    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();

        maxHealth = health;
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetHealth((float)health, (float)maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int amount, bool knockback = false, Vector3? knockbackPosition = null, float knockbackSpeed = 5f)
    {
        if(0f >= amount) {
            return;
        }

        AudioController.Instance.EnemyHit();

        health -= amount;

        healthBar.SetHealth((float)health, (float)maxHealth);

        if(0 >= health) {
            EnemySpawner.Instance.Death();
            Destroy(gameObject);
        }

        if(knockback) {
            enemy.Knockback((Vector3)knockbackPosition, knockbackSpeed);
        }
    }
}
