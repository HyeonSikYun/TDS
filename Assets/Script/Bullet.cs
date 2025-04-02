using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    public float speed = 20f;  // ÃÑ¾Ë ¼Óµµ
    public int damage = 70;    // ÃÑ¾Ë µ¥¹ÌÁö
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
            DamageText checkComponent = damageTextPrefab.GetComponent<DamageText>();

            
            Vector3 textPosition = position + new Vector3(0, 0.5f, 0);

            
            GameObject damageTextObj = Instantiate(damageTextPrefab, textPosition, Quaternion.identity);

            
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            if (damageText != null)
            {
                damageText.SetDamageValue(damageAmount);
            }
        }
    }
}