using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   //Variables jugador
   public CharacterController player;
   public float speed;
   public float walkSpeed = 5f;
   public float runSpeed = 10f;
   
   //Variables de la gravetat
   private Vector3 velocity;
   public float gravity = -9.81f;
   
   //Ground check
   public bool isGrounded;
   public Transform groundCheck;
   public float groundDistance = 0.4f;
   public LayerMask groundMask;

   public float jumpHeight = 2f;
   
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
      
      //Comprovar si esta tocant el terra
      isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
      if (isGrounded && velocity.y < 0)
      {
         velocity.y = -2f;
      }
      
      //print(velocity.y);
      if (Input.GetButtonDown("Jump") && isGrounded)
      {
         velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
      }

      if (Input.GetButton("Fire3") && isGrounded)
      {
         speed = runSpeed;
      }
      else
      {
         speed = walkSpeed;
      }
   }
}
