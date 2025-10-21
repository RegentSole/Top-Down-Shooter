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

    [Header("Visual Settings")]
    public Sprite weaponSprite;
    public Vector3 equippedPosition = new Vector3(0.3f, 0.1f, 0f);

    protected SpriteRenderer spriteRenderer;
    protected float nextFireTime;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (weaponSprite != null)
        {
            spriteRenderer.sprite = weaponSprite;
        }

        spriteRenderer.sortingLayerName = "Weapons";
        spriteRenderer.sortingOrder = 1;

        transform.localPosition = equippedPosition;
    }

    public abstract void Shoot();

    public virtual void Reload()
    {
        ammo = maxAmmo;
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
}