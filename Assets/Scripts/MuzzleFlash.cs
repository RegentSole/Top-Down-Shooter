using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashDuration = 0.08f;
    public bool useLight = true;

    private SpriteRenderer spriteRenderer;
    private Light flashLight;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashLight = GetComponent<Light>();

        // Гарантируем, что компоненты активны
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (flashLight != null && useLight)
        {
            flashLight.enabled = true;
        }

        // Уничтожаем через время
        Destroy(gameObject, flashDuration);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / flashDuration;

        // Плавное исчезновение для спрайта
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f - progress;
            spriteRenderer.color = color;

            // Не меняем масштаб - оставляем как есть
            // transform.localScale = Vector3.one * (1f - progress * 0.3f);
        }

        // Плавное исчезновение для света
        if (flashLight != null && useLight)
        {
            flashLight.intensity *= (1f - progress);
        }
    }
}