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

    [Header("Audio")]
    public AudioClip shootSound;
    public float shootVolume = 0.7f;

    [Header("References")]
    public Transform player;

    private float nextFireTime;
    private bool playerInRange;

    void Start()
    {
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange && distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (playerInRange)
        {
            RotateTowardsPlayer();
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Vector2 direction = (player.position - firePoint.position).normalized;
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        PlayShootSound();
    }

    void PlayShootSound()
    {
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position, shootVolume);
        }
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}