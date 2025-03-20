using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam;
    public float range;
    public float weaponDamage = 25;
    
    //Animation Controller
    public Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

    public void Shoot()
    {
        playerAnimator.SetBool("isShooting", true);
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            Debug.Log("Fire");
            if (enemyManager != null)
            {
                Debug.Log("Enemic alcanzat");
                enemyManager.HitEnemy(weaponDamage);
            }
        }
        
    }
}
