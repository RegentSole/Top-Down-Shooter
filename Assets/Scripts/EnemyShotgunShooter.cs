using UnityEngine;

public class EnemyShotgunShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float bulletForce = 12f;
    public int pellets = 5;
    public float spreadAngle = 35f;
    public float detectionRange = 10f;
    public float attackRange = 7f;

    [Header("References")]
    public Transform player;

    private float nextFireTime;
    private bool playerInRange;

    void Start()
    {
        // Автоматически находим игрока
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

        // Поворачиваем врага в сторону игрока
        if (playerInRange)
        {
            RotateTowardsPlayer();
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab == null || firePoint == null) return;

        // Создаем несколько дробинок
        for (int i = 0; i < pellets; i++)
        {
            // Рассчитываем случайное отклонение
            float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);

            // Создаем дробинку
            GameObject pellet = Instantiate(enemyBulletPrefab, firePoint.position, rotation);
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();

            // Добавляем силу дробинке
            if (rb != null)
            {
                rb.AddForce(pellet.transform.up * bulletForce, ForceMode2D.Impulse);
            }

            // Настраиваем урон дробинки
            EnemyBullet bulletScript = pellet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                // Уменьшаем урон каждой дробинки
                bulletScript.damage = bulletScript.damage / 2;
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Визуализация зон в редакторе
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