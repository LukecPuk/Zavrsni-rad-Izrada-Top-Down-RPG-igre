using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Start loading the scene asynchronously
        SceneManager.LoadSceneAsync(1);

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Callback function for scene loaded event
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is the game scene
        if (scene.buildIndex == 1) // Assuming scene index 1 is the game scene
        {
            // Find the Player GameObject and set its position
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector2(0, 0);
            }

            // Unsubscribe from the sceneLoaded event
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
