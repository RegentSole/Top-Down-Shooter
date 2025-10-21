using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;

    private bool isPaused = false;

    void Update()
    {
        // Обработка нажатия ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Обработка действий в меню паузы
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                ExitGame();
            }
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(isPaused);
        }

        // Пауза игры
        Time.timeScale = isPaused ? 0f : 1f;
    }

    void RestartGame()
    {
        // Возобновляем время перед перезагрузкой
        Time.timeScale = 1f;

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ExitGame()
    {
        // В редакторе останавливаем воспроизведение, в билде - закрываем приложение
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}