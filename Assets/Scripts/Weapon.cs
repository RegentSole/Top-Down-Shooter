using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    public int ammo = 10;
    public int maxAmmo = 10;
    public bool isAvailable = false;
    public bool infiniteAmmo = false;

    [Header("Shell Ejection")]
    public GameObject shellPrefab;
    public Transform ejectionPoint;
    public float ejectionForce = 3f;
    public float ejectionTorque = 10f;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPoint;
    public float flashIntensityMultiplier = 1f;

    [Header("Visual Settings")]
    public Sprite weaponSprite;
    public Vector3 equippedPosition = new Vector3(0.2f, 0.1f, 0f);

    protected SpriteRenderer spriteRenderer;
    protected float nextFireTime;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && weaponSprite != null)
        {
            spriteRenderer.sprite = weaponSprite;
        }

        // Устанавливаем начальную позицию
        transform.localPosition = equippedPosition;
    }

    void Update()
    {
        if (isAvailable)
        {
            UpdateWeaponFlip();
        }
    }

    // Метод для обновления переворота оружия
    protected virtual void UpdateWeaponFlip()
    {
        if (Camera.main == null) return;

        // Получаем позицию мыши
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Определяем направление к мыши
        Vector3 direction = mousePos - transform.position;

        // Вычисляем угол
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Применяем переворот спрайта
        if (spriteRenderer != null)
        {
            // Переворачиваем спрайт, если оружие направлено влево
            spriteRenderer.flipY = Mathf.Abs(angle) > 90f;
        }
    }

    public abstract void Shoot();

    public virtual void Reload()
    {
        ammo = maxAmmo;
        Debug.Log(weaponName + " reloaded. Ammo: " + ammo);
    }

    public virtual bool CanShoot()
    {
        return Time.time >= nextFireTime && (ammo > 0 || infiniteAmmo) && isAvailable;
    }

    protected void UseAmmo()
    {
        if (!infiniteAmmo)
        {
            ammo--;
        }
    }

    public virtual void SetVisible(bool visible)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }
    }

    protected virtual void EjectShell()
    {
        if (shellPrefab == null || ejectionPoint == null) return;

        // Создаем гильзу
        GameObject shell = Instantiate(shellPrefab, ejectionPoint.position, ejectionPoint.rotation);
        Rigidbody2D shellRb = shell.GetComponent<Rigidbody2D>();

        if (shellRb != null)
        {
            // Случайное направление выброса
            Vector2 ejectionDirection = Quaternion.Euler(0, 0, Random.Range(-30f, 30f)) * -transform.right;
            shellRb.AddForce(ejectionDirection * ejectionForce, ForceMode2D.Impulse);

            // Случайное вращение
            shellRb.AddTorque(Random.Range(-ejectionTorque, ejectionTorque));
        }
    }

    protected virtual void CreateMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzleFlashPoint == null) return;

        // Создаем вспышку
        GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
        flash.transform.SetParent(muzzleFlashPoint);

        // Настраиваем интенсивность для разного оружия
        MuzzleFlash flashScript = flash.GetComponent<MuzzleFlash>();
        if (flashScript != null)
        {
            // Можно настроить индивидуально для каждого оружия
        }

        Light flashLight = flash.GetComponent<Light>();
        if (flashLight != null)
        {
            flashLight.intensity *= flashIntensityMultiplier;
        }
    }
}