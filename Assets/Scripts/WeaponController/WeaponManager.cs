using EnemyAI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace WeaponController
{
    public class WeaponManager : MonoBehaviour, IOnEventCallback
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
        
        public PhotonView photonView;

        public GameManager gameManager;

        private const byte VFX_EVENT = 0;
        
        void Start()
        {
         weaponAudioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.InRoom && !photonView.IsMine)
            {
                return;
            }
            if (!gameManager.isPaused && !gameManager.isGameOver)
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
            if (PhotonNetwork.InRoom)
            {
                int viewID = photonView.ViewID;
                
                RaiseEventOptions raiseOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                
                PhotonNetwork.RaiseEvent(VFX_EVENT, viewID, raiseOptions, sendOptions);
                //photonView.RPC("WeaponShootSFX", RpcTarget.All, photonView.ViewID);
            }
            else
            {
                ShootVFX(photonView.ViewID);
            }
            playerAnimator.SetBool("isShooting", true);
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
        
        public void ShootVFX(int viewID)
        {
            if (photonView.ViewID == viewID)
            {
                flashParticleSystem.Play();
                weaponAudioSource.PlayOneShot(shootClip, 0.75f);
            }
        }

        void IOnEventCallback.OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == VFX_EVENT)
            {
                Debug.Log("EventReceived");
                int viewID = (int)photonEvent.CustomData;
                ShootVFX(viewID);
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
    }
}
