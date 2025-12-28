using UnityEngine;
using TMPro;

public class EscapeZone : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject escapePanel;
    [SerializeField] private TextMeshProUGUI escapeText;
    [SerializeField] private TextMeshProUGUI restartHintText;
    [SerializeField] private TextMeshProUGUI leaveHintText;

    [Header("Настройки")]
    [SerializeField] private string escapeMessage = "YOU ESCAPED!";
    [SerializeField] private string secondaryMessage = "Level Complete";
    [SerializeField] private float showDelay = 0.5f;
    [SerializeField] private bool pauseTimeOnEscape = true;

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem winParticles;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private float soundVolume = 0.7f;

    [Header("Автоматические действия")]
    [SerializeField] private bool disablePlayerOnEscape = true;
    [SerializeField] private bool showCursorOnEscape = true;

    [Header("Следующий уровень")]
    [SerializeField] private string nextLevelScene;
    [SerializeField] private bool loadNextLevelAutomatically = false;
    [SerializeField] private float autoLoadDelay = 3f;

    private bool hasEscaped = false;
    private PlayerHealth playerHealth;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (escapePanel != null)
        {
            escapePanel.SetActive(false);
        }

        if (escapeText != null)
        {
            escapeText.text = escapeMessage;
        }

        if (restartHintText != null)
        {
            restartHintText.text = "R - Restart Level";
        }

        if (leaveHintText != null)
        {
            leaveHintText.text = "L - Main Menu";
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (!hasEscaped) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LeaveToMenu();
        }

        if (Input.GetKeyDown(KeyCode.N) && !string.IsNullOrEmpty(nextLevelScene))
        {
            LoadNextLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEscaped) return;

        if (other.CompareTag("Player"))
        {
            Escape();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!hasEscaped && other.CompareTag("Player"))
        {
            Escape();
        }
    }

    private void Escape()
    {
        if (hasEscaped) return;

        hasEscaped = true;

        PlayWinEffects();

        if (disablePlayerOnEscape)
        {
            DisablePlayerControls();
        }

        if (pauseTimeOnEscape)
        {
            Time.timeScale = 0f;
        }

        if (showCursorOnEscape)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        StartCoroutine(ShowEscapePanel());

        if (loadNextLevelAutomatically && !string.IsNullOrEmpty(nextLevelScene))
        {
            StartCoroutine(AutoLoadNextLevel());
        }

        Debug.Log("Игрок сбежал! Уровень пройден.");
    }

    private System.Collections.IEnumerator ShowEscapePanel()
    {
        yield return new WaitForSecondsRealtime(showDelay);

        if (escapePanel != null)
        {
            escapePanel.SetActive(true);

            CanvasGroup canvasGroup = escapePanel.GetComponent<CanvasGroup>();
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

    private System.Collections.IEnumerator AutoLoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(autoLoadDelay);
        LoadNextLevel();
    }

    private void PlayWinEffects()
    {
        if (winParticles != null)
        {
            winParticles.Play();
        }

        if (winSound != null)
        {
            AudioSource.PlayClipAtPoint(winSound, transform.position, soundVolume);
        }

        CameraShake cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.Shake(0.5f, 0.2f);
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
                if (script != this)
                {
                    script.enabled = false;
                }
            }

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
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
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LeaveToMenu()
    {
        Time.timeScale = 1f;

        string menuScene = "MainMenu";
        if (gameManager != null)
        {
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("mainMenuScene",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                menuScene = (string)field.GetValue(gameManager);
            }
        }

        if (!string.IsNullOrEmpty(menuScene))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuScene);
        }
        else
        {
            Debug.LogWarning("Сцена главного меню не указана");
        }
    }

    public void LoadNextLevel()
    {
        if (string.IsNullOrEmpty(nextLevelScene))
        {
            Debug.LogWarning("Следующий уровень не указан");
            return;
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelScene);
    }

    public void ForceEscape()
    {
        if (!hasEscaped)
        {
            Escape();
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null && collider.enabled)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position + (Vector3)collider.offset, collider.size);

            Gizmos.color = Color.green;
            Vector3 center = transform.position + (Vector3)collider.offset;
            Gizmos.DrawLine(center + Vector3.left * 0.3f, center + Vector3.right * 0.3f);
            Gizmos.DrawLine(center + Vector3.up * 0.3f, center + Vector3.down * 0.3f);
        }
    }
}