using UnityEngine;

public class Pistol : Weapon
{
    public float bulletForce = 20f;

    void Start()
    {
        weaponName = "Pistol";
        isAvailable = true;
        infiniteAmmo = true;
        flashIntensityMultiplier = 0.8f; // Меньшая вспышка для пистолета
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        // Выстрел
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 shootDirection = firePoint.right;
            rb.AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
        }

        // Вспышка выстрела
        CreateMuzzleFlash();

        // Выброс гильзы
        EjectShell();

        nextFireTime = Time.time + fireRate;
    }
}