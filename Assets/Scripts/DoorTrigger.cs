using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Reference")]
    public AnimatedExitDoor doorController;

    [Header("Trigger Settings")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            if (doorController != null)
            {
                doorController.OnPlayerPassed();
                hasTriggered = true;
                Debug.Log("Player passed through door");
            }
        }
    }

    // Визуализация триггера в редакторе
    void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, collider.bounds.size);
        }
    }
}