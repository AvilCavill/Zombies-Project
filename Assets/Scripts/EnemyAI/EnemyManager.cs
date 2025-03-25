using System;
using Photon.Pun;
using PlayerController;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EnemyAI
{
    public class EnemyManager : MonoBehaviour
    {
        public GameObject player;
        public Animator enemyAnimator;
        private PlayerManager playerManager;
        public float enemyHealth = 100f;
        public float damage = 20f;
        public GameManager gameManager;
        
        //UI Health
        public Slider healthBar;
        
        //Animació i millora del xoc
        public bool playerInReach;
        public float attackDelayTimer;
        public float howMuchEarlierStartAttackAnimation;
        public float delayBetweenAttacks;

        // public AudioSource enemyAudioSource;
        // public AudioClip[] growlAudioClips;

        private GameObject[] playersInScene;
        
        public PhotonView photonView;
        
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            playersInScene = GameObject.FindGameObjectsWithTag("Player");
            player = GameObject.FindGameObjectWithTag("Player");

            healthBar.maxValue = enemyHealth;
            healthBar.value = enemyHealth;
            // enemyAudioSource = GetComponent<AudioSource>();/
        }

        // Update is called once per frame
        void Update()
        {
            // if (!enemyAudioSource.isPlaying)
            // {
            //     enemyAudioSource.clip = growlAudioClips[Random.Range(0, growlAudioClips.Length)];
            //     enemyAudioSource.Play();
            // }

            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            GetClosestPlayer();
            if (player != null)
            {
                GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            
                healthBar.transform.LookAt(player.transform);
            }
            
            if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
            {
                enemyAnimator.SetBool("isRunning", false);
            }
            else
            {
                enemyAnimator.SetBool("isRunning",true);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInReach = true;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (playerInReach)
            {
                attackDelayTimer += Time.deltaTime;
                if (attackDelayTimer >= delayBetweenAttacks - howMuchEarlierStartAttackAnimation &&
                    attackDelayTimer <= delayBetweenAttacks)
                {
                    enemyAnimator.SetTrigger("isAttacking");
                }

                if (attackDelayTimer >= delayBetweenAttacks)
                {
                    player.GetComponent<PlayerManager>().Hit(damage);
                    attackDelayTimer = 0;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject == player)
            {
                playerInReach = false;
                attackDelayTimer = 0;
            }
        }

        public void HitEnemy(float damage)
        {
            photonView.RPC("TakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }

        [PunRPC]
        public void TakeDamage(float damage, int viewID)
        {
            if(photonView.ViewID == viewID)
            {
                healthBar.value -= damage;
                enemyHealth -= damage;
                if (enemyHealth <= 0)
                {
                    enemyAnimator.SetTrigger("isDead");
                    Destroy(gameObject,10f);
                    Destroy(GetComponent<NavMeshAgent>());
                    Destroy(GetComponent<EnemyManager>());
                    Destroy(GetComponent<CapsuleCollider>());

                    if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient && photonView.IsMine)
                    {
                        gameManager.enemiesAlive--;    
                    }
                }
            }
        }

        private void GetClosestPlayer()
        {
            float minddistance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (GameObject p in playersInScene)
            {
                if (p != null)
                {
                    float distance = Vector3.Distance(p.transform.position, currentPosition);
                    if (distance < minddistance)
                    {
                        minddistance = distance;
                    }
                }
                
            }
        }
        
    }
}
