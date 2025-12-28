using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [Header("Ranged Attack Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletForce = 10f;
    public float aimAccuracy = 0.95f;

    [Header("Cover Settings")]
    public bool useCover = true;
    public float coverSearchRadius = 3f;
    public LayerMask coverLayers;

    private float nextFireTime;
    private Vector3 coverPosition;
    private bool inCover = false;

    protected override void InitializeAI()
    {
        detectionRange = 15f;
        attackRange = 12f;
        moveSpeed = 1.5f;
    }

    protected override void IdleBehavior()
    {
        movement = Vector2.zero;
    }

    protected override void PatrolBehavior()
    {
        if (Random.Range(0f, 1f) < 0.01f)
        {
            movement = Random.insideUnitCircle.normalized;
        }
    }

    protected override void ChaseBehavior()
    {
        if (useCover && !inCover)
        {
            FindCover();
        }

        if (inCover)
        {
            movement = Vector2.zero;
            if (Time.time >= nextFireTime && playerDetected)
            {
                AttackBehavior();
            }
        }
        else
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float desiredDistance = attackRange * 0.8f;

            if (distanceToPlayer > desiredDistance)
            {
                movement = directionToPlayer;
            }
            else
            {
                movement = -directionToPlayer; // Отступление если слишком близко
            }
        }
    }

    protected override void AttackBehavior()
    {
        movement = Vector2.zero;

        Vector2 aimDirection = (player.position - firePoint.position).normalized;

        if (Random.Range(0f, 1f) > aimAccuracy)
        {
            aimDirection = Quaternion.Euler(0, 0, Random.Range(-5f, 5f)) * aimDirection;
        }

        if (Time.time >= nextFireTime)
        {
            Shoot(aimDirection);
            nextFireTime = Time.time + 1f / fireRate;
        }

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected override void FleeBehavior()
    {
        Vector2 directionFromPlayer = (transform.position - player.position).normalized;
        movement = directionFromPlayer;
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }

    }

    void FindCover()
    {
        Collider2D[] coverSpots = Physics2D.OverlapCircleAll(transform.position, coverSearchRadius, coverLayers);

        if (coverSpots.Length > 0)
        {
            Collider2D bestCover = null;
            float bestScore = 0f;

            foreach (var cover in coverSpots)
            {
                Vector3 coverPos = cover.transform.position;
                Vector3 toPlayer = player.position - coverPos;

                RaycastHit2D hit = Physics2D.Raycast(coverPos, toPlayer.normalized, toPlayer.magnitude, obstacleLayers);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    float score = 1f / Vector3.Distance(transform.position, coverPos);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestCover = cover;
                    }
                }
            }

            if (bestCover != null)
            {
                coverPosition = bestCover.transform.position;
                movement = (coverPosition - transform.position).normalized;
                inCover = true;
            }
        }
    }

    protected override void HandleStateTransitions()
    {
        base.HandleStateTransitions();

        if (currentState == AIState.Chase && inCover && !playerDetected)
        {
            inCover = false; // Покидаем укрытие если игрок не виден
        }
    }
}