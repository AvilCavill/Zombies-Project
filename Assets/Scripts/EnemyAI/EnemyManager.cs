using System;
using PlayerController;
using UnityEngine;
using UnityEngine.AI;

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
        
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerManager = player.GetComponent<PlayerManager>();
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
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
                playerManager.Hit(20f);
                Debug.Log("Player Hit, health" + playerManager.health);
            }
        }

        public void HitEnemy(float damage)
        {
            enemyHealth -= damage;
            if (enemyHealth <= 0)
            {
                Destroy(gameObject);
                gameManager.enemiesAlive--;
            }
        }
        
    }
}
