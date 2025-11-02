using UnityEngine;

public class WeaponPointsFlipFix : MonoBehaviour
{
    [Header("Weapon References")]
    public SpriteRenderer weaponSprite;

    [Header("Points to Adjust")]
    public Transform firePoint;
    public Vector3 firePointRight = new Vector3(0.3f, 0f, 0f);
    public Vector3 firePointLeft = new Vector3(-0.3f, 0f, 0f);

    public Transform muzzleFlashPoint;
    public Vector3 muzzleRight = new Vector3(0.3f, 0f, 0f);
    public Vector3 muzzleLeft = new Vector3(-0.3f, 0f, 0f);

    public Transform ejectionPoint;
    public Vector3 ejectionRight = new Vector3(0.1f, 0.1f, 0f);
    public Vector3 ejectionLeft = new Vector3(-0.1f, 0.1f, 0f);

    [Header("Debug")]
    public bool showGizmos = true;

    private bool wasFlipped = false;

    void Update()
    {
        UpdatePointsPosition();
    }

    void UpdatePointsPosition()
    {
        if (weaponSprite == null) return;

        bool isFlipped = weaponSprite.flipY;

        // Обновляем только при изменении состояния flip
        if (isFlipped != wasFlipped)
        {
            // Корректируем firePoint
            if (firePoint != null)
            {
                firePoint.localPosition = isFlipped ? firePointLeft : firePointRight;
            }

            // Корректируем muzzleFlashPoint
            if (muzzleFlashPoint != null)
            {
                muzzleFlashPoint.localPosition = isFlipped ? muzzleLeft : muzzleRight;
            }

            // Корректируем ejectionPoint
            if (ejectionPoint != null)
            {
                ejectionPoint.localPosition = isFlipped ? ejectionLeft : ejectionRight;
            }

            wasFlipped = isFlipped;
        }
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.red;
        if (firePoint != null)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(firePointRight), 0.02f);
            Gizmos.DrawWireSphere(transform.TransformPoint(firePointLeft), 0.02f);
        }

        Gizmos.color = Color.yellow;
        if (muzzleFlashPoint != null)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(muzzleRight), 0.015f);
            Gizmos.DrawWireSphere(transform.TransformPoint(muzzleLeft), 0.015f);
        }

        Gizmos.color = Color.blue;
        if (ejectionPoint != null)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(ejectionRight), 0.01f);
            Gizmos.DrawWireSphere(transform.TransformPoint(ejectionLeft), 0.01f);
        }
    }
}