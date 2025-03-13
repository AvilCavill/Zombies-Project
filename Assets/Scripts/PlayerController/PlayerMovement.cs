using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   //Variables jugador
   public CharacterController player;
   public float speed = 12.0f;
   
   //Variables de la gravetat
   private Vector3 velocity;
   public float gravity = -9.81f;

   private void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
   }

   private void Update()
   {
      float x = Input.GetAxis("Horizontal");
      float z = Input.GetAxis("Vertical");
      
      Vector3 move = transform.right * x + transform.forward * z;
      player.Move(move * speed * Time.deltaTime);
      //Gravetat
      //Fórmula de velocitat = acceleració * temps^2
      velocity.y += gravity * Time.deltaTime;
      player.Move(velocity * Time.deltaTime);
   }
}
