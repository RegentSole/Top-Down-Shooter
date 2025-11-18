using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 2f;

    [Header("Trail Settings")]
    public TrailRenderer bulletTrail;
    public float trailTime = 0.1f;

    void Start()
    {
        // Настройка трейла при старте
        if (bulletTrail != null)
        {
            bulletTrail.time = trailTime;
            bulletTrail.Clear(); // Очищаем старые следы
        }

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Отсоединяем трейл перед уничтожением пули
            DetachTrail();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            DetachTrail();
            Destroy(gameObject);
        }
    }

    void DetachTrail()
    {
        if (bulletTrail != null)
        {
            // Отсоединяем трейл от пули, чтобы он плавно исчез
            bulletTrail.transform.SetParent(null);
            Destroy(bulletTrail.gameObject, bulletTrail.time);
        }
    }

    // Для пула объектов (на будущее)
    public void ResetTrail()
    {
        if (bulletTrail != null)
        {
            bulletTrail.Clear();
        }
    }
}