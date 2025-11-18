using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public float healthAmount = 25f;
    public bool isFullHeal = false;

    [Header("Visual Settings")]
    public SpriteRenderer spriteRenderer;
    public Color commonColor = Color.green;
    public Color rareColor = Color.red;
    public float rotationSpeed = 45f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1f;

    [Header("Audio")]
    public AudioClip pickupSound;
    public float pickupVolume = 0.7f;

    [Header("Effects")]
    public GameObject pickupEffect;

    private Vector3 startPosition;
    private bool isCollected = false;

    void Start()
    {
        startPosition = transform.position;

        // Визуальная настройка в зависимости от типа аптечки
        if (spriteRenderer != null)
        {
            if (healthAmount >= 50f || isFullHeal)
            {
                spriteRenderer.color = rareColor;
            }
            else
            {
                spriteRenderer.color = commonColor;
            }
        }
    }

    void Update()
    {
        // Анимация парения и вращения
        if (!isCollected)
        {
            // Вращение
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // Парение
            float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Collect(playerHealth);
            }
        }
    }

    // В методе Collect() HealthPickup.cs замените создание эффекта:
    void Collect(PlayerHealth playerHealth)
    {
        isCollected = true;

        // Восстанавливаем здоровье игроку
        if (isFullHeal)
        {
            playerHealth.Heal(playerHealth.maxHealth);
        }
        else
        {
            playerHealth.Heal(healthAmount);
        }

        // Воспроизводим звук подбора
        PlayPickupSound();

        // Создаем эффект лечения НА ИГРОКЕ
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, playerHealth.transform.position, Quaternion.identity);
        }
        else
        {
            // Создаем базовый эффект если нет назначенного префаба
            CreateDefaultHealEffect(playerHealth.transform.position);
        }

        // Отключаем визуальную часть
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Уничтожаем объект
        Destroy(gameObject, 0.5f);

        Debug.Log($"Health pickup collected! Healed: {(isFullHeal ? "FULL" : healthAmount.ToString())}");
    }

    void CreateDefaultHealEffect(Vector3 position)
    {
        // Создаем простой эффект программно
        GameObject healEffect = new GameObject("HealEffect");
        healEffect.transform.position = position;
        healEffect.AddComponent<HealEffect>();
    }

    void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);
        }
    }

    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = healthAmount >= 50f ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}