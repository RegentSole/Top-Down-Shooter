using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    private Vector3 originalPosition;
    private float shakeTimer = 0f;
    private bool isShaking = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking && shakeTimer > 0)
        {
            // Случайное смещение в локальных координатах
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else if (isShaking && shakeTimer <= 0)
        {
            // Возврат к исходной позиции
            transform.localPosition = originalPosition;
            isShaking = false;
        }
    }

    public void Shake()
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition;
        }

        shakeTimer = shakeDuration;
        isShaking = true;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        Shake();
    }
}