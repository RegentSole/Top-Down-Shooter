using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletForce = 10f;
    public float detectionRange = 10f;
    public float attackRange = 8f;

    [Header("References")]
    public Transform player;

    private float nextFireTime;
    private bool playerInRange;

    void Start()
    {
        // Автоматически находим игрока по тегу
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        nextFireTime = Time.time + fireRate;
    }

    void Update()
    {
        if (player == null) return;

        // Проверяем, находится ли игрок в зоне обнаружения
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;

        // Если игрок в зоне атаки и пришло время стрелять
        if (playerInRange && distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Поворачиваем врага в сторону игрока (опционально)
        if (playerInRange)
        {
            RotateTowardsPlayer();
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab == null || firePoint == null) return;

        // Создаем пулю
        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Направляем пулю в игрока
        Vector2 direction = (player.position - firePoint.position).normalized;
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Визуализация зон обнаружения и атаки в редакторе
    void OnDrawGizmosSelected()
    {
        // Зона обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Зона атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}