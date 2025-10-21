using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string weaponName;
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Получаем менеджер оружия игрока
            PlayerWeaponManager weaponManager = other.GetComponent<PlayerWeaponManager>();

            if (weaponManager != null)
            {
                // Добавляем оружие игроку
                weaponManager.PickupWeapon(weaponName);

                // Создаем эффект подбора
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // Уничтожаем предмет подбора
                Destroy(gameObject);
            }
        }
    }
}