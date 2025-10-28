using UnityEngine;

public class Automat : Weapon
{
    public float bulletForce = 18f;
    public float automaticFireRate = 0.1f;

    void Start()
    {
        weaponName = "Automat";
        fireRate = automaticFireRate;
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        // Выстрел
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float spread = Random.Range(-2f, 2f);
            Vector2 direction = Quaternion.Euler(0, 0, spread) * firePoint.right;
            rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }

        // Выброс гильзы
        EjectShell();

        // Настраиваем урон пули
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = 8f;
        }

        nextFireTime = Time.time + fireRate;
        UseAmmo();
    }
}