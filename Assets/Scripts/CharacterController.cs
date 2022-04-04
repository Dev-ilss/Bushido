using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;							// Cantidad de fuerza añadida cuando el jugador brinca.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Cantidad de maxSpeed aplicada al moverse agachado. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// Que tan fluido sera el movimiento
	[SerializeField] private bool m_AirControl = false;							// Si el jugador puede moverse en el aire
	[SerializeField] private LayerMask m_WhatIsGround;							// Un mask para saber que es piso para el jugador
	[SerializeField] private Transform m_GroundCheck;							// La posicion para checar si el jugador esta en el suelo.
	[SerializeField] private Transform m_CeilingCheck;							// La posicion para checar si el jugador esta tocando un techo
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// Collider que se desactivara cuando estas agachado

	const float k_GroundedRadius = .2f; // Radio para saber si el jugador esta en el suelo
	private bool m_Grounded;            // si el jugador esta en el suelo.
	const float k_CeilingRadius = .2f; // Radio para saber si el jugador se puede parar
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // Para determinar a que direccion esta viendo el jugador.
	private Vector3 m_Velocity = Vector3.zero;

	
	// Eventos
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent; //Detectar si el jugador ya cayo

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent; // Evento para cuando se agache
	private bool m_wasCrouching = false; // Saber si estaba agachado anteriormente

	//Inicializamos algunos eventos y un rigidbody en el stage de awake
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		
		//El jugador esta en el suelo si el radio del suelo esta haciendo hit a algo denominado como suelo, mandamos llamar el evento OnLand
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// Si esta agachado, checar si el jugador se puede levantar
		if (crouch)
		{
			// Si el personaje tiene un objeto que no le permita pararse, entonces mantenerlo agachado
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//Solo controlar al jugador si esta en el suelo o tiene el control de aire activado
		if (m_Grounded || m_AirControl)
		{

			// Si esta agachado
			if (crouch)
			{
				//Y no estaba agachado, mandar llamar el evento de OnCrouch
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reducir la velocidad por el procentaje de velocidad de agachado
				move *= m_CrouchSpeed;

				// Desactivar un collider cuando este agachado
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Activar el collider cuando no esta agachado
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				//Mandar llamar el evento de OnCrouch para hacerle saber que ya no sigue agachado
				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			
			//Mover al personaje encontrando la velocidad del target
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// Y luego suavizarlo un poco aplicandoselo al personaje
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// Si el input es hacia la derecha, entonces que voltee al lado derecho
			if (move > 0 && !m_FacingRight)
			{
				// FLIP!
				Flip();
			}
			// De otro modo que voltee a la izquierda
			else if (move < 0 && m_FacingRight)
			{
				// FLIP!
				Flip();
			}
		}
		// Si el personaje puede brincar
		if (m_Grounded && jump)
		{
			// Añadir una fuerza vertical al jugador
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}


	private void Flip()
	{
		// Cambiar el lado hacia donde esta viendo el jugador
		m_FacingRight = !m_FacingRight;
		
		// Multiplicar el eje x de scale por -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
