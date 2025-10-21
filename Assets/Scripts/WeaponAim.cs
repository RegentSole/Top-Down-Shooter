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

        // Получаем позицию мыши в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Вычисляем направление от оружия к курсору
        Vector2 direction = mousePosition - transform.position;

        // Вычисляем угол поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Применяем поворот
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Определяем, нужно ли переворачивать спрайт
        if (weaponSprite != null)
        {
            // Переворачиваем спрайт, если оружие направлено влево
            weaponSprite.flipY = Mathf.Abs(angle) > 90f;
        }
    }
}