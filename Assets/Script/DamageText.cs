using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float fadeSpeed = 0.5f;
    public float upSpeed = 1.0f;
    public float lifetime = 1.0f;

    public TextMeshProUGUI textComponent;
    private Color originalColor;

    void Awake()
    {

        originalColor = textComponent.color;
    }

    void Start()
    {
        StartCoroutine(FadeAndMoveUp());
    }

    public void SetDamageValue(int damage)
    {
        if (textComponent != null)
        {
            textComponent.text = damage.ToString();
            Debug.Log("데미지 텍스트 설정: " + damage);
        }
        else
        {
            Debug.LogError("TextMeshPro 컴포넌트가 null입니다!");
        }
    }

    private IEnumerator FadeAndMoveUp()
    {
        float elapsed = 0f;
        Vector3 originalPosition = transform.position;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / lifetime;

            // 위로 올라가는 동작
            transform.position = originalPosition + new Vector3(0, normalizedTime * upSpeed, 0);

            // 페이드 아웃
            if (textComponent != null)
            {
                textComponent.color = new Color(
                    originalColor.r,
                    originalColor.g,
                    originalColor.b,
                    Mathf.Lerp(originalColor.a, 0f, normalizedTime * fadeSpeed)
                );
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}