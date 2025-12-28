using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    [Header("Melee Attack Settings")]
    public float meleeDamage = 20f;
    public float meleeRange = 1.5f;
    public float attackRate = 1f;
    public float lungeForce = 5f;

    [Header("Movement Patterns")]
    public bool useZigZag = true;
    public float zigZagFrequency = 2f;
    public float zigZagAmplitude = 1f;

    private float nextAttackTime;
    private Vector2 zigZagDirection;
    private float zigZagTimer;

    protected override void InitializeAI()
    {
        detectionRange = 6f;
        attackRange = meleeRange;
        moveSpeed = 3f;
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
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        if (useZigZag)
        {
            zigZagTimer += Time.deltaTime;
            Vector2 perpendicular = new Vector2(-directionToPlayer.y, directionToPlayer.x);
            zigZagDirection = directionToPlayer + perpendicular * Mathf.Sin(zigZagTimer * zigZagFrequency) * zigZagAmplitude;
            movement = zigZagDirection.normalized;
        }
        else
        {
            movement = directionToPlayer;
        }

        if (distanceToPlayer <= meleeRange && Time.time >= nextAttackTime)
        {
            AttackBehavior();
        }
    }

    protected override void AttackBehavior()
    {
        if (Time.time >= nextAttackTime)
        {
            Vector2 lungeDirection = (player.position - transform.position).normalized;
            rb.AddForce(lungeDirection * lungeForce, ForceMode2D.Impulse);

            if (distanceToPlayer <= meleeRange * 1.2f) // Немного увеличенный диапазон для компенсации движения
            {
                PerformMeleeAttack();
            }

            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    protected override void FleeBehavior()
    {
        if (health < 15f)
        {
            Vector2 directionFromPlayer = (transform.position - player.position).normalized;
            movement = directionFromPlayer;
        }
        else
        {
            AttackBehavior();
        }
    }

    void PerformMeleeAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, meleeRange);

        foreach (var collider in hitPlayers)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(meleeDamage);
                    Debug.Log($"Melee attack hit player for {meleeDamage} damage");
                }
            }
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    protected override void HandleStateTransitions()
    {
        base.HandleStateTransitions();

        if (playerDetected && currentState == AIState.Patrol)
        {
            currentState = AIState.Chase;
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}