using System;
using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using PlayerController;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive;
    public int round;
    
    //UI
    public TMP_Text roundText;
    public TMP_Text totalRoundsText;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    
    //Panells
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject UIGamePanel;
    public GameObject hitPanel;

    public PlayerManager playerManager;

    public bool isPaused;
    public bool isGameOver;

    public static GameManager sharedInstance;

    public void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isPaused = false;
        isGameOver = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        roundText.text = round.ToString();
        
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
        
    }

    private void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            roundText.text = round.ToString();
            totalRoundsText.text = "Rounds survived: " + round.ToString();
            NextWave(round);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();          
        }

        if (playerManager.health <= 0)
        {
            GameOver();
        }
    }

    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            int randPos = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randPos];
            
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;



        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        UIGamePanel.SetActive(false);
        hitPanel.SetActive(false);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        UIGamePanel.SetActive(true);
        hitPanel.SetActive(true);
        isPaused = false;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        gameOverPanel.SetActive(true);
        UIGamePanel.SetActive(false);
        hitPanel.SetActive(false);
        isGameOver = true;
    }
    

}
