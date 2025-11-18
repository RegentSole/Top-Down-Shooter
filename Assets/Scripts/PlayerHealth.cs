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

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        float oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        float actualHeal = currentHealth - oldHealth;

        UpdateHealthUI();

        // Воспроизводим звук лечения
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

            // Меняем цвет текста в зависимости от здоровья
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
        Debug.Log("Player died!");
        // Здесь можно добавить обработку смерти игрока
    }

    // Для отладки
    void Update()
    {
        // Тестовые клавиши для проверки лечения/урона
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(25f);
        }
    }

    // Свойство для доступа к текущему здоровью из других скриптов
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => currentHealth / maxHealth;
}