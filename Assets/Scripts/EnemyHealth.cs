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
        if (health <= 0) return; // Если уже мертв, игнорируем урон

        health -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        // Отключаем коллайдер
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        // Запускаем эффект растворения
        if (useDissolveEffect && dissolveEffect != null)
        {
            dissolveEffect.StartDissolve();
        }
        else
        {
            // Стандартное уничтожение
            Destroy(gameObject);
        }
    }

    // Для тестирования в редакторе
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }
    }
}