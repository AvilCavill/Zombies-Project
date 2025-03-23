using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Network
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        public static RoomManager sharedInstance;

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

        private void OnEnable()
        {
            //Hem subscribim al esdeveniment
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            //Anul·lam la subscripció
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            //Anul·lam la subscripció
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-3f, -2f), 2f, Random.Range(-3f, -2f));
            if (PhotonNetwork.InRoom)
            {
                // Estam online
                PhotonNetwork.Instantiate("First_Person_Player", spawnPos, Quaternion.identity, 0);
            }
            else
            {
                //Single Player
                Instantiate(Resources.Load("First_Person_Player"), spawnPos, Quaternion.identity);
            }
        }
        
    }
}
