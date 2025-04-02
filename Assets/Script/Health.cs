using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("ü�� ����")]
    public float maxHealth = 100f;         // �ִ� ü��
    public float currentHealth;            // ���� ü��

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

    // ��� ó�� �Լ�
    void Die()
    {
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}