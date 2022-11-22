using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public UnityEvent GamePaused;
    public UnityEvent GameResumed;

    public ScoreDisplay scoreDisplay;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI HighWaveScoreText;
    public TextMeshProUGUI HighScoreText;

   
    public void Setup()
    {



        Time.timeScale = 0;
        GamePaused.Invoke();
            gameObject.SetActive(true);
            UpdateScore();
            CheckHighScore();
            UpdateHighScore();
        

    }
   


    public void CheckHighScore()
    {
        if (scoreDisplay.score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", scoreDisplay.score);
        }
    }

    public void UpdateHighScore()
    {
        HighScoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore", 0)}";
    }

    public void UpdateScore()
    {
        scoreText.text = "Score: " + scoreDisplay.score;
    }

    public void CheckHighWaveScore()
    {
        if (scoreDisplay.score > PlayerPrefs.GetInt("HighWave", 1))
        {
            PlayerPrefs.SetInt("HighWave", WaveSpawner.instance.currWave);
        }
    }

    public void UpdateWaveHighScore()
    {
        HighWaveScoreText.text = $"Highest wave: {PlayerPrefs.GetInt("HighWave", 1)}";
    }


    public void RestartButton()
    {

        SceneManager.LoadScene(1);
        GameResumed.Invoke();
        Time.timeScale = 1;
    }

    public void ExitButton()
    {
        SceneManager.LoadScene(0);
        GameResumed.Invoke();
        Time.timeScale = 1;
    }
}
