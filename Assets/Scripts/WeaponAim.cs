using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public Transform firePoint;
    public SpriteRenderer weaponSprite;

    void Update()
    {
        AimWeapon();
    }

    void AimWeapon()
    {
        if (Camera.main == null) return;


        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 direction = mousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (weaponSprite != null)
        {
            weaponSprite.flipY = Mathf.Abs(angle) > 90f;
        }
    }
}