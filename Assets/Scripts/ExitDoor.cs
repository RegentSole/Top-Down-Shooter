using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isLocked = true;
    public GameObject lockedVisual;
    public GameObject unlockedVisual;

    [Header("Audio")]
    public AudioClip unlockSound;
    public AudioClip openSound;

    private AudioSource audioSource;
    private bool isOpen = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateDoorVisual();
    }

    void UpdateDoorVisual()
    {
        if (lockedVisual != null)
        {
            lockedVisual.SetActive(isLocked);
        }
        if (unlockedVisual != null)
        {
            unlockedVisual.SetActive(!isLocked);
        }
    }

    public void Unlock()
    {
        if (isLocked)
        {
            isLocked = false;

            if (unlockSound != null)
            {
                audioSource.PlayOneShot(unlockSound);
            }

            UpdateDoorVisual();
            Debug.Log("Door unlocked!");
        }
    }

    public void Open()
    {
        if (!isLocked && !isOpen)
        {
            isOpen = true;

            if (openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }

            // Анимация открытия или просто отключение
            gameObject.SetActive(false);
            Debug.Log("Door opened!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isLocked)
        {
            Open();

            // Здесь можно перейти на следующий уровень
            Debug.Log("Level completed!");

            // Пример перехода на следующий уровень:
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Для отладки
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Unlock();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
        }
    }
}