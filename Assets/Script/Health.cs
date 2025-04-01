using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // UI.Text ���
using TMPro; // TextMeshPro ��� (�ؽ�Ʈ ������Ʈ�� ������ ���� ����)

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;         // �ִ� ü��
    public float currentHealth;            // ���� ü��

    [Header("Visual Feedback")]
    public GameObject damageEffect;        // ������ �ð� ȿ�� (���� ����)
    public GameObject deathEffect;         // ��� �ð� ȿ�� (���� ����)

    [Header("Events")]
    public UnityEvent onDamage;            // ������ ���� �� �̺�Ʈ
    public UnityEvent onDeath;             // ��� �� �̺�Ʈ

    void Start()
    {
        // ���� �� �ִ� ü������ �ʱ�ȭ
        currentHealth = maxHealth;

    }

    // ������ ó�� �Լ�
    public void TakeDamage(float damageAmount)
    {
        // ü�� ����
        currentHealth -= damageAmount;
        // ������ ����Ʈ ����
        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // ������ �̺�Ʈ ȣ��
        if (onDamage != null)
            onDamage.Invoke();

        // ü���� 0 ���ϸ� ��� ó��
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ��� ó�� �Լ�
    void Die()
    {
        // ��� ����Ʈ ����
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // ��� �̺�Ʈ ȣ��
        if (onDeath != null)
            onDeath.Invoke();

        // ������Ʈ �ı�
        Destroy(gameObject);
    }

    // ���� ü�� ���� ��ȯ (0.0 ~ 1.0)
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}