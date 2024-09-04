using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text currentScoreText; // Reference to the current score Text on the game over screen

    private TMP_Text scoreText;
    const string FINAL_SCORE = "FinalScoreNumber";
    private int currentScore = 0;

    private void Start()
    {
        // Get the current score from EconomyManager
        currentScore = EconomyManager.Instance.GetCurrentScore();
        scoreText = GameObject.Find(FINAL_SCORE).GetComponent<TMP_Text>();
        scoreText.text = currentScore.ToString("D3");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
