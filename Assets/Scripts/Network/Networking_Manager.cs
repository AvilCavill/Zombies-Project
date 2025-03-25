using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class Networking_Manager : MonoBehaviourPunCallbacks
    {
        public Button multiplayerButton;
        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                StartCoroutine(DisconnectPlayer());
            }
            Debug.Log("Connected to server");
            PhotonNetwork.ConnectUsingSettings();
        }

        IEnumerator DisconnectPlayer()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                yield return null;
            }
        }
        

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Join lobby");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Apunt per jugar multiplayer");
            multiplayerButton.interactable = true;
        }

        public void FindMatch()
        {
            Debug.Log("Cercant sala");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            MakeRoom();
        }

        private void MakeRoom()
        {
            int randomRoomName = Random.Range(0, 5000);
            RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = 6,
                PublishUserId = true,
            };
            PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOptions);
            Debug.Log($"Room Created{randomRoomName}");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Carregar Escena del joc MP");
            PhotonNetwork.LoadLevel(2);
        }

        public void LoadMainMenu()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
    }
}
