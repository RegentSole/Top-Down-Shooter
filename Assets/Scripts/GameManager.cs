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
        FindPlayerHealth();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

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

        StartCoroutine(ShowGameOverPanel());

        if (pauseTimeOnGameOver)
        {
            Time.timeScale = 0f;
        }

        DisablePlayerControls();

        Debug.Log("GAME OVER");
    }

    private System.Collections.IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

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
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this && script.enabled)
                {
                    script.enabled = false;
                }
            }

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
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        isGameOver = false;

        Debug.Log("Игра перезапущена");
    }

    public void LeaveGame()
    {
        Time.timeScale = 1f;

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
            Application.Quit();

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

    public void ForceGameOver()
    {
        GameOver();
    }

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