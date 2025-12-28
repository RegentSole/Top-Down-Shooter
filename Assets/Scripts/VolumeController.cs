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
            float savedVolume = PlayerPrefs.GetFloat(saveKey, defaultVolume);
            volumeSlider.value = savedVolume;

            ApplyVolume(savedVolume);

            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    private void OnSliderChanged(float value)
    {
        ApplyVolume(value);

        PlayerPrefs.SetFloat(saveKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float volume)
    {
        AudioListener.volume = volume;

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