using UnityEngine;
using UnityEngine.UI;

public class SimpleVolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private string saveKey = "GameVolume";
    [SerializeField] private float defaultVolume = 0.7f;

    private void Start()
    {
        if (volumeSlider == null)
        {
            volumeSlider = GetComponent<Slider>();
        }

        if (volumeSlider != null)
        {
            // Загружаем сохраненное значение
            float savedVolume = PlayerPrefs.GetFloat(saveKey, defaultVolume);
            volumeSlider.value = savedVolume;

            // Применяем начальную громкость
            ApplyVolume(savedVolume);

            // Добавляем обработчик
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    private void OnSliderChanged(float value)
    {
        ApplyVolume(value);

        // Сохраняем
        PlayerPrefs.SetFloat(saveKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float volume)
    {
        // Устанавливаем громкость для всех AudioListener
        AudioListener.volume = volume;

        // Дополнительно для всех AudioSource (если нужно)
        AudioSource[] allSources = FindObjectsOfType<AudioSource>(true);
        foreach (AudioSource source in allSources)
        {
            source.volume = volume;
        }
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }
}