using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class SceneManagement : Singleton<SceneManagement>
{
    public string SceneTransitionName { get; private set; }

    [SerializeField] private GameObject defeatEnemiesText; // Reference to the DefeatEnemies GameObject
    private float messageDisplayDuration = 3f; // Duration for which the message will be displayed

    public void SetTransitionName(string sceneTransitionName)
    {
        this.SceneTransitionName = sceneTransitionName;
    }

    public void DisplayDefeatEnemiesMessage()
    {
        StartCoroutine(DisplayDefeatEnemiesMessageCoroutine());
    }

    private IEnumerator DisplayDefeatEnemiesMessageCoroutine()
    {
        if (defeatEnemiesText != null)
        {
            defeatEnemiesText.SetActive(true);

            yield return new WaitForSeconds(messageDisplayDuration);

            if (defeatEnemiesText != null)
            {
                defeatEnemiesText.SetActive(false);
            }
        }
    }
}
