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
    public float recoilAmount = 0.1f;
    public float recoilRecoverySpeed = 5f;

    [Header("Shell Ejection")]
    public GameObject shellPrefab;
    public Transform ejectionPoint;
    public float ejectionForce = 3f;
    public float ejectionTorque = 10f;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPoint;
    public float flashIntensityMultiplier = 1f;

    [Header("Audio")]
    public AudioClip shootSound;
    public float volume = 1f;

    [Header("Weapon Switch Sound")]
    public AudioClip equipSound;
    public float equipVolume = 0.7f;

    [Header("Empty Mag Sound")]
    public AudioClip emptySound;
    public float emptySoundVolume = 0.5f;

    [Header("Visual Settings")]
    public Sprite weaponSprite;
    public Vector3 equippedPosition = new Vector3(0.2f, 0.1f, 0f);

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;


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

    protected virtual void PlayShootSound()
    {
        if (shootSound != null)
        {
            // Простой способ, который всегда работает
            AudioSource.PlayClipAtPoint(shootSound, transform.position, volume);
            Debug.Log("Sound played via PlayClipAtPoint");
        }
        else
        {
            Debug.LogError("No shoot sound assigned!");
        }
    }

    protected virtual void PlayEmptySound()
    {
        if (emptySound != null)
        {
            AudioSource.PlayClipAtPoint(emptySound, Camera.main.transform.position, emptySoundVolume);
            Debug.Log("Played empty mag sound");
        }
        else
        {
            Debug.LogWarning("No empty sound assigned!");
        }
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

    /*public virtual bool TryShoot()
    {
        if (CanShoot())
        {
            Shoot();
            return true;
        }
        else
        {
            // Воспроизводим звук пустого магазина если нет патронов
            if (!infiniteAmmo && ammo <= 0)
            {
                PlayEmptySound();
            }
            return false;
        }
    }*/

    public void CheckAmmoAndPlaySound()
    {
        if (!infiniteAmmo && ammo <= 0 && emptySound != null)
        {
            AudioSource.PlayClipAtPoint(emptySound, Camera.main.transform.position, emptySoundVolume);
        }
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

        // Запоминаем текущую позицию и поворот
        Vector3 flashPosition = muzzleFlashPoint.position;
        Quaternion flashRotation = muzzleFlashPoint.rotation;

        // Создаем вспышку в запомненной позиции
        GameObject flash = Instantiate(muzzleFlashPrefab, flashPosition, flashRotation);

        // Запускаем корутину для микро-корректировки позиции
        StartCoroutine(AdjustFlashPosition(flash, muzzleFlashPoint));
    }

    private System.Collections.IEnumerator AdjustFlashPosition(GameObject flash, Transform target)
    {
        // Ждем до конца кадра, чтобы все трансформы обновились
        yield return new WaitForEndOfFrame();

        // Корректируем позицию
        if (flash != null && target != null)
        {
            flash.transform.position = target.position;
            flash.transform.rotation = target.rotation;
        }
    }

    // Добавьте в класс Weapon

    protected virtual void ApplyRecoil()
    {
        StartCoroutine(RecoilCoroutine());
    }

    protected virtual void ShakeCamera()
    {
        if (CameraShake.Instance != null)
        {
            Debug.Log("Shaking camera!"); // Для отладки
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
        }
        else
        {
            Debug.LogWarning("CameraShake Instance is null!");
        }
    }

    System.Collections.IEnumerator RecoilCoroutine()
    {
        Vector3 originalPosition = transform.localPosition;
        Vector3 recoilPosition = originalPosition - new Vector3(recoilAmount, 0, 0);

        // Отдача
        float elapsedTime = 0f;
        while (elapsedTime < 0.05f)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, elapsedTime / 0.05f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Возврат
        elapsedTime = 0f;
        while (elapsedTime < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, elapsedTime / 0.1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    // Вызывайте ApplyRecoil() в каждом Shoot после CreateMuzzleFlash()

    public void PlayEquipSound()
    {
        if (equipSound != null)
        {
            AudioSource.PlayClipAtPoint(equipSound, transform.position, equipVolume);
        }
    }
}