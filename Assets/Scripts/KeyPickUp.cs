using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    public KeyType keyType = KeyType.Gold;
    public float rotationSpeed = 90f;
    public float floatHeight = 0.2f;
    public float floatSpeed = 1f;

    [Header("Effects")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private Vector3 startPosition;
    private bool isCollected = false;

    public enum KeyType
    {
        Gold,
        Silver,
        Bronze
    }

    void Start()
    {
        startPosition = transform.position;

        // Визуальная настройка в зависимости от типа ключа
        SetKeyAppearance();
    }

    void Update()
    {
        if (!isCollected)
        {
            // Вращение
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // Парение
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void SetKeyAppearance()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            switch (keyType)
            {
                case KeyType.Gold:
                    sr.color = Color.yellow;
                    break;
                case KeyType.Silver:
                    sr.color = Color.white;
                    break;
                case KeyType.Bronze:
                    sr.color = new Color(0.8f, 0.5f, 0.2f);
                    break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            PlayerKeyManager keyManager = other.GetComponent<PlayerKeyManager>();
            if (keyManager != null)
            {
                Collect(keyManager);
            }
        }
    }

    void Collect(PlayerKeyManager keyManager)
    {
        isCollected = true;

        // Добавляем ключ игроку
        keyManager.AddKey();

        // Создаем эффект подбора
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // Воспроизводим звук
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Отключаем визуальную часть
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Уничтожаем объект
        Destroy(gameObject, 1f);

        Debug.Log($"Key collected! Type: {keyType}");
    }

    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}