using UnityEngine;

public class Shotgun : Weapon
{
    public float bulletForce = 15f;
    public int pellets = 5;
    public float spreadAngle = 30f;

    void Start()
    {
        weaponName = "Shotgun";
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        // Выстрел
        for (int i = 0; i < pellets; i++)
        {
            GameObject pellet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
                Vector2 direction = Quaternion.Euler(0, 0, angle) * firePoint.up;
                rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
            }

            Bullet bulletScript = pellet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = bulletScript.damage / 2;
            }
        }

        CreateMuzzleFlash();

        // Выброс гильзы (только одна за весь заряд дроби)
        EjectShell();

        ApplyRecoil();

        PlayShootSound();

        nextFireTime = Time.time + fireRate;
        UseAmmo();
    }
}