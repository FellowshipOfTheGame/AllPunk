using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controlador do jogador para o protótipo do ALLPUNK
 * @author João Victor L. da S. Guimarães
 */
public class scr_PlayerController : MonoBehaviour {

	// RigidBody component instance for the player
	private Rigidbody2D playerRigidBody2D;

	//Variable to track how much movement is needed from input
	private float movePlayerVector;

	// For determining which way the player is currently facing.
	private bool isFacingRight;

	// Speed modifier for player movement
	public float speed = 4.0f;

	// Speed modifier for player movement
	public float jumpSpeed = 4.0f;

	//booleano se determina se o jogador 
	private bool isGrounded;

	//Vector que armazena a posição do jogador
	private Vector2 playerPosition;

	//Transform que armazena a posição dos pés
	private Transform playerFeetPosition;

	//Chamado ao carregar o script, inicializa variáveis
	void Awake()
	{
		isFacingRight = true;
		playerRigidBody2D = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;
		//getChil
	}

	/**
	 * Chamado uma vez por frame, usada para gerenciar Rigibody
	 */
	void FixedUpdate(){
		// Pega input horizontal
		movePlayerVector = Input.GetAxisRaw("Horizontal");

		//Verifica contat com o chão
		//TODO configurar tecla
		if (Input.GetKeyDown (KeyCode.Space) && isGrounded ){
			playerRigidBody2D.AddForce(new Vector2(0,4), ForceMode2D.Impulse);
			//rigidbody.velocity.y = jumpSpeed;
		}

		//
		/*Physics2D.OverlapCircle (playerFeetPosition.position, 
			0.5f);
		print(isGrounded);*/
		//LayerMask.NameToLayer("Ground")
		Collider2D [] array = Physics2D.OverlapCircleAll (playerFeetPosition.position, 0.25f);
		foreach (Collider2D obj in array) {
			if (obj.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
				isGrounded = true;
				print (isGrounded);
				break;
			} else {
				isGrounded = false;
			}
		}
		/**
		  * Usar playerRigidBody2D.velocity para que o movimento do personagem
		  * sofra a influência da física. Um transform direto poderia quebrar
		  * A física em parte
		  */
		playerRigidBody2D.velocity = new Vector2(movePlayerVector * speed, playerRigidBody2D.velocity.y);


		//Indo para a direita e virado para a esquerda
		if (movePlayerVector > 0 && !isFacingRight)
		{
			Flip();
		}
		//Indo para a esquerda e virado para a direita
		else if (movePlayerVector < 0 && isFacingRight)
		{
			Flip();
		}

	}


	// Chamado uma vez por frame
	void Update () {
		playerPosition = transform.position;
		//print (playerPosition);
		//print(isGrounded);
	}
		
	/***
	 * Método que troca o sentido do sprite.
	 */
	void Flip()
	{
		// Switch the way the player is labeled as facing.
		isFacingRight = !isFacingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
