using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 10f;
    public float lifetime = 3f;
    public GameObject destroyEffect; // Эффект разрушения пули

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем столкновения с врагами и другими пулями
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
            return;

        // Если попали в игрока
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else
        {
            // Если столкнулись с чем-то другим (стена, препятствие)
            // Создаем эффект разрушения
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            }
        }

        // Уничтожаем пулю при любом столкновении
        Destroy(gameObject);
    }

    // Также обрабатываем физические столкновения (если коллайдер не является триггером)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Создаем эффект разрушения
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Уничтожаем пулю
        Destroy(gameObject);
    }
}