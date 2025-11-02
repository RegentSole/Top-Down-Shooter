using UnityEngine;

public class DissolveTester : MonoBehaviour
{
    public KeyCode testKey = KeyCode.T;
    public bool testOnStart = false;

    private DissolveEffect dissolveEffect;
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material dissolveMaterial;

    void Start()
    {
        dissolveEffect = GetComponent<DissolveEffect>();
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }

        if (testOnStart)
        {
            StartDissolveTest();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            StartDissolveTest();
        }

        // Сброс эффекта по R
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDissolveTest();
        }
    }

    void StartDissolveTest()
    {
        if (dissolveEffect != null)
        {
            Debug.Log("Starting dissolve effect via DissolveEffect component");
            dissolveEffect.StartDissolve();
        }
        else
        {
            Debug.LogError("No DissolveEffect component found!");
        }
    }

    void ResetDissolveTest()
    {
        if (dissolveEffect != null)
        {
            dissolveEffect.ResetDissolve();
            Debug.Log("Dissolve effect reset");
        }
    }

    // Визуальная отладка в редакторе
    void OnDrawGizmos()
    {
        if (dissolveEffect != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.5f, "DissolveIcon.png", true);
        }
    }
}