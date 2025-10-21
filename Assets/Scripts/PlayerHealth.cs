using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    [Header("UI References")]
    public TextMeshProUGUI healthText;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Этот метод теперь вызывается только из скрипта пули
    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return; // Если игрок уже мертв, не обрабатываем урон

        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {Mathf.RoundToInt(currentHealth)}/{maxHealth}";
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Здесь можно добавить обработку смерти игрока
        // Например, отключение управления, анимацию смерти, перезагрузку уровня
    }

    // Метод для лечения (на будущее)
    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthUI();
    }
}