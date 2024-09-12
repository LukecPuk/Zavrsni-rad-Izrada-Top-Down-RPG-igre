using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    [SerializeField] private Transform enemiesParent; // Reference to the parent GameObject of all enemies

    private float waitToLoadTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            if (AreAllEnemiesDefeated())
            {
                SceneManagement.Instance.SetTransitionName(sceneTransitionName);
                UIFade.Instance.FadeToBlack();
                StartCoroutine(LoadSceneRoutine());
            }
            else
            {
                // Pozovi metodu da pokaže da nisu ubijeni svi neprijatelji
                SceneManagement.Instance.DisplayDefeatEnemiesMessage();
            }
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        while (waitToLoadTime >= 0)
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private bool AreAllEnemiesDefeated()
    {
        // Check if the enemiesParent has been assigned
        if (enemiesParent == null)
        {
            Debug.LogWarning("Enemies Parent not assigned! Skipping enemy check.");
            return true; // Allow exit if no enemiesParent is assigned
        }

        // Check if there are any active children in the enemiesParent
        foreach (Transform enemy in enemiesParent)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                return false; // An enemy is still alive
            }
        }

        return true; // All enemies are defeated
    }
}
