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

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (flashLight != null && useLight)
        {
            flashLight.enabled = true;
        }

        Destroy(gameObject, flashDuration);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / flashDuration;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f - progress;
            spriteRenderer.color = color;

        }

        if (flashLight != null && useLight)
        {
            flashLight.intensity *= (1f - progress);
        }
    }
}