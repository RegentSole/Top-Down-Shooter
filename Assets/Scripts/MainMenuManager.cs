using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Кнопки меню")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button settingsButton; // опционально

    [Header("Настройки сцен")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private float sceneTransitionDelay = 0.5f;

    [Header("Анимации и эффекты")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private string fadeOutTrigger = "FadeOut";
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip menuMusic;

    [Header("Настройки мыши")]
    [SerializeField] private bool showCursor = true;
    [SerializeField] private CursorLockMode cursorLockMode = CursorLockMode.None;

    private AudioSource audioSource;

    void Start()
    {
        if (showCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = cursorLockMode;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        SetupButtons();

        CheckScenes();
    }

    void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayGame);

            StartCoroutine(WaitForEnterKey());
        }

        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(QuitGame);

            StartCoroutine(WaitForEscapeKey());
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OpenSettings);
        }
    }

    void CheckScenes()
    {
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName == gameSceneName)
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError($"Сцена '{gameSceneName}' не найдена в Build Settings!");
            if (playButton != null)
            {
                playButton.interactable = false;
                TextMeshProUGUI playText = playButton.GetComponentInChildren<TextMeshProUGUI>();
                if (playText != null)
                {
                    playText.text = "Scene Not Found";
                }
            }
        }
    }

    System.Collections.IEnumerator WaitForEnterKey()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                PlayGame();
                yield break;
            }
            yield return null;
        }
    }

    System.Collections.IEnumerator WaitForEscapeKey()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
                yield break;
            }
            yield return null;
        }
    }

    public void PlayGame()
    {
        PlayButtonSound();

        if (playButton != null) playButton.interactable = false;
        if (leaveButton != null) leaveButton.interactable = false;

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger(fadeOutTrigger);
        }

        StartCoroutine(LoadGameSceneWithDelay());
    }

    System.Collections.IEnumerator LoadGameSceneWithDelay()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);

        if (!string.IsNullOrEmpty(gameSceneName))
        {
            try
            {
                SceneManager.LoadScene(gameSceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Не удалось загрузить сцену '{gameSceneName}': {e.Message}");

                if (playButton != null) playButton.interactable = true;
                if (leaveButton != null) leaveButton.interactable = true;
            }
        }
        else
        {
            Debug.LogError("Имя сцены не указано!");
        }
    }

    public void QuitGame()
    {
        PlayButtonSound();

        if (playButton != null) playButton.interactable = false;
        if (leaveButton != null) leaveButton.interactable = false;

        Debug.Log("Игра закрывается...");

        StartCoroutine(QuitWithDelay());
    }

    System.Collections.IEnumerator QuitWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OpenSettings()
    {
        PlayButtonSound();
        Debug.Log("Открытие настроек...");
        // Здесь можно добавить логику открытия меню настроек
    }

    void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void LoadLevel(string levelName)
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            gameSceneName = levelName;
            PlayGame();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayGame();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
    }
}