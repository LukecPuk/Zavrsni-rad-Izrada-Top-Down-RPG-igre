using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import for Scene Management

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float musicVolume = 0.25f;

    public AudioClip bgmMusic;      // Default background music
    public AudioClip bossMusic;   // Music for Scene_5
    public AudioClip gameOverMusic; // Music for Game Over scene

    private void Start()
    {
        // Set initial music settings
        musicSource.clip = bgmMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();

        // Subscribe to the sceneLoaded event to detect when the scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Callback function to handle scene change
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene_5")
        {
            ChangeMusic(bossMusic);
        }
        else if (scene.name == "Game Over")
        {
            ChangeMusic(gameOverMusic);
        }
        else
        {
            ChangeMusic(bgmMusic);
        }
    }

    private void ChangeMusic(AudioClip newMusic)
    {
        if (musicSource.clip != newMusic)
        {
            musicSource.Stop();
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
