using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestNPC : MonoBehaviour
{
    [Header("Quest Settings")]
    public int requiredKeys = 3;
    public GameObject exitDoor;
    public string[] dialogueLines = {
        "Привет! Мне нужна помощь...",
        "Чтобы открыть дверь выхода, нужно найти 3 ключа.",
        "Они разбросаны по этой территории.",
        "Принеси их мне, и я открою дверь."
    };

    [Header("UI References")]
    public GameObject interactionHint;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typeWriterSpeed = 0.05f;

    [Header("Дверь и эффекты")]
    public bool useDissolveForDoor = true;
    public float doorDissolveDuration = 2f;
    public Color doorDissolveColor = Color.blue;
    public GameObject doorOpenEffect; // Эффект при открытии двери

    [Header("Audio")]
    public AudioClip talkSound;
    public AudioClip completeSound;
    public AudioClip doorOpenSound;

    [Header("После завершения квеста")]
    public bool disableAfterQuest = true;
    public GameObject questCompleteIndicator; // Индикатор завершения квеста
    public string completedQuestText = "Спасибо! Дверь открыта.";

    private bool isPlayerInRange = false;
    private bool isTalking = false;
    private bool questCompleted = false;
    private int currentLine = 0;
    private AudioSource audioSource;
    private PlayerKeyManager keyManager;
    private DissolveEffect doorDissolveEffect;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }

        if (exitDoor != null && useDissolveForDoor)
        {
            doorDissolveEffect = exitDoor.GetComponent<DissolveEffect>();
            if (doorDissolveEffect == null)
            {
                doorDissolveEffect = exitDoor.AddComponent<DissolveEffect>();
            }

            if (doorDissolveEffect != null)
            {
                doorDissolveEffect.dissolveDuration = doorDissolveDuration;
                doorDissolveEffect.startDelay = 0.5f;
                doorDissolveEffect.destroyOnComplete = false;
            }
        }

        if (questCompleteIndicator != null)
        {
            questCompleteIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (!isPlayerInRange || questCompleted) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        currentLine = 0;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        keyManager = FindObjectOfType<PlayerKeyManager>();

        if (keyManager != null && keyManager.keysCollected >= requiredKeys && !questCompleted)
        {
            CompleteQuest();
            return;
        }

        PlayTalkSound();
        StartCoroutine(TypeDialogue(dialogueLines[currentLine]));
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            PlayTalkSound();
            StopAllCoroutines();
            StartCoroutine(TypeDialogue(dialogueLines[currentLine]));
        }
        else
        {
            EndDialogue();
        }
    }

    System.Collections.IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeWriterSpeed);
        }
    }

    void EndDialogue()
    {
        isTalking = false;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void CompleteQuest()
    {
        if (questCompleted) return;

        questCompleted = true;

        StopAllCoroutines();
        dialogueText.text = completedQuestText;

        OpenDoorWithDissolve();

        if (completeSound != null)
        {
            audioSource.PlayOneShot(completeSound);
        }

        if (questCompleteIndicator != null)
        {
            questCompleteIndicator.SetActive(true);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }

        if (disableAfterQuest)
        {
            StartCoroutine(DisableNPCAfterDelay(3f));
        }

        Debug.Log("Quest completed! Door is opening with dissolve effect.");
    }

    void OpenDoorWithDissolve()
    {
        if (doorOpenEffect != null)
        {
            Instantiate(doorOpenEffect, exitDoor.transform.position, Quaternion.identity);
        }

        if (doorOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(doorOpenSound, exitDoor.transform.position, 1f);
        }

        if (useDissolveForDoor && doorDissolveEffect != null)
        {
            doorDissolveEffect.StartDissolve();

            Collider2D doorCollider = exitDoor.GetComponent<Collider2D>();
            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }
        }
        else
        {
            // Резервный вариант - старый метод
            AnimatedExitDoor animatedDoor = exitDoor.GetComponent<AnimatedExitDoor>();
            if (animatedDoor != null)
            {
                animatedDoor.UnlockAndOpen();
            }
            else
            {
                ExitDoor oldDoor = exitDoor.GetComponent<ExitDoor>();
                if (oldDoor != null)
                {
                    oldDoor.Unlock();
                }
                else
                {
                    exitDoor.SetActive(false);
                }
            }
        }
    }

    System.Collections.IEnumerator DisableNPCAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        EndDialogue();

        this.enabled = false;

        // Меняем спрайт NPC (опционально)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        Debug.Log("NPC отключен после завершения квеста");
    }

    void PlayTalkSound()
    {
        if (talkSound != null)
        {
            audioSource.PlayOneShot(talkSound);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !questCompleted)
        {
            isPlayerInRange = true;
            if (interactionHint != null)
            {
                interactionHint.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionHint != null && !questCompleted)
            {
                interactionHint.SetActive(false);
            }
            if (isTalking)
            {
                EndDialogue();
            }
        }
    }

    public void ForceCompleteQuest()
    {
        if (!questCompleted)
        {
            CompleteQuest();
        }
    }

    public bool IsQuestCompleted()
    {
        return questCompleted;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = questCompleted ? Color.gray : Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        if (exitDoor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, exitDoor.transform.position);
        }
    }
}