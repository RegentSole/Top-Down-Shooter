using UnityEngine;
using TMPro;

public class FloatingHint : MonoBehaviour
{
    [Header("Настройки анимации")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.2f;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Настройки UI")]
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0);
    [SerializeField] private bool faceCamera = true;

    private Vector3 startPosition;
    private Transform playerCamera;
    private TextMeshProUGUI textMesh;

    void Start()
    {
        startPosition = transform.localPosition;
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);

        if (rotationSpeed > 0)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        if (faceCamera && playerCamera != null)
        {
            transform.LookAt(transform.position + playerCamera.forward);
        }

        if (textMesh != null && playerCamera != null)
        {
            float distance = Vector3.Distance(transform.position, playerCamera.position);
            float alpha = Mathf.Clamp01(1f - (distance - 2f) / 5f);
            Color color = textMesh.color;
            color.a = alpha;
            textMesh.color = color;
        }
    }
}