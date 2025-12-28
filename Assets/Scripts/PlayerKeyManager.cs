using UnityEngine;
using TMPro;

public class PlayerKeyManager : MonoBehaviour
{
    [Header("Key Settings")]
    public int keysCollected = 0;
    public int maxKeys = 3;

    [Header("UI References")]
    public TextMeshProUGUI keysText;
    public GameObject keyIconPrefab;
    public Transform keyIconsParent;

    [Header("Audio")]
    public AudioClip keyPickupSound;

    private GameObject[] keyIcons;

    void Start()
    {
        InitializeKeyUI();
        UpdateKeyUI();
    }

    void InitializeKeyUI()
    {
        if (keyIconsParent != null && keyIconPrefab != null)
        {
            keyIcons = new GameObject[maxKeys];
            for (int i = 0; i < maxKeys; i++)
            {
                keyIcons[i] = Instantiate(keyIconPrefab, keyIconsParent);
                keyIcons[i].SetActive(false);
            }
        }
    }

    public void AddKey()
    {
        if (keysCollected < maxKeys)
        {
            keysCollected++;

            if (keyPickupSound != null)
            {
                AudioSource.PlayClipAtPoint(keyPickupSound, transform.position);
            }

            UpdateKeyUI();
            Debug.Log($"Key collected! Total keys: {keysCollected}/{maxKeys}");
        }
    }

    void UpdateKeyUI()
    {
        if (keysText != null)
        {
            keysText.text = $"Keys: {keysCollected}/{maxKeys}";
        }

        if (keyIcons != null)
        {
            for (int i = 0; i < keyIcons.Length; i++)
            {
                if (keyIcons[i] != null)
                {
                    keyIcons[i].SetActive(i < keysCollected);
                }
            }
        }
    }

    public bool HasAllKeys()
    {
        return keysCollected >= maxKeys;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddKey();
        }
    }
}