using UnityEngine;
using UnityEngine.UI;

public class SimpleUpDownMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 2f;        // Скорость движения
    [SerializeField] private float moveDistance = 50f;    // Расстояние движения (в пикселях/юнитах)
    [SerializeField] private bool startMovingUp = true;   // Начинать движение вверх?

    [Header("Тип движения")]
    [SerializeField] private MovementType movementType = MovementType.Smooth;

    private Vector3 startPosition;
    private float timer = 0f;
    private bool movingUp = true;

    private enum MovementType
    {
        Smooth,     // Плавное синусоидальное движение
        PingPong,   // Движение вперед-назад
        Bounce      // С отскоком
    }

    void Start()
    {
        // Сохраняем начальную позицию
        startPosition = transform.position;
        movingUp = startMovingUp;

        // Если используем синус, сбрасываем таймер в зависимости от направления
        if (movementType == MovementType.Smooth)
        {
            timer = startMovingUp ? 0f : Mathf.PI;
        }
    }

    void Update()
    {
        switch (movementType)
        {
            case MovementType.Smooth:
                SmoothMovement();
                break;

            case MovementType.PingPong:
                PingPongMovement();
                break;

            case MovementType.Bounce:
                BounceMovement();
                break;
        }
    }

    // Плавное синусоидальное движение
    void SmoothMovement()
    {
        timer += Time.deltaTime * moveSpeed;

        // Используем синус для плавного движения вверх-вниз
        float yOffset = Mathf.Sin(timer) * moveDistance;

        // Обновляем позицию
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    // Движение вперед-назад
    void PingPongMovement()
    {
        // Используем PingPong для движения между 0 и moveDistance
        float yOffset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

        // Если начинаем движение вниз, инвертируем
        if (!startMovingUp)
        {
            yOffset = moveDistance - yOffset;
        }

        // Обновляем позицию
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    // Движение с отскоком
    void BounceMovement()
    {
        // Определяем направление движения
        float direction = movingUp ? 1f : -1f;

        // Двигаем объект
        transform.Translate(0, direction * moveSpeed * Time.deltaTime, 0);

        // Проверяем границы
        float currentY = transform.position.y;

        if (movingUp && currentY >= startPosition.y + moveDistance)
        {
            movingUp = false;
        }
        else if (!movingUp && currentY <= startPosition.y - moveDistance)
        {
            movingUp = true;
        }
    }

    // Для отладки: визуализация диапазона движения в редакторе
    void OnDrawGizmosSelected()
    {
        // Рисуем линию, показывающую диапазон движения
        Gizmos.color = Color.cyan;
        Vector3 currentPos = Application.isPlaying ? startPosition : transform.position;

        Gizmos.DrawLine(
            currentPos + new Vector3(-20, moveDistance, 0),
            currentPos + new Vector3(20, moveDistance, 0)
        );

        Gizmos.DrawLine(
            currentPos + new Vector3(-20, -moveDistance, 0),
            currentPos + new Vector3(20, -moveDistance, 0)
        );
    }
}