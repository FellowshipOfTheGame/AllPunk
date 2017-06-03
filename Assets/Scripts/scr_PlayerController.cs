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


	//
	private Vector2 position;

	//Chamado ao carregar o script, inicializa variáveis
	void Awake()
	{
		isFacingRight = true;
		playerRigidBody2D = (Rigidbody2D)GetComponent
			(typeof(Rigidbody2D));
	}

	// Chamado uma vez por frame
	void Update () {
		// Pega input horizontal
		movePlayerVector = Input.GetAxisRaw("Horizontal");
		playerRigidBody2D.velocity = new Vector2(movePlayerVector *
			speed, playerRigidBody2D.velocity.y);
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
