using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource sfxObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn in GameObject
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        // Assign the AudioClip
        audioSource.clip = audioClip;

        // Assign the Volume
        audioSource.volume = volume;

        // Play the sound
        audioSource.Play();

        // Get length of SFX clip
        float clipLength = audioSource.clip.length;

        // Destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
