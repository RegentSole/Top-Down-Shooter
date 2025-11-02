using UnityEngine;

public class VisibilityDebug : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        Debug.Log("=== VISIBILITY DEBUG ===");
        Debug.Log($"GameObject: {gameObject.name}");
        Debug.Log($"Renderer: {renderer != null}");
        Debug.Log($"SpriteRenderer: {spriteRenderer != null}");
        Debug.Log($"Renderer enabled: {renderer != null && renderer.enabled}");
        Debug.Log($"GameObject active: {gameObject.activeInHierarchy}");
        
        if (renderer != null)
        {
            Debug.Log($"Material: {renderer.material != null}");
            Debug.Log($"Shader: {renderer.material.shader.name}");
            Debug.Log($"Main texture: {renderer.material.mainTexture != null}");
            
            // Проверяем dissolve параметры
            if (renderer.material.HasProperty("_DissolveAmount"))
            {
                float dissolve = renderer.material.GetFloat("_DissolveAmount");
                Debug.Log($"Dissolve amount: {dissolve}");
            }
        }
    }
    
    void Update()
    {
        // Мигание для проверки видимости
        if (Time.time % 1f < 0.5f)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}