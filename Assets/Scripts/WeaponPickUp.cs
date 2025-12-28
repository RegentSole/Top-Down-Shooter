using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string weaponName;
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeaponManager weaponManager = other.GetComponent<PlayerWeaponManager>();

            if (weaponManager != null)
            {
                weaponManager.PickupWeapon(weaponName);

                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}