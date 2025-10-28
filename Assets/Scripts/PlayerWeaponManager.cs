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

    void Start()
    {
        InitializeWeapons();
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
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2); // Добавили для автомата

        // Перезарядка
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapons[currentWeaponIndex].Reload();
            UpdateUI();
        }

        // Стрельба - для автомата используем автоматический огонь при зажатой кнопке
        if (Input.GetButton("Fire1"))
        {
            // Для автомата разрешаем автоматическую стрельбу
            if (weapons[currentWeaponIndex] is Automat)
            {
                weapons[currentWeaponIndex].Shoot();
            }
            else
            {
                // Для других оружий - одиночные выстрелы
                if (Input.GetButtonDown("Fire1"))
                {
                    weapons[currentWeaponIndex].Shoot();
                }
            }
            UpdateUI();
        }
    }

    void SwitchWeapon(int newIndex)
    {
        if (newIndex < 0) newIndex = weapons.Count - 1;
        if (newIndex >= weapons.Count) newIndex = 0;

        if (!weapons[newIndex].isAvailable) return;

        weapons[currentWeaponIndex].SetVisible(false);
        currentWeaponIndex = newIndex;
        weapons[currentWeaponIndex].SetVisible(true);

        UpdateUI();
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
                // Добавляем патроны при подборе
                if (!weapons[i].infiniteAmmo)
                {
                    weapons[i].ammo = Mathf.Min(weapons[i].ammo + 30, weapons[i].maxAmmo);
                }
                SwitchWeapon(i);
                Debug.Log("Picked up: " + weaponName);
                return;
            }
        }

        Debug.LogWarning("Weapon not found: " + weaponName);
    }
}