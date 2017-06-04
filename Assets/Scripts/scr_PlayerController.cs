using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controlador do jogador para o protótipo do ALLPUNK
 * @author João Victor L. da S. Guimarães
 */
public class scr_PlayerController : MonoBehaviour {


	// VARIÁVEIS 



	// RigidBody component instance for the player
	private Rigidbody2D playerRigidBody2D;

	//Variable to track how much movement is needed from input
	private float movePlayerVector;

	// For determining which way the player is currently facing.
	private bool isFacingRight;

	// Speed modifier for player movement
	public float speed = 4.0f;

	// Speed modifier for player movement
	public float jumpForce = 4.0f;

	//booleano se determina se o jogador 
	private bool isGrounded;

	//Transform que armazena a posição dos pés
	private Transform playerFeetPosition;

	//Chamado ao carregar o script, inicializa variáveis
	void Awake()
	{
		isFacingRight = true;
		playerRigidBody2D = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;
	}

	/**
	 * Chamado uma vez por frame, usada para gerenciar Rigibody
	 */
	void FixedUpdate(){
		// Pega input horizontal
		movePlayerVector = Input.GetAxisRaw("Horizontal");

		//Verifica contato com o chão
		isGrounded = touchesGround (playerFeetPosition.position);

		/** 
		  * Usar playerRigidBody2D.velocity para que o movimento do personagem
		  * sofra a influência da física. Um transform direto poderia quebrar
		  * A física em parte
		  */
		playerRigidBody2D.velocity = new Vector2 (movePlayerVector * speed, playerRigidBody2D.velocity.y);
		/*if(isGrounded)
			playerRigidBody2D.velocity = new Vector2(movePlayerVector * speed, playerRigidBody2D.velocity.y);
		else
			playerRigidBody2D.velocity = new Vector2(movePlayerVector * speed, playerRigidBody2D.velocity.y);*/


		//TODO configurar tecla
		if (Input.GetKey (KeyCode.Space) && isGrounded )
			playerRigidBody2D.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);

		//Indo para a direita e virado para a esquerda
		if (movePlayerVector > 0 && !isFacingRight)
			Flip();
		//Indo para a esquerda e virado para a direita
		else if (movePlayerVector < 0 && isFacingRight)
			Flip();
	}

	/**
	 * Método que faz a verificação se o jogador está fazendo contato com o chão.
	 * @return true		Está tocando o chão
	 */
	bool touchesGround(Vector2 pos){
		/*Array de todos os colliders que colidem com os pés do jogador.
		 * recebe de argumento um Vector2, raio do círculo*/
		bool isGrounded = true;
		Collider2D [] array = Physics2D.OverlapCircleAll (pos, 0.2f);
		foreach (Collider2D obj in array) {
			//Verificação manual da layer
			if (obj.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
				isGrounded = true;
				break;
			} else {
				isGrounded = false;
			}	
		}
		return isGrounded;
	}


	// Chamado uma vez por frame
	void Update () {
	}
		
	/***
	 * Método que troca o sentido do sprite.
	 */
	void Flip()
	{
		//muda a direção que o jogador está encarando
		isFacingRight = !isFacingRight;
		//Multiplica a escala por -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
