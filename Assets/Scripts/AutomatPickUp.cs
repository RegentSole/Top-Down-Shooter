using UnityEngine;

public class AutomatPickup : MonoBehaviour
{
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeaponManager weaponManager = other.GetComponent<PlayerWeaponManager>();

            if (weaponManager != null)
            {
                weaponManager.PickupWeapon("Automat");

                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}