using UnityEngine;

public class WeaponFirePointFix : MonoBehaviour
{
    [Header("Fire Point References")]
    public Transform firePoint;
    public SpriteRenderer weaponSprite;

    [Header("Fire Point Positions")]
    public Vector3 rightPosition = new Vector3(0.3f, 0f, 0f);
    public Vector3 leftPosition = new Vector3(-0.3f, 0f, 0f);

    void Update()
    {
        FixFirePointPosition();
    }

    void FixFirePointPosition()
    {
        // Проверяем, что все ссылки установлены
        if (firePoint == null || weaponSprite == null)
        {
            Debug.LogWarning("FirePoint or WeaponSprite reference is missing!");
            return;
        }

        // Корректируем позицию FirePoint в зависимости от направления оружия
        if (weaponSprite.flipY) // Оружие смотрит влево
        {
            firePoint.localPosition = leftPosition;
        }
        else // Оружие смотрит вправо
        {
            firePoint.localPosition = rightPosition;
        }

        // Сохраняем нейтральный поворот FirePoint
        firePoint.localRotation = Quaternion.identity;
    }

    // Визуализация в редакторе для удобства настройки
    void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.05f);

            // Рисуем направление выстрела
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePoint.position, firePoint.right * 0.3f);
        }
    }
}