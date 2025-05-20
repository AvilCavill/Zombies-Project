using System;
using EnemyAI;
using PlayerController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace StateMachine
{
    public class IA_StateMachine : MonoBehaviour
    {
        enum State
        {
            Idle,
            Chase,
            Attack,
            Dead,
            Fake_Dead,
            Eat_Corpse,
            Confused
        }
        
        State currentState;
        
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

        public float confusedTimer;
        public float confusedDuration = 3.0f;
        
        
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = GameObject.FindGameObjectWithTag("Player");

            healthBar.maxValue = enemyHealth;
            healthBar.value = enemyHealth;
            
            currentState = State.Idle;
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
            
            if (player != null)
            {
                GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            
                healthBar.transform.LookAt(player.transform);
            }

            switch (currentState)
            {
                case State.Idle:
                    IdleBehaviour();
                    break;
                case State.Chase:
                    ChaseBehaviour();
                    break;
                case State.Attack:
                    AttackBehaviour();
                    break;
                case State.Confused:
                    ConfusedBehaviour();
                    break;
                case State.Dead:
                    DeadBehaviour();
                    break;
                case State.Eat_Corpse:
                    EatCorpseBehaviour();
                    break;
                case State.Fake_Dead:
                    FakeDeadBehaviour();
                    break;
                
            }
            enemyAnimator.SetBool("isRunning", GetComponent<NavMeshAgent>().velocity.magnitude > 1);
        }
        
        private void IdleBehaviour()
        {
            GetComponent<NavMeshAgent>().SetDestination(transform.position); // Quieto
        }

        private void ChaseBehaviour()
        {
            GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        }

        private void AttackBehaviour()
        {
            // Ya manejado por OnCollisionStay, pero podrías animar al enemigo aquí
        }

        private void DeadBehaviour()
        {
            // Ya está en TakeDamage
        }

        private void FakeDeadBehaviour()
        {
            // Aquí puedes hacer que se haga el muerto, por ejemplo, dejar de moverse
        }

        private void EatCorpseBehaviour()
        {
            // Ir hacia el cadáver y reproducir animación de comer
        }

        private void ConfusedBehaviour()
        {
            confusedTimer -= Time.deltaTime;
            if (confusedTimer <= 0)
            {
                enemyAnimator.SetBool("isConfused", false);
                currentState = State.Idle;
            }
        }


        private void EnterConfusedState()
        {
            currentState = State.Confused;
            confusedTimer = confusedDuration;
            enemyAnimator.SetBool("isConfused", true);
        }

        private void HitHeadshot(float damage)
        {
            TakeDamage(damage);
            
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
            TakeDamage(damage);
        }

     
        public void TakeDamage(float damage)
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
                gameManager.enemiesAlive--;    
            }
        }
    }
}
