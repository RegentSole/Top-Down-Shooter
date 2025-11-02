using UnityEngine;
using System.Collections;

public class DissolveEffect : MonoBehaviour
{
    [Header("Dissolve Settings")]
    public float dissolveDuration = 1.5f;
    public float startDelay = 0f;
    public bool destroyOnComplete = true;

    [Header("References")]
    public Renderer objectRenderer;
    
    private Material dissolveMaterial;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int DissolveColor = Shader.PropertyToID("_DissolveColor");

    void Start()
    {
        // Автоматически находим рендерер если не назначен
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }

        // Создаем копию материала для работы с шейдером
        if (objectRenderer != null)
        {
            dissolveMaterial = objectRenderer.material;
        }
        else
        {
            Debug.LogWarning("No renderer found for dissolve effect!");
            enabled = false;
        }
    }

    // Запуск dissolve эффекта
    public void StartDissolve()
    {
        StartCoroutine(DissolveRoutine());
    }

    // Запуск с кастомным цветом
    public void StartDissolve(Color dissolveColor)
    {
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetColor(DissolveColor, dissolveColor);
        }
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        // Задержка перед началом
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        float timer = 0f;
        
        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Clamp01(timer / dissolveDuration);
            
            if (dissolveMaterial != null)
            {
                dissolveMaterial.SetFloat(DissolveAmount, dissolveValue);
            }
            
            yield return null;
        }

        // Завершение
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetFloat(DissolveAmount, 1f);
        }

        // Уничтожение объекта если нужно
        if (destroyOnComplete)
        {
            Destroy(gameObject);
        }
    }

    // Метод для сброса эффекта
    public void ResetDissolve()
    {
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetFloat(DissolveAmount, 0f);
        }
        StopAllCoroutines();
    }

    // Автоматический запуск при отключении (для тестирования)
    void OnDisable()
    {
        ResetDissolve();
    }
}