using UnityEngine;
using System.Collections;

public abstract class EnemyAI : MonoBehaviour
{
    [Header("Base AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float health = 50f;

    [Header("References")]
    public Transform player;
    public LayerMask obstacleLayers;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 movement;
    protected bool playerDetected = false;
    protected float distanceToPlayer;
    protected Vector2 lastKnownPlayerPosition;

    // Состояния ИИ
    public enum AIState { Idle, Patrol, Chase, Attack, Flee }
    public AIState currentState = AIState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        InitializeAI();
    }

    void Update()
    {
        if (player == null) return;

        UpdatePlayerDetection();
        UpdateAI();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        MoveEnemy();
    }

    void UpdatePlayerDetection()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Проверка видимости игрока
        bool hasLineOfSight = CheckLineOfSight();

        playerDetected = (distanceToPlayer <= detectionRange) && hasLineOfSight;

        if (playerDetected)
        {
            lastKnownPlayerPosition = player.position;
        }
    }

    bool CheckLineOfSight()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, obstacleLayers);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }

    protected virtual void UpdateAI()
    {
        switch (currentState)
        {
            case AIState.Idle:
                IdleBehavior();
                break;
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Flee:
                FleeBehavior();
                break;
        }

        HandleStateTransitions();
    }

    protected virtual void HandleStateTransitions()
    {
        if (playerDetected)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = AIState.Attack;
            }
            else
            {
                currentState = AIState.Chase;
            }
        }
        else
        {
            if (currentState == AIState.Chase || currentState == AIState.Attack)
            {
                // Поиск игрока в последней известной позиции
                if (Vector2.Distance(transform.position, lastKnownPlayerPosition) > 1f)
                {
                    currentState = AIState.Chase;
                }
                else
                {
                    currentState = AIState.Patrol;
                }
            }
        }
    }

    protected abstract void InitializeAI();
    protected abstract void IdleBehavior();
    protected abstract void PatrolBehavior();
    protected abstract void ChaseBehavior();
    protected abstract void AttackBehavior();
    protected abstract void FleeBehavior();

    protected virtual void MoveEnemy()
    {
        if (movement.magnitude > 0.1f)
        {
            rb.velocity = movement * moveSpeed;

            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", rb.velocity.magnitude);
            animator.SetBool("PlayerDetected", playerDetected);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Gizmos.color = playerDetected ? Color.green : Color.white;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Gizmos.color = playerDetected ? Color.green : Color.white;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}