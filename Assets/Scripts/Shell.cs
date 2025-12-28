using UnityEngine;

public class Shell : MonoBehaviour
{
    [Header("Shell Settings")]
    public float lifetime = 5f;
    public float fadeTime = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float timer;
    private bool isFading = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.AddTorque(Random.Range(-50f, 50f));
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!isFading && timer >= lifetime - fadeTime)
        {
            StartFading();
        }
    }

    void StartFading()
    {
        isFading = true;
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        float startAlpha = spriteRenderer.color.a;
        float fadeTimer = 0f;

        while (fadeTimer < fadeTime)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, fadeTimer / fadeTime);

            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            yield return null;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb != null && collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity *= 0.5f;
            rb.angularVelocity *= 0.5f;
        }
    }
}