using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // UI.Text 사용
using TMPro; // TextMeshPro 사용 (텍스트 컴포넌트의 종류에 따라 선택)

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;         // 최대 체력
    public float currentHealth;            // 현재 체력

    [Header("Visual Feedback")]
    public GameObject damageEffect;        // 데미지 시각 효과 (선택 사항)
    public GameObject deathEffect;         // 사망 시각 효과 (선택 사항)

    [Header("Events")]
    public UnityEvent onDamage;            // 데미지 받을 때 이벤트
    public UnityEvent onDeath;             // 사망 시 이벤트

    void Start()
    {
        // 시작 시 최대 체력으로 초기화
        currentHealth = maxHealth;

    }

    // 데미지 처리 함수
    public void TakeDamage(float damageAmount)
    {
        // 체력 감소
        currentHealth -= damageAmount;
        // 데미지 이펙트 생성
        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // 데미지 이벤트 호출
        if (onDamage != null)
            onDamage.Invoke();

        // 체력이 0 이하면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리 함수
    void Die()
    {
        // 사망 이펙트 생성
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // 사망 이벤트 호출
        if (onDeath != null)
            onDeath.Invoke();

        // 오브젝트 파괴
        Destroy(gameObject);
    }

    // 현재 체력 비율 반환 (0.0 ~ 1.0)
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}