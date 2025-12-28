using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 50f;
    public bool useDissolveEffect = true;

    private DissolveEffect dissolveEffect;

    void Start()
    {
        dissolveEffect = GetComponent<DissolveEffect>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (health <= 0) return;

        health -= damageAmount;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(damageAmount);
        }

        Debug.Log($"Enemy took {damageAmount} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        if (useDissolveEffect && dissolveEffect != null)
        {
            dissolveEffect.StartDissolve();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }
    }
}