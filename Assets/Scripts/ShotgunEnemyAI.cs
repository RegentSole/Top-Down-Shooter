using UnityEngine;

public class ShotgunEnemyAI : EnemyAI
{
    [Header("Shotgun Attack Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.8f;
    public float bulletForce = 8f;
    public int pellets = 5;
    public float spreadAngle = 45f;

    [Header("Aggressive Behavior")]
    public float chargeSpeed = 3.5f;
    public float minAttackDistance = 2f;
    public bool alwaysCharge = true;

    private float nextFireTime;
    private float originalMoveSpeed;

    protected override void InitializeAI()
    {
        detectionRange = 8f;
        attackRange = 5f;
        moveSpeed = 2.5f;
        originalMoveSpeed = moveSpeed;
    }

    protected override void IdleBehavior()
    {
        movement = Vector2.zero;
    }

    protected override void PatrolBehavior()
    {
        // Агрессивное патрулирование - быстрые движения
        if (Random.Range(0f, 1f) < 0.02f)
        {
            movement = Random.insideUnitCircle.normalized;
        }
    }

    protected override void ChaseBehavior()
    {
        if (alwaysCharge)
        {
            moveSpeed = chargeSpeed;
        }

        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Агрессивно преследуем игрока
        movement = directionToPlayer;

        // Если достаточно близко - атакуем даже во время преследования
        if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            AttackBehavior();
        }
    }

    protected override void AttackBehavior()
    {
        // Продолжаем движение во время атаки (агрессивное поведение)
        if (distanceToPlayer > minAttackDistance)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            movement = directionToPlayer * 0.5f; // Замедляемся при стрельбе
        }
        else
        {
            movement = Vector2.zero; // Останавливаемся если слишком близко
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    protected override void FleeBehavior()
    {
        // Дробовики редко отступают, но если здоровье низкое...
        if (health < 20f)
        {
            Vector2 directionFromPlayer = (transform.position - player.position).normalized;
            movement = directionFromPlayer;
        }
        else
        {
            // Иначе продолжаем атаковать
            AttackBehavior();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        for (int i = 0; i < pellets; i++)
        {
            float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);

            GameObject pellet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce(pellet.transform.right * bulletForce, ForceMode2D.Impulse);
            }
        }

        // Отдача (отталкивание назад)
        Vector2 recoilDirection = -((Vector2)(player.position - transform.position)).normalized;
        rb.AddForce(recoilDirection * 2f, ForceMode2D.Impulse);
    }

    protected override void HandleStateTransitions()
    {
        base.HandleStateTransitions();

        // Дополнительная агрессивная логика
        if (playerDetected && currentState != AIState.Attack)
        {
            currentState = AIState.Chase;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // Становятся еще более агрессивными при получении урона
        if (health < 30f && !alwaysCharge)
        {
            alwaysCharge = true;
            moveSpeed = chargeSpeed;
        }
    }
}