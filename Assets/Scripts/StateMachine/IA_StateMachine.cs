using System;
using System.Collections;
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
        
        private bool isDead = false;
        [Range(0f, 1f)]public float fakeDeathChance = 0.3f;
        private NavMeshAgent agent;
        
        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
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
            if (isDead) return;
            // if (!enemyAudioSource.isPlaying)
            // {
            //     enemyAudioSource.clip = growlAudioClips[Random.Range(0, growlAudioClips.Length)];
            //     enemyAudioSource.Play();
            // }
            
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (player != null && agent.enabled && agent.isOnNavMesh && currentState != State.Fake_Dead && currentState != State.Dead)
            {
                agent.SetDestination(player.transform.position);
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
            GetComponent<NavMeshAgent>().SetDestination(transform.position); //Se queda quieto
            
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < 15f)  
            {
                currentState = State.Chase;
            }
        }

        private void ChaseBehaviour()
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
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

        public void HitHeadshot(float damage)
        {
            TakeDamage(damage);
            EnterConfusedState();
            
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

        
        private IEnumerator PerformFakeDeath()
        {
            isDead = false;
            enemyAnimator.SetTrigger("isDead");
            currentState = State.Dead;

            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;

            yield return new WaitForSeconds(1.5f); // tiempo de "muerto"

            currentState = State.Fake_Dead;
            enemyAnimator.SetTrigger("fakeDeath"); // animación de levantarse

            // Esperar a que la animación termine
            yield return new WaitUntil(() =>
                enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("zombie_fakeDeath") &&
                enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            );

            // Revivir completamente
            enemyHealth = 40f;
            healthBar.maxValue = enemyHealth;
            healthBar.value = enemyHealth;

            GetComponent<CapsuleCollider>().enabled = true;
            agent.enabled = true;

            currentState = State.Chase;
        }

     
        public void TakeDamage(float damage)
        {
            healthBar.value -= damage;
            enemyHealth -= damage;
            if (enemyHealth <= 0)
            {
                if (UnityEngine.Random.value <= fakeDeathChance)
                {
                    StartCoroutine(PerformFakeDeath());
                    return;
                }
            isDead = true;
            enemyAnimator.SetTrigger("isDead");
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            gameManager.enemiesAlive--;
            Destroy(gameObject, 10f);
        }   
        }
    }
}
