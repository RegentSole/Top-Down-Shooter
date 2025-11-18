using UnityEngine;

public class HealEffect : MonoBehaviour
{
    [Header("Particle Settings")]
    public ParticleSystem healParticles;
    public int healParticleCount = 15;

    [Header("Light Settings")]
    public Light healLight;
    public float lightDuration = 0.5f;
    public float lightIntensity = 2f;

    void Start()
    {
        PlayHealEffect();
        Destroy(gameObject, 3f); // Автоуничтожение после завершения
    }

    void PlayHealEffect()
    {
        // Запускаем частицы
        if (healParticles != null)
        {
            healParticles.Emit(healParticleCount);
        }

        // Запускаем свет (если есть)
        if (healLight != null)
        {
            StartCoroutine(AnimateLight());
        }
    }

    System.Collections.IEnumerator AnimateLight()
    {
        healLight.enabled = true;
        healLight.intensity = lightIntensity;

        float timer = 0f;
        while (timer < lightDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / lightDuration;

            // Плавное затухание света
            healLight.intensity = Mathf.Lerp(lightIntensity, 0f, progress);

            yield return null;
        }

        healLight.enabled = false;
    }
}