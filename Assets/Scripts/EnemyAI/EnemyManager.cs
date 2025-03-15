using System;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    public class EnemyManager : MonoBehaviour
    {
        public GameObject player;
        public Animator enemyAnimator;
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
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
                Debug.Log("Player Hit");
            }
        }
    }
}
