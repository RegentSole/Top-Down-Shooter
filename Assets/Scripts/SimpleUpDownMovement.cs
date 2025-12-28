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
        startPosition = transform.position;
        movingUp = startMovingUp;

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

    void SmoothMovement()
    {
        timer += Time.deltaTime * moveSpeed;

        float yOffset = Mathf.Sin(timer) * moveDistance;

        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    void PingPongMovement()
    {
        float yOffset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

        if (!startMovingUp)
        {
            yOffset = moveDistance - yOffset;
        }

        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    void BounceMovement()
    {
        float direction = movingUp ? 1f : -1f;

        transform.Translate(0, direction * moveSpeed * Time.deltaTime, 0);

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

    void OnDrawGizmosSelected()
    {
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