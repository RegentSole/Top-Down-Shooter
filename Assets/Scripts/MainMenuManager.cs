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
        // Настройка курсора
        if (showCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = cursorLockMode;
        }

        // Инициализация аудио
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Проигрываем фоновую музыку
        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Настройка кнопок
        SetupButtons();

        // Проверяем, существует ли указанная сцена
        CheckScenes();
    }

    void SetupButtons()
    {
        // Кнопка Play
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayGame);

            // Добавляем горячую клавишу Enter
            StartCoroutine(WaitForEnterKey());
        }

        // Кнопка Leave/Quit
        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(QuitGame);

            // Добавляем горячую клавишу Escape
            StartCoroutine(WaitForEscapeKey());
        }

        // Опциональная кнопка настроек
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OpenSettings);
        }
    }

    void CheckScenes()
    {
        // Проверяем, существует ли сцена с игрой
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
        // Проигрываем звук нажатия
        PlayButtonSound();

        // Отключаем кнопки, чтобы предотвратить множественные нажатия
        if (playButton != null) playButton.interactable = false;
        if (leaveButton != null) leaveButton.interactable = false;

        // Запускаем анимацию перехода
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger(fadeOutTrigger);
        }

        // Загружаем игровую сцену с задержкой
        StartCoroutine(LoadGameSceneWithDelay());
    }

    System.Collections.IEnumerator LoadGameSceneWithDelay()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);

        // Загружаем игровую сцену
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            try
            {
                SceneManager.LoadScene(gameSceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Не удалось загрузить сцену '{gameSceneName}': {e.Message}");

                // Включаем кнопки обратно при ошибке
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
        // Проигрываем звук нажатия
        PlayButtonSound();

        // Отключаем кнопки
        if (playButton != null) playButton.interactable = false;
        if (leaveButton != null) leaveButton.interactable = false;

        // Показываем сообщение о выходе
        Debug.Log("Игра закрывается...");

        // Задержка перед выходом (для анимации)
        StartCoroutine(QuitWithDelay());
    }

    System.Collections.IEnumerator QuitWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        // Выход из игры
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

    // Метод для изменения громкости музыки
    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }

    // Метод для загрузки конкретного уровня (если у вас несколько уровней)
    public void LoadLevel(string levelName)
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            gameSceneName = levelName;
            PlayGame();
        }
    }

    // Для отладки в редакторе
    void Update()
    {
        // Быстрые клавиши для тестирования
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