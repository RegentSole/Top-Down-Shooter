/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip footstepSound;
    public AudioClip dashSound;
    public AudioClip runSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void PlayFootstep()
    {
        if (isRunning)
        {
            audioSource.PlayOneShot(runSound);
        }
        else
        {
            audioSource.PlayOneShot(footstepSound);
        }
    }

    // Вызывайте этот метод через события анимации или по таймеру
}
*/