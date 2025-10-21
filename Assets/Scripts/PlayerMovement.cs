using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float rotationSpeed = 10f;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.2f;

    [Header("References")]
    public Animator animator;
    public ParticleSystem dashParticles;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;
    private float currentSpeed;
    public bool isRunning;
    public bool isDashing;
    public float dashCooldownTimer;
    private float dashTime;
    private Camera cam;
    private Vector2 mousePos;

    // Для визуальной отладки в редакторе
    public bool IsRunning => isRunning;
    public bool IsDashing => isDashing;
    public float CurrentSpeed => currentSpeed;
    public float DashCooldownTimer => dashCooldownTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        if (isDashing) return;

        ProcessInput();
        HandleDash();
        HandleRunning();
        UpdateAnimations();
        RotateTowardsMouse();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDashMovement();
            return;
        }

        HandleMovement();
    }

    void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Сохраняем последнее направление движения для даша
        if (moveX != 0 || moveY != 0)
        {
            lastMoveDirection = new Vector2(moveX, moveY).normalized;
        }

        moveDirection = new Vector2(moveX, moveY).normalized;

        // Получаем позицию мыши для вращения
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void HandleMovement()
    {
        Vector2 targetVelocity = moveDirection * currentSpeed;

        // Плавное ускорение и замедление
        if (moveDirection.magnitude > 0.1f)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    void HandleRunning()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : moveSpeed;
    }

    void HandleDash()
    {
        // Обновляем таймер перезарядки
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Активация даша
        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0 && lastMoveDirection != Vector2.zero)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Сохраняем текущую скорость перед рывком
        Vector2 dashVelocity = lastMoveDirection * (dashDistance / dashDuration);

        // Применяем рывок
        rb.velocity = dashVelocity;

        // Визуальные эффекты
        if (dashParticles != null)
        {
            dashParticles.Play();
        }
    }

    void HandleDashMovement()
    {
        dashTime -= Time.fixedDeltaTime;

        if (dashTime <= 0)
        {
            EndDash();
        }
        else
        {
            // Плавно уменьшаем скорость во время даша
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.1f);
        }
    }

    void EndDash()
    {
        isDashing = false;

        // ВОССТАНАВЛИВАЕМ КОНТРОЛЬ НАД ПЕРСОНАЖЕМ
        // Сбрасываем скорость к нормальному значению
        if (moveDirection.magnitude > 0.1f)
        {
            // Если есть input, устанавливаем нормальную скорость движения
            rb.velocity = moveDirection * currentSpeed;
        }
        else
        {
            // Если input нет, останавливаем персонажа
            rb.velocity = Vector2.zero;
        }
    }

    void RotateTowardsMouse()
    {
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        // Плавное вращение
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", rb.velocity.magnitude);
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsDashing", isDashing);
        }
    }
}