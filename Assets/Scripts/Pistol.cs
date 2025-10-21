using UnityEngine;

public class Pistol : Weapon
{
    public float bulletForce = 20f;

    void Start()
    {
        weaponName = "Pistol";
        isAvailable = true;
        infiniteAmmo = true;

        // Устанавливаем спрайт, если он назначен
        if (weaponSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = weaponSprite;
        }
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        }

        nextFireTime = Time.time + fireRate;
        Debug.Log("Pistol shot!");
    }
}