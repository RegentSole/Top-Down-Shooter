using UnityEngine;
using System.Collections;

public class AnimatedExitDoor : MonoBehaviour
{
    [Header("Door Parts")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Animation Settings")]
    public float openDistance = 2f;
    public float openSpeed = 2f;
    public float closeSpeed = 2f;
    public float closeDelay = 1f; // Задержка перед закрытием после прохождения игрока

    [Header("Door States")]
    public bool isLocked = true;
    public bool isOpen = false;

    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip unlockSound;

    private Vector3 leftDoorStartPos;
    private Vector3 rightDoorStartPos;
    private Vector3 leftDoorOpenPos;
    private Vector3 rightDoorOpenPos;

    private AudioSource audioSource;
    private Coroutine animationCoroutine;
    private bool playerHasPassed = false;

    void Start()
    {
        // Сохраняем начальные позиции дверей
        leftDoorStartPos = leftDoor.position;
        rightDoorStartPos = rightDoor.position;

        // Рассчитываем позиции открытия
        leftDoorOpenPos = leftDoorStartPos + Vector3.left * openDistance;
        rightDoorOpenPos = rightDoorStartPos + Vector3.right * openDistance;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void UnlockAndOpen()
    {
        if (isLocked)
        {
            isLocked = false;

            if (unlockSound != null)
            {
                audioSource.PlayOneShot(unlockSound);
            }

            Debug.Log("Door unlocked!");
        }

        if (!isOpen && !isLocked)
        {
            OpenDoors();
        }
    }

    void OpenDoors()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(OpenDoorsAnimation());
    }

    IEnumerator OpenDoorsAnimation()
    {
        isOpen = true;

        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * openSpeed;
            leftDoor.position = Vector3.Lerp(leftDoorStartPos, leftDoorOpenPos, progress);
            rightDoor.position = Vector3.Lerp(rightDoorStartPos, rightDoorOpenPos, progress);
            yield return null;
        }

        Debug.Log("Doors fully opened");
    }

    void CloseDoors()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(CloseDoorsAnimation());
    }

    IEnumerator CloseDoorsAnimation()
    {
        yield return new WaitForSeconds(closeDelay);

        if (closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        Vector3 currentLeftPos = leftDoor.position;
        Vector3 currentRightPos = rightDoor.position;

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * closeSpeed;
            leftDoor.position = Vector3.Lerp(currentLeftPos, leftDoorStartPos, progress);
            rightDoor.position = Vector3.Lerp(currentRightPos, rightDoorStartPos, progress);
            yield return null;
        }

        isOpen = false;
        playerHasPassed = false;
        Debug.Log("Doors closed");
    }

    // Вызывается когда игрок проходит через дверь
    public void OnPlayerPassed()
    {
        if (!playerHasPassed && isOpen)
        {
            playerHasPassed = true;
            CloseDoors();
        }
    }

    // Для отладки
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            UnlockAndOpen();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CloseDoors();
        }
    }

    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        if (leftDoor != null && rightDoor != null)
        {
            Gizmos.color = isLocked ? Color.red : Color.green;
            Gizmos.DrawLine(leftDoor.position, rightDoor.position);
            Gizmos.DrawWireCube(leftDoor.position, Vector3.one * 0.3f);
            Gizmos.DrawWireCube(rightDoor.position, Vector3.one * 0.3f);
        }
    }
}