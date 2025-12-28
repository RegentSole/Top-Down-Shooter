using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Настройки взрыва")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float explosionDelay = 0.1f;

    [Header("Эффекты")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private GameObject barrelModel;

    [Header("Настройки урона")]
    [SerializeField] private bool damagePlayer = true;
    [SerializeField] private bool damageEnemies = true;
    [SerializeField] private LayerMask damageLayers = -1; // Все слои по умолчанию

    [Header("Дебаг")]
    [SerializeField] private bool showExplosionRadius = true;
    [SerializeField] private bool debugMode = true;

    private bool hasExploded = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (debugMode)
            Debug.Log($"ExplosiveBarrel инициализирован на {gameObject.name}");
    }

    // Вариант 1: Срабатывание при попадании пули (триггер)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (debugMode)
            Debug.Log($"OnTriggerEnter2D: {other.gameObject.name}, tag: {other.tag}");

        if (other.CompareTag("Bullet") && !hasExploded)
        {
            if (debugMode)
                Debug.Log($"Пуля попала в бочку от {other.gameObject.name}");

            StartCoroutine(ExplodeWithDelay());

            Destroy(other.gameObject);
        }
    }

    // Вариант 2: Срабатывание при столкновении (физика)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (debugMode)
            Debug.Log($"OnCollisionEnter2D: {collision.gameObject.name}, tag: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Bullet") && !hasExploded)
        {
            if (debugMode)
                Debug.Log($"Пуля столкнулась с бочкой от {collision.gameObject.name}");

            StartCoroutine(ExplodeWithDelay());

            Destroy(collision.gameObject);
        }

        if (collision.relativeVelocity.magnitude > 10f && !hasExploded)
        {
            if (debugMode)
                Debug.Log($"Сильное столкновение с {collision.gameObject.name}, скорость: {collision.relativeVelocity.magnitude}");

            StartCoroutine(ExplodeWithDelay());
        }
    }

    public void TriggerExplosion()
    {
        if (!hasExploded)
        {
            StartCoroutine(ExplodeWithDelay());
        }
    }

    private IEnumerator ExplodeWithDelay()
    {
        if (hasExploded) yield break;

        hasExploded = true;

        if (debugMode)
            Debug.Log($"Бочка начинает взрыв на {gameObject.name}");

        if (barrelModel != null)
        {
            var spriteRenderer = barrelModel.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }
        }

        yield return new WaitForSeconds(explosionDelay);

        Explode();
    }

    private void Explode()
    {
        if (debugMode)
            Debug.Log($"ВЗРЫВ! Позиция: {transform.position}, радиус: {explosionRadius}");

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        else if (debugMode)
        {
            Debug.LogWarning("Не назначен explosionEffectPrefab!");
        }

        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        if (debugMode)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.right * explosionRadius, Color.red, 2f);
            Debug.DrawLine(transform.position, transform.position + Vector3.left * explosionRadius, Color.red, 2f);
            Debug.DrawLine(transform.position, transform.position + Vector3.up * explosionRadius, Color.red, 2f);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * explosionRadius, Color.red, 2f);
        }

        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        if (debugMode)
            Debug.Log($"Найдено коллайдеров в радиусе: {allColliders.Length}");

        List<string> damagedObjects = new List<string>();

        foreach (Collider2D hit in allColliders)
        {
            if (hit == null || hit.gameObject == gameObject) continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);

            if (debugMode)
                Debug.Log($"Проверяем объект: {hit.gameObject.name}, тег: {hit.tag}, расстояние: {distance}");

            if (distance > explosionRadius) continue;

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null && !rb.isKinematic)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                rb.AddForce(direction * explosionForce);

                if (debugMode)
                    Debug.Log($"Применена сила к {hit.gameObject.name}: {direction * explosionForce}");
            }

            float damageMultiplier = Mathf.Clamp01(1f - (distance / explosionRadius));
            float actualDamage = explosionDamage * damageMultiplier;

            if (debugMode && actualDamage > 0)
                Debug.Log($"Расчет урона для {hit.gameObject.name}: базовый={explosionDamage}, множитель={damageMultiplier:F2}, итого={actualDamage:F1}");

            bool damageApplied = false;

            if (damageEnemies && hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(actualDamage);
                    damageApplied = true;
                    damagedObjects.Add($"{hit.gameObject.name} (Enemy): {actualDamage:F1} урона");

                    if (debugMode)
                        Debug.Log($"Нанесен урон врагу {hit.gameObject.name}: {actualDamage:F1}");
                }
            }

            if (damagePlayer && hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(actualDamage);
                    damageApplied = true;
                    damagedObjects.Add($"{hit.gameObject.name} (Player): {actualDamage:F1} урона");

                    if (debugMode)
                        Debug.Log($"Нанесен урон игроку {hit.gameObject.name}: {actualDamage:F1}");
                }
            }

            if (hit.CompareTag("Explosive") && hit.gameObject != gameObject)
            {
                ExplosiveBarrel otherBarrel = hit.GetComponent<ExplosiveBarrel>();
                if (otherBarrel != null && !otherBarrel.hasExploded)
                {
                    otherBarrel.TriggerExplosion();
                    damagedObjects.Add($"{hit.gameObject.name} (Explosive): запущена цепная реакция");

                    if (debugMode)
                        Debug.Log($"Запущена цепная реакция с {hit.gameObject.name}");
                }
            }
        }

        if (debugMode)
        {
            Debug.Log($"=== ОТЧЕТ О ВЗРЫВЕ ===");
            Debug.Log($"Всего объектов в радиусе: {allColliders.Length}");
            Debug.Log($"Нанесен урон {damagedObjects.Count} объектам:");
            foreach (var obj in damagedObjects)
            {
                Debug.Log($"  - {obj}");
            }
            Debug.Log($"=====================");
        }

        if (barrelModel != null)
        {
            barrelModel.SetActive(false);
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (showExplosionRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && debugMode && !hasExploded)
        {
            Debug.Log("Тестовый взрыв по клавише K");
            StartCoroutine(ExplodeWithDelay());
        }
    }
}