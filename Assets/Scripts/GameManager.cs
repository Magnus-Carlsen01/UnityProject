using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    int progressAmount;
    public Slider progressBar;

    public GameObject player;
    public GameObject LoadingUI;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int levelsSurvivedCount;

    public static event Action OnReset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressAmount = 0;
        progressBar.value = 0;
        Gem.OnCollect += IncreaseProgress;
        PlayerHealth.OnPlayerDeath += GameOverScreen;
        HoldToNextLevel.OnHoldComplete += NextLevel;
        LoadingUI.SetActive(false);
        gameOverScreen.SetActive(false);
    }
    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + levelsSurvivedCount + " LEVEL";
        if (levelsSurvivedCount != 1)
        {
            survivedText.text += "S";
        }
        Time.timeScale = 0f; // Pause the game
    }
    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        levelsSurvivedCount = 0;
        LoadLevel(0, false);
        Time.timeScale = 1f;
        OnReset.Invoke();
    }
        void IncreaseProgress(int amount)
    {
        progressAmount += amount;
        progressBar.value = progressAmount;
        if(progressAmount >= 100)
        {
            LoadingUI.SetActive(true);
            Debug.Log("You win!");
        }
    }
    void LoadLevel(int level, bool isSurvivedIncrease)
    {
        LoadingUI.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 0, 0);

        currentLevelIndex = level;
        progressAmount = 0;
        progressBar.value = 0;
        if (isSurvivedIncrease) levelsSurvivedCount++;
    }
    void NextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}
