using UnityEngine;
using UnityEngine.UI;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Настройки урона")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1f; // КД между ударами
    [SerializeField] private bool knockback = true;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float soundVolume = 0.5f;

    [Header("Настройки столкновения")]
    [SerializeField] private bool useTrigger = false;
    [SerializeField] private LayerMask playerLayer;

    private float lastDamageTime;
    private PlayerHealth playerHealth;

    private void Start()
    {
        if (playerLayer.value == 0)
        {
            playerLayer = LayerMask.GetMask("Player");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;

        if (other.CompareTag("Player"))
        {
            ProcessDamage(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (useTrigger) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            ProcessDamage(collision.gameObject);
        }
    }

    private void ProcessDamage(GameObject player)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        if (playerHealth == null || playerHealth.gameObject != player)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            lastDamageTime = Time.time;

            PlayHitEffects(player.transform.position);

            if (knockback)
            {
                ApplyKnockback(player);
            }

            Debug.Log($"Игрок получил {damageAmount} урона от {gameObject.name}");
        }
    }

    private void ApplyKnockback(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private void PlayHitEffects(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, position, Quaternion.identity);
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position, soundVolume);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (useTrigger) return;

        if (collision.gameObject.CompareTag("Player") && Time.time - lastDamageTime >= damageCooldown)
        {
            ProcessDamage(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!useTrigger) return;

        if (other.CompareTag("Player") && Time.time - lastDamageTime >= damageCooldown)
        {
            ProcessDamage(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}