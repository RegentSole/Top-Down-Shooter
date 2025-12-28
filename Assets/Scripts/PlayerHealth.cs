using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    [Header("UI References")]
    public TextMeshProUGUI healthText;

    [Header("Audio")]
    public AudioClip healSound;
    public float healVolume = 0.5f;

    [Header("Game Over")]
    public bool triggerGameOver = true;
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;

    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0 || isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        float oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        float actualHeal = currentHealth - oldHealth;

        UpdateHealthUI();

        if (actualHeal > 0 && healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position, healVolume);
        }

        Debug.Log($"Player healed for {actualHeal}. Health: {currentHealth}/{maxHealth}");
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {Mathf.RoundToInt(currentHealth)}/{maxHealth}";

            if (currentHealth < maxHealth * 0.3f)
            {
                healthText.color = Color.red;
            }
            else if (currentHealth < maxHealth * 0.7f)
            {
                healthText.color = Color.yellow;
            }
            else
            {
                healthText.color = Color.green;
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Player died!");

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        if (triggerGameOver && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("GameManager не найден. Game Over не будет вызван.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !isDead)
        {
            TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.J) && !isDead)
        {
            Heal(25f);
        }

        if (Input.GetKeyDown(KeyCode.G) && !isDead)
        {
            Die();
        }
    }

    public float CurrentHealth => currentHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsDead => isDead;

    public void Revive(float healthPercent = 1f)
    {
        isDead = false;
        currentHealth = maxHealth * Mathf.Clamp01(healthPercent);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = true;
        }

        UpdateHealthUI();
    }
}