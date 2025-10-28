using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashDuration = 0.05f;
    public float maxSize = 1.5f;
    public float minSize = 0.8f;

    private SpriteRenderer spriteRenderer;
    private Light flashLight;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashLight = GetComponent<Light>();

        // Случайный размер вспышки
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = Vector3.one * randomSize;

        // Случайный поворот
        transform.Rotate(0, 0, Random.Range(0, 360f));

        // Начинаем исчезать
        StartCoroutine(FadeFlash());
    }

    System.Collections.IEnumerator FadeFlash()
    {
        timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / flashDuration;

            // Плавное исчезновение
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - progress;
                spriteRenderer.color = color;
            }

            // Уменьшаем интенсивность света
            if (flashLight != null)
            {
                flashLight.intensity = 1f - progress;
            }

            // Немного уменьшаем размер
            transform.localScale = Vector3.one * (maxSize * (1f - progress * 0.5f));

            yield return null;
        }

        Destroy(gameObject);
    }
}