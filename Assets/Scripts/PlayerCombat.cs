using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator; //Animator del personaje

    public Transform attackPoint; //Posicion del objeto que nos ayudara a saber si estamos atacando

    public float attackRange = 0.5f; //Rango del ataque (en que posicion con respecto al personaje se encuentra)

    public int attakDamage = 40; //Daño de ataque
    public LayerMask enemyLayers; // Las layers que nos ayudaran a identificar a los enemigos

    public float attackRate = 2f; //Cuantos ataques podemos hacer 

    private float nextAttackTime = 0f; //Tiempo para determinar si podemos volver a atacar
    void Update()
    {
        // Esto esta bien fumado, pero es basicamente si el tiempo es mayor a lo que es nuestro tiempo de ataque
        if (Time.time >= nextAttackTime)
        {
            // Y si dentro de ese tiempo estamos presionando la tecla para atacar, entonces realiza el ataque y añadimos mas tiempo
            if (Input.GetKeyDown(KeyCode.K))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }       
        }
        
    }

    //Funcion para atacar, manda llamar a la animacion y colecciona la cantidad de enemigos dentro del rango de ataque para poder mandar llamar su funcion de TakeDamage
    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attakDamage);
        }
    }

    // Esto es solo para visualizar el rango de ataque con respecto al objeto (atack point)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position,attackRange);
    }
}
