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
        [SerializeField] float confusedDuration = 3f;
        [SerializeField] float confusedRadius = 5f;
        private Vector3 confusedDestination;
        
        private bool isDead = false;
        [Range(0f, 1f)]public float fakeDeathChance = 0.3f;
        private NavMeshAgent agent;
        
        //Eat corpse variables
        public float minHealthEat = 40f;
        public float healthRecoverEat = 30f;
        public float eatDuration = 5f;
        private GameObject targetCorpse;
        private bool isEating;
         
        
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
            if (player != null && agent.enabled && agent.isOnNavMesh 
                && currentState != State.Fake_Dead 
                && currentState != State.Dead
                && currentState != State.Confused)
            {
                agent.SetDestination(player.transform.position);
                healthBar.transform.LookAt(player.transform);
            }
            
            if (enemyHealth <= minHealthEat &&
                currentState != State.Eat_Corpse &&
                currentState != State.Dead &&
                currentState != State.Fake_Dead &&
                !isEating)
            {
                FindNearestCorpse();
                if (targetCorpse != null)
                {
                    currentState = State.Eat_Corpse;
                }
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
            enemyAnimator.SetBool("isRunning", GetComponent<NavMeshAgent>().velocity.magnitude > 1);
            float distance = Vector3.Distance(transform.position, player.transform.position);
            GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        }

        private void AttackBehaviour()
        {
            
        }

        private void DeadBehaviour()
        {
            
        }

        private void FakeDeadBehaviour()
        {
            // Resurrecio zombie
            enemyHealth = 40f;
            healthBar.value = enemyHealth;
            

            GetComponent<CapsuleCollider>().enabled = true;
            agent.enabled = true;

            enemyAnimator.SetBool("hasRevived", true);
            isDead = false;
            currentState = State.Chase;
        }

        
        private void EatCorpseBehaviour()
        {
            if (targetCorpse == null)
            {
                currentState = State.Idle;
                return;
            }

            float distanceToCorpse = Vector3.Distance(transform.position, targetCorpse.transform.position);

            if (distanceToCorpse > 0.5f && !isEating)
            {
                agent.SetDestination(targetCorpse.transform.position);
                enemyAnimator.SetBool("isRunning", true);
            }
            else if (!isEating)
            {
                StartCoroutine(EatCorpseCoroutine());
            }
        }

        private void ConfusedBehaviour()
        {
            confusedTimer -= Time.deltaTime;
            if (confusedTimer <= 0f)
            {
                currentState = State.Chase;
                enemyAnimator.SetBool("isConfused", false);
            }
        }


        private void EnterConfusedState()
        {

            currentState = State.Confused;
            confusedTimer = confusedDuration;
            enemyAnimator.SetBool("isRunning", true);

            // Genera un punto aleatorio cercano en la NavMesh
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * confusedRadius;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, confusedRadius, NavMesh.AllAreas) 
                && enemyHealth > 0 )
            {
                confusedDestination = hit.position;
                agent.SetDestination(confusedDestination);
            }
        }

        private void FindNearestCorpse()
        {
            GameObject [] corpses = GameObject.FindGameObjectsWithTag("Corpse");
            float minDistance = Mathf.Infinity;
            GameObject nearestCorpse = null;

            foreach (var corpse in corpses)
            {
                float distance = Vector3.Distance(transform.position, corpse.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCorpse = corpse;
                }
            }
            targetCorpse = nearestCorpse;
        }

        private IEnumerator EatCorpseCoroutine()
        {
            isEating = true;
            agent.isStopped = true;
            
            enemyAnimator.SetBool("isRunning", false);
            enemyAnimator.SetBool("isEatingCorpse", true);
            
            
            yield return new WaitForSeconds(eatDuration);

            enemyAnimator.SetBool("isEatingCorpse", false);
            
            enemyHealth += healthRecoverEat;
            enemyHealth = Mathf.Min(enemyHealth, 100);
            healthBar.value = enemyHealth;
            
            
            
                agent.isStopped = false;
                isEating = false;
            
            currentState = State.Idle;
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

            enemyAnimator.SetTrigger("fakeDeath");
            
            yield return new WaitForSeconds(2.5f); // tiempo "muerto"

            currentState = State.Fake_Dead;
                        
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
