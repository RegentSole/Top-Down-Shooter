using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon References")]
    public List<Weapon> weapons = new List<Weapon>();
    public int currentWeaponIndex = 0;

    [Header("UI References")]
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI ammoText;

    private WeaponAim weaponAim;

    void Start()
    {
        weaponAim = GetComponentInChildren<WeaponAim>();

        // Инициализируем все оружие
        InitializeWeapons();

        // Активируем начальное оружие
        SwitchWeapon(0);
        UpdateUI();
    }

    void InitializeWeapons()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.SetVisible(false);
        }
    }

    void Update()
    {
        // Переключение оружия колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? 1 : -1;
            SwitchWeapon(currentWeaponIndex + direction);
        }

        // Переключение оружия цифровыми клавишами
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);

        // Перезарядка
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapons[currentWeaponIndex].Reload();
            UpdateUI();
        }

        // Стрельба
        if (Input.GetButton("Fire1"))
        {
            weapons[currentWeaponIndex].Shoot();
            UpdateUI();
        }
    }

    void SwitchWeapon(int newIndex)
    {
        // Зацикливаем индекс оружия
        if (newIndex < 0) newIndex = weapons.Count - 1;
        if (newIndex >= weapons.Count) newIndex = 0;

        // Проверяем, доступно ли оружие
        if (!weapons[newIndex].isAvailable) return;

        // Скрываем текущее оружие
        weapons[currentWeaponIndex].SetVisible(false);

        // Переключаемся на новое оружие
        currentWeaponIndex = newIndex;

        // Показываем новое оружие
        weapons[currentWeaponIndex].SetVisible(true);

        // Обновляем прицеливание для нового оружия
        if (weaponAim != null)
        {
            weaponAim.firePoint = weapons[currentWeaponIndex].firePoint;
        }

        UpdateUI();
        Debug.Log("Switched to: " + weapons[currentWeaponIndex].weaponName);
    }

    void UpdateUI()
    {
        if (weaponText != null)
        {
            weaponText.text = weapons[currentWeaponIndex].weaponName;
        }

        if (ammoText != null)
        {
            string ammoDisplay = weapons[currentWeaponIndex].infiniteAmmo ? "∞" : weapons[currentWeaponIndex].ammo.ToString();
            ammoText.text = "Ammo: " + ammoDisplay + "/" + weapons[currentWeaponIndex].maxAmmo;
        }
    }

    public void PickupWeapon(string weaponName)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].weaponName == weaponName)
            {
                weapons[i].isAvailable = true;
                SwitchWeapon(i);
                Debug.Log("Picked up: " + weaponName);
                return;
            }
        }

        Debug.LogWarning("Weapon not found: " + weaponName);
    }
}