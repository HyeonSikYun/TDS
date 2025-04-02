using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float fadeSpeed = 0.5f;
    public float moveSpeed = 2.0f;
    public float lifetime = 1.0f;
    public float arcHeight = 2.0f; // 포물선의 높이
    public TextMeshProUGUI textComponent;

    private Color originalColor;
    private Vector3 randomDirection;

    void Awake()
    {
        originalColor = textComponent.color;
    }

    void Start()
    {
        float randomAngle = Random.Range(0, 2f * Mathf.PI);
        randomDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;

        StartCoroutine(MoveInArc());
    }

    public void SetDamageValue(int damage)
    {
        if (textComponent != null)
        {
            textComponent.text = damage.ToString();
        }
        
    }

    private IEnumerator MoveInArc()
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / lifetime;

            float verticalPosition = arcHeight * 4 * normalizedTime * (1 - normalizedTime);

            Vector3 horizontalMovement = randomDirection * moveSpeed * normalizedTime;

            transform.position = startPosition + horizontalMovement + new Vector3(0, verticalPosition, 0);

            if (textComponent != null)
            {
                float alpha;
                if (normalizedTime < 0.5f)
                {                  
                    alpha = Mathf.Lerp(originalColor.a, originalColor.a * 0.8f, normalizedTime * 2);
                }
                else
                {
                    alpha = Mathf.Lerp(originalColor.a * 0.8f, 0f, (normalizedTime - 0.5f) * 4);
                }

                textComponent.color = new Color(
                    originalColor.r,
                    originalColor.g,
                    originalColor.b,
                    alpha
                );
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}