using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace PlayerController
{
    public class PlayerManager : MonoBehaviour
    {
        public float health = 100f;
        public TMP_Text healthText;
        
        //Shake camera variables
        public GameObject playerCamera;
        private float shakeTime = 1f;
        public float shakeDuration = 0.5f;
        private Quaternion playerCameraOriginalRotation;
        
        //HitPanel
        public CanvasGroup hitPanel;

        public void Hit(float damage)
        {
            shakeTime = 0;
            health -= damage;
            hitPanel.alpha = 1;
        }

        public void CameraShake()
        {
            playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2f, 2f), 0f, 0f);
        }
        
        private void Update()
        {
            if (hitPanel.alpha > 0)
            {
                hitPanel.alpha -= Time.deltaTime;
            }
            healthText.text = health.ToString();
            if (shakeTime < shakeDuration)
            {
                shakeTime += Time.deltaTime;
                CameraShake();
            } else if (playerCamera.transform.localRotation != playerCameraOriginalRotation)
            {
                playerCamera.transform.localRotation = playerCameraOriginalRotation;
            }
        }
    }
}
