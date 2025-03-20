using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float lookSpeed = 3;
    public float mouseSensibility = 100f;
    
    private float xRotation;
    
    private void Update()
    {
        Look();
    }
    
    public void Look() 
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensibility * lookSpeed* Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensibility * lookSpeed* Time.deltaTime;

        // diferència entre xRotation i mouseY
        xRotation -= mouseY;
        // filtram per a que no passi del màxim i del mínim
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotació eix horitzontal
        playerBody.Rotate(Vector3.up * mouseX);
        //Rotació eix vertial
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

    }

    
}
