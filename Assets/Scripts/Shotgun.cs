using UnityEngine;

public class Shotgun : Weapon
{
    public float bulletForce = 15f;
    public int pellets = 5;
    public float spreadAngle = 30f;

    void Start()
    {
        weaponName = "Shotgun";

        // Устанавливаем спрайт, если он назначен
        if (weaponSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = weaponSprite;
        }
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        for (int i = 0; i < pellets; i++)
        {
            float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);

            GameObject pellet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce(pellet.transform.up * bulletForce, ForceMode2D.Impulse);
            }

            Bullet bulletScript = pellet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = bulletScript.damage / 2;
            }
        }

        nextFireTime = Time.time + fireRate;
        UseAmmo();
        Debug.Log("Shotgun blast! Ammo: " + ammo);
    }
}