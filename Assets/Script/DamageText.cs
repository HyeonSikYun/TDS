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
            Debug.Log("������ �ؽ�Ʈ ����: " + damage);
        }
        else
        {
            Debug.LogError("TextMeshPro ������Ʈ�� null�Դϴ�!");
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

            // ���� �ö󰡴� ����
            transform.position = originalPosition + new Vector3(0, normalizedTime * upSpeed, 0);

            // ���̵� �ƿ�
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