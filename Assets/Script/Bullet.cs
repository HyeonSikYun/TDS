using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    public float speed = 20f;  // 총알 속도
    public int damage = 10;    // 총알 데미지
    public GameObject damageTextPrefab;

    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject hitObject)
    {
        // 좀비 태그 확인 또는 좀비 컴포넌트 확인
        if (hitObject.CompareTag("Zombie") || hitObject.GetComponent<Zombie>() != null)
        {
            Health health = hitObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                CreateDamageText(hitObject.transform.position, damage);
            }
            Destroy(gameObject);
        }
    }

    private void CreateDamageText(Vector3 position, int damageAmount)
    {
        if (damageTextPrefab != null)
        {
            // 프리팹에 DamageText 컴포넌트가 있는지 미리 확인
            DamageText checkComponent = damageTextPrefab.GetComponent<DamageText>();

            // 좀비 위치에서 약간 위에 데미지 텍스트 생성
            Vector3 textPosition = position + new Vector3(0, 0.5f, 0);

            // 데미지 텍스트 생성
            GameObject damageTextObj = Instantiate(damageTextPrefab, textPosition, Quaternion.identity);

            // 데미지 값 설정
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            if (damageText != null)
            {
                damageText.SetDamageValue(damageAmount);
            }
        }
    }
}