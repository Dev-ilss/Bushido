using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator; // Instancia del animator para poder llamar otras animaciones
    public int maxHealth = 100; // Vida maxima del enemigo

    private int currentHealth; // Vida actual del enemigo
    void Start()
    {
        //Inicializar la vida actual con la maxima
        currentHealth = maxHealth;
    }
    
    //Funcion del enemigo cuando recibe un ataque
    public void TakeDamage(int damage)
    {
        //Hacemos trigger a la animacion de tomar daño
        animator.SetTrigger("Hurt");
        currentHealth -= damage; //Le restamos el daño a la vida

        // Si su vida llega a cero o menor, mandar llamar la funcion de morir
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Se murio");
        //Cambiar el parametro del animatior de isDead a true
        animator.SetBool("isDead", true);
        
        //Eliminar el collider, para que no estorbe xd
        GetComponent<Collider2D>().enabled = false;
        //Deshabilitar el objeto
        this.enabled = false;
    }
}
