using UnityEngine;

public class BulletRotation : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Поворачиваем пулю в направлении движения
        if (rb != null && rb.velocity != Vector2.zero)
        {
            // Вычисляем угол движения
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            // Применяем поворот (вершиной треугольника вперед)
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}