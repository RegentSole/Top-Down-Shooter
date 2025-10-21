using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIcons : MonoBehaviour
{
    [Header("Sprint Settings")]
    public Image sprintIcon;
    public Color sprintActiveColor = Color.gray;
    public Color sprintInactiveColor = Color.white;

    [Header("Dash Settings")]
    public Image dashIcon;
    public Color dashActiveColor = Color.gray;
    public Color dashInactiveColor = Color.white;
    public Image dashCooldownOverlay; // Для визуализации перезарядки

    private PlayerMovement playerMovement;
    private float dashCooldown;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement != null)
        {
            dashCooldown = playerMovement.dashCooldown;
        }

        // Скрываем overlay перезарядки в начале
        if (dashCooldownOverlay != null)
        {
            dashCooldownOverlay.fillAmount = 1;
        }
    }

    void Update()
    {
        UpdateSprintIcon();
        UpdateDashIcon();
    }

    void UpdateSprintIcon()
    {
        if (sprintIcon == null || playerMovement == null) return;

        // Меняем цвет иконки в зависимости от состояния спринта
        sprintIcon.color = playerMovement.IsRunning ? sprintActiveColor : sprintInactiveColor;
    }

    void UpdateDashIcon()
    {
        if (dashIcon == null || dashCooldownOverlay == null || playerMovement == null) return;

        // Меняем цвет иконки в зависимости от состояния перезарядки
        dashIcon.color = playerMovement.IsDashing || playerMovement.dashCooldownTimer > 0 ? dashActiveColor : dashInactiveColor;

        // Обновляем заполнение перезарядки
        if (playerMovement.dashCooldownTimer > 0)
        {
            float cooldownProgress = 1 - (playerMovement.dashCooldownTimer / dashCooldown);
            dashCooldownOverlay.fillAmount = cooldownProgress;
        }
        else
        {
            dashCooldownOverlay.fillAmount = 0;
        }
    }
}