using UnityEngine;

public class SceneDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== SCENE DEBUG ===");
        Debug.Log($"Camera: {Camera.main != null}");

        if (Camera.main != null)
        {
            Debug.Log($"Camera position: {Camera.main.transform.position}");
            Debug.Log($"Camera orthographic: {Camera.main.orthographic}");
            Debug.Log($"Camera size: {Camera.main.orthographicSize}");
        }

        Debug.Log($"Enemy position: {transform.position}");
        Debug.Log($"Distance to camera: {Vector3.Distance(transform.position, Camera.main.transform.position)}");
    }

    void Update()
    {
        // Медленно двигаем врага чтобы увидеть его
        transform.position += Vector3.up * Mathf.Sin(Time.time) * 0.01f;
    }
}