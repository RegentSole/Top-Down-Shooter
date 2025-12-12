using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI restartHintText;
    [SerializeField] private TextMeshProUGUI leaveHintText;

    [Header("Настройки")]
    [SerializeField] private string gameOverMessage = "GAME OVER";
    [SerializeField] private float gameOverDelay = 1f; // Задержка перед показом панели
    [SerializeField] private bool pauseTimeOnGameOver = true;

    [Header("Сцены")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isGameOver = false;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        // Паттерн Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Находим здоровье игрока
        FindPlayerHealth();

        // Скрываем панель Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Устанавливаем текст
        if (gameOverText != null)
        {
            gameOverText.text = gameOverMessage;
        }

        if (restartHintText != null)
        {
            restartHintText.text = "R - Restart";
        }

        if (leaveHintText != null)
        {
            leaveHintText.text = "L - Leave";
        }
    }

    private void Update()
    {
        if (!isGameOver) return;

        // Обработка ввода после Game Over
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LeaveGame();
        }
    }

    private void FindPlayerHealth()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Подписываемся на событие смерти игрока
                // Для этого модифицируем PlayerHealth (см. ниже)
            }
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth не найден. GameManager не сможет обработать смерть игрока.");
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Запускаем отложенный показ панели
        StartCoroutine(ShowGameOverPanel());

        // Пауза игры
        if (pauseTimeOnGameOver)
        {
            Time.timeScale = 0f;
        }

        // Отключаем управление игроком (опционально)
        DisablePlayerControls();

        Debug.Log("GAME OVER");
    }

    private System.Collections.IEnumerator ShowGameOverPanel()
    {
        // Небольшая задержка перед показом панели
        yield return new WaitForSecondsRealtime(gameOverDelay);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Анимация появления (опционально)
            CanvasGroup canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                float fadeTime = 0.5f;
                float timer = 0f;

                while (timer < fadeTime)
                {
                    timer += Time.unscaledDeltaTime;
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);
                    yield return null;
                }
            }
        }
    }

    private void DisablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Отключаем скрипты управления
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this && script.enabled)
                {
                    script.enabled = false;
                }
            }

            // Отключаем вращение камеры, если есть
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.enabled = false;
                }
            }
        }
    }

    private void EnablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                script.enabled = true;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.enabled = true;
                }
            }
        }
    }

    public void RestartGame()
    {
        // Возвращаем нормальную скорость времени
        Time.timeScale = 1f;

        // Перезагружаем текущую сцену
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        // Сброс состояния
        isGameOver = false;

        Debug.Log("Игра перезапущена");
    }

    public void LeaveGame()
    {
        // Возвращаем нормальную скорость времени
        Time.timeScale = 1f;

        // Выход в главное меню или из игры
        if (!string.IsNullOrEmpty(mainMenuScene))
        {
            try
            {
                SceneManager.LoadScene(mainMenuScene);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Не удалось загрузить сцену {mainMenuScene}: {e.Message}");
                Application.Quit();
            }
        }
        else
        {
            // Выход из игры
            Application.Quit();

            // Для тестирования в редакторе
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        Debug.Log("Выход из игры");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    // Метод для принудительной установки Game Over
    public void ForceGameOver()
    {
        GameOver();
    }

    // Обработка смены сцены
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerHealth();
        isGameOver = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}