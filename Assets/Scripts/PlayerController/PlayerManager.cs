using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerController
{
    public class PlayerManager : MonoBehaviour
    {
        public float health = 100f;

        public void Hit(float damage)
        {
            health -= damage;
        }

        private void Update()
        {
            if (health <= 0f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
