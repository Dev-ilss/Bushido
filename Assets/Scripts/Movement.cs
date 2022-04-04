using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public CharacterController controller; //instancia del script del controlador del personaje (para acceder a los metodos con los valores que tiene el personaje)
    public Animator animator; //Instancia del animator
    float horizontalMove = 0f; // Variable para saber cuando activar la animacion de correr
    public float runSpeed = 75f; // Velocidad para correr
    private bool jump = false; // Para saber si brinco

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed; // Usamos la direccion del eje x del personaje multiplicada por la velocidad
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); //Aumentamos el parametro Speed
        
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping", true); // Si brinca, cambiamos el parametro isJumping del animator
        }
    }

    //Cuando aterrice, resetear la variable y el parametro de brincar
    public void OnLanding()
    {
        jump = false;
        animator.SetBool("isJumping", false);
    }
    
    private void FixedUpdate()
    {
        //Movemos al personaje con la funcion del controlador
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        //jump = false;
    }
}
