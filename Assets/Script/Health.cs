using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("체력 세팅")]
    public float maxHealth = 100f;         // 최대 체력
    public float currentHealth;            // 현재 체력

    void Start()
    {
        currentHealth = maxHealth;

    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
 
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리 함수
    void Die()
    {
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}