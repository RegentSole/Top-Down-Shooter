using UnityEngine;

public class SpriteDissolveSetup : MonoBehaviour
{
    [Header("Dissolve Material")]
    public Material dissolveMaterial;

    [Header("Noise Texture")]
    public Texture2D noiseTexture;

    void Start()
    {
        SetupDissolveMaterial();
    }

    [ContextMenu("Setup Dissolve Material")]
    void SetupDissolveMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found!");
            return;
        }

        if (dissolveMaterial == null)
        {
            Debug.LogError("No dissolve material assigned!");
            return;
        }

        // Создаем новый экземпляр материала
        Material materialInstance = new Material(dissolveMaterial);

        // Назначаем текстуру шума
        if (noiseTexture != null)
        {
            materialInstance.SetTexture("_NoiseTex", noiseTexture);
        }

        // Назначаем материал спрайту
        sr.material = materialInstance;

        Debug.Log("Dissolve material setup complete!");
        Debug.Log($"Sprite: {sr.sprite.name}");
        Debug.Log($"Material: {materialInstance.name}");
        Debug.Log($"Noise texture: {noiseTexture != null}");
    }

    void Update()
    {
        // Тест dissolve эффекта
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartDissolveTest();
        }
    }

    void StartDissolveTest()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.material != null)
        {
            StartCoroutine(DissolveTestRoutine());
        }
    }

    System.Collections.IEnumerator DissolveTestRoutine()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float duration = 2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float dissolveAmount = timer / duration;
            sr.material.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }

        // Сброс
        sr.material.SetFloat("_DissolveAmount", 0f);
    }
}