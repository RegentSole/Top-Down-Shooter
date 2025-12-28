using UnityEngine;
using TMPro;
using System;

public class InteractZoneWithDissolve : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    [SerializeField] private GameObject objectToDissolve; // Объект, который будет растворяться
    [SerializeField] private string interactionText = "E - Interact";
    [SerializeField] private float dissolveDelay = 0.5f; // Задержка перед началом растворения

    [Header("Настройки Dissolve эффекта")]
    [SerializeField] private float dissolveDuration = 1.5f;
    [SerializeField] private Color dissolveColor = Color.blue; // Цвет растворения
    [SerializeField] private bool useCustomDissolveColor = false;

    [Header("UI элементы")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionTextUI;

    [Header("Звуки")]
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private float soundVolume = 0.5f;

    [Header("Визуальные эффекты")]
    [SerializeField] private ParticleSystem interactionParticles;
    [SerializeField] private bool useHighlight = true;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Опции")]
    [SerializeField] private bool disableZoneAfterInteraction = true;
    [SerializeField] private bool destroyObjectAfterDissolve = true;

    private bool isPlayerInZone = false;
    private bool hasInteracted = false;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private DissolveEffect dissolveEffect;
    private Collider2D objectCollider;

    // Событие для других скриптов
    public event Action OnInteracted;
    public event Action OnDissolveComplete;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (interactionTextUI != null)
        {
            interactionTextUI.text = interactionText;
        }

        if (objectToDissolve != null)
        {
            if (useHighlight)
            {
                spriteRenderer = objectToDissolve.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    originalColor = spriteRenderer.color;
                }
            }

            objectCollider = objectToDissolve.GetComponent<Collider2D>();

            dissolveEffect = objectToDissolve.GetComponent<DissolveEffect>();
            if (dissolveEffect == null)
            {
                dissolveEffect = objectToDissolve.AddComponent<DissolveEffect>();
            }

            if (dissolveEffect != null)
            {
                dissolveEffect.dissolveDuration = dissolveDuration;
                dissolveEffect.startDelay = dissolveDelay;
                dissolveEffect.destroyOnComplete = destroyObjectAfterDissolve;

                if (dissolveEffect.objectRenderer == null)
                {
                    dissolveEffect.objectRenderer = objectToDissolve.GetComponent<Renderer>();
                }
            }
        }
        else
        {
            Debug.LogError("Object To Dissolve не назначен!", this);
        }
    }

    void Update()
    {
        if (!isPlayerInZone || hasInteracted) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (useHighlight && spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, highlightColor, Mathf.PingPong(Time.time, 1f));
        }
    }

    void Interact()
    {
        if (hasInteracted) return;

        hasInteracted = true;

        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position, soundVolume);
        }

        if (interactionParticles != null)
        {
            interactionParticles.Play();
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (useHighlight && spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }

        StartCoroutine(StartDissolveEffect());

        OnInteracted?.Invoke();

        if (disableZoneAfterInteraction)
        {
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }

        Debug.Log($"Взаимодействие выполнено! Объект {objectToDissolve.name} растворяется.");
    }

    private System.Collections.IEnumerator StartDissolveEffect()
    {
        yield return new WaitForSeconds(dissolveDelay);

        if (dissolveEffect != null)
        {
            if (useCustomDissolveColor)
            {
                dissolveEffect.StartDissolve(dissolveColor);
            }
            else
            {
                dissolveEffect.StartDissolve();
            }

            yield return new WaitForSeconds(dissolveDuration + 0.1f);

            OnDissolveComplete?.Invoke();
        }
        else
        {
            Debug.LogWarning("DissolveEffect не найден, просто отключаем объект.");
            objectToDissolve.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasInteracted) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }

            Debug.Log("Игрок вошел в зону взаимодействия");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hasInteracted) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }

            if (useHighlight && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }

            Debug.Log("Игрок вышел из зоны взаимодействия");
        }
    }

    public void ForceInteract()
    {
        Interact();
    }

    public void ResetInteraction()
    {
        hasInteracted = false;
        isPlayerInZone = false;

        if (objectToDissolve != null)
        {
            objectToDissolve.SetActive(true);

            if (objectCollider != null)
            {
                objectCollider.enabled = true;
            }

            if (dissolveEffect != null)
            {
                dissolveEffect.ResetDissolve();
            }
        }

        GetComponent<Collider2D>().enabled = true;
        this.enabled = true;

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null && collider.enabled)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position + (Vector3)collider.offset, collider.size);

            if (objectToDissolve != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, objectToDissolve.transform.position);
            }
        }
    }
}