using EnemyAI;
using UnityEngine;

namespace WeaponController
{
    public class WeaponManager : MonoBehaviour
    {
        public GameObject playerCam;
        public float range;
        public float weaponDamage = 25;
    
        //Animation Controller
        public Animator playerAnimator;
        
        //Particules
        public ParticleSystem flashParticleSystem;
        public GameObject bloodParticleSystem;

        //Efectes so
        public AudioClip shootClip;
        public AudioSource weaponAudioSource;
        
        void Start()
        {
         weaponAudioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.sharedInstance.isPaused && !GameManager.sharedInstance.isGameOver)
            {
                if (playerAnimator.GetBool("isShooting"))
                {
                    playerAnimator.SetBool("isShooting", false);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            }
        }

        public void Shoot()
        {
            flashParticleSystem.Play();
            playerAnimator.SetBool("isShooting", true);
            weaponAudioSource.PlayOneShot(shootClip, 0.75f);
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
            {
                EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
                Debug.Log("Fire");
                if (enemyManager != null)
                {
                    GameObject particleInstance = Instantiate(bloodParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                    particleInstance.transform.parent = hit.transform;
                    Debug.Log("Enemic alcanzat");
                    enemyManager.HitEnemy(weaponDamage);
                }
            }
        
        }
    }
}
