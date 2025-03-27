using System;
using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using Photon.Pun;
using Photon.Realtime;
using PlayerController;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int enemiesAlive;
    public int round;
    
    //UI
    public TMP_Text roundText;
    public TMP_Text totalRoundsText;

    public GameObject[] spawnPoints;
    
    //Panells
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject UIGamePanel;
    public GameObject hitPanel;

    public PlayerManager playerManager;

    public bool isPaused;
    public bool isGameOver;

    public PhotonView photonView;

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
        if (!PhotonNetwork.InRoom || (PhotonNetwork.IsMasterClient && photonView.IsMine))
        {
            ;
            if (enemiesAlive == 0)
            {
                Debug.Log("Canvi de ronda" + PhotonNetwork.InRoom);
                round++;
                totalRoundsText.text = "Rounds survived: " + round.ToString();
                NextWave(round);
                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("currentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRound(round);
                }
            }
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

    private void DisplayNextRound(int i)
    {
        roundText.text = round.ToString();
    }

    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            int randPos = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randPos];
            GameObject enemyInstance;
            
            if (PhotonNetwork.InRoom)
            {
                enemyInstance = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemyInstance = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
            }
            
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void RestartGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            // Tornam a temps a la normalitat si no estam online
            Time.timeScale = 1;
            SceneManager.LoadScene(3);
        }
        else
        {
            PhotonNetwork.LoadLevel(2);
        }
    }

    public void BackToMainMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void PauseGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        pausePanel.SetActive(true);
        UIGamePanel.SetActive(false);
        hitPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void Resume()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        UIGamePanel.SetActive(true);
        hitPanel.SetActive(true);
        isPaused = false;
    }

    public void GameOver()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverPanel.SetActive(true);
        UIGamePanel.SetActive(false);
        hitPanel.SetActive(false);
        isGameOver = true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (photonView.IsMine)
        {
            if (changedProps["currentRound"] != null)
            {
                DisplayNextRound((int) changedProps["currentRound"]);
            }
        }
    }
    

}
