using System.Collections;

using System.Collections.Generic;

using UnityEngine;



/**

 * Controlador do jogador para o protótipo do ALLPUNK

 * @author João Victor L. da S. Guimarães

 */

public class scr_PlayerController : MonoBehaviour {


	#region variables

	//Tempo maximo para salto alto
	public float maxHighJumpTime = 0.25f;

	//Velocidade da caminhada do personagem

	public float speed = 4.0f;

	//Velocidade do salto do personagem

	public float jumpSpeed = 6.0f;

	//Multiplicador da gravidade p/ queda, deixando-a mais rápida

	public float fallMultiplier = 1.5f;

	//Multiplicador da gravidade p/ saltos curtos

	public float lowJumpMultiplier = 1f;

    //Define se o personagem tem ou vai usar os IKs para movimentar os braços

    public bool useArmIK = true;

    //Distancia considerada até inverter o personagem

    public float armOffset;

	//booleano se determina se o jogador esta no chão

	private bool isGrounded;

	//booleano se determina se o jogador esta pulando

	private bool isJumping;

	//Transform que armazena a posição dos pés

	private Transform playerFeetPosition;

    //Transform do IK do braço direito

    private Transform rightArmIK;

    //Transform do IK do braço esquerdo

    private Transform leftArmIK;

	//Variable to track how much movement is needed from input

	private float movePlayerVector;

	// For determining which way the player is currently facing.

	private bool isFacingRight;

	//Referencia ao rigidBody

	private Rigidbody2D rb;

	//Tempo atual de salto alto
	private float currHighJumpTime;

    //Referência para o gerenciador de PA's
    private scr_PA_Manager paManager;

    //Referência para o animator
    private Animator animator;

    #endregion variables





	#region MonoBehaviour methods




    //Chamado ao carregar o script, inicializa variáveis

    protected void Awake()

	{
		currHighJumpTime = maxHighJumpTime; //1s

		isFacingRight = true;

		rb = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));

		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;

        if (useArmIK)

        {

            rightArmIK = this.transform.Find("IK").Find("IK_R_Hand").GetComponent<Transform>();

            leftArmIK = this.transform.Find("IK").Find("IK_L_Hand").GetComponent<Transform>();

        }

        paManager = GetComponent<scr_PA_Manager>();

        animator = GetComponent<Animator>();

    }



	/**

	 * Chamado uma vez por frame, usada para gerenciar Rigibody

	 */

	void FixedUpdate(){

    }



    private void Update()

    {

		//Verifica contato com o chão

		isGrounded = touchesGround (playerFeetPosition.position);


		playerHorizontalMove ();


		/*if (Input.GetButtonDown ("Jump") && isGrounded)
			Jump ();

		if (rb.velocity.y < 0 && !isGrounded)
			Fall ();

		if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !isGrounded)
			lowJump();*/


		if (Input.GetButtonDown ("Jump") && isGrounded)
			Jump ();

		if (rb.velocity.y > 0 && Input.GetButton("Jump") && !isGrounded)
			highJump();



		if (!useArmIK)
		{

			//Indo para a direita e virado para a esquerda

			if (movePlayerVector > 0 && !isFacingRight)

				Flip();

			//Indo para a esquerda e virado para a direita

			else if (movePlayerVector < 0 && isFacingRight)

				Flip();

		}



		if (useArmIK)

		{

			//Change arm orientation

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			mouseWorldPosition.z = transform.position.z;



			rightArmIK.SetPositionAndRotation(mouseWorldPosition, rightArmIK.rotation);

			leftArmIK.SetPositionAndRotation(mouseWorldPosition,leftArmIK.rotation);



			Vector3 relativeMouse = mouseWorldPosition - transform.position;

			if (relativeMouse.x + armOffset < 0 && isFacingRight)

				Flip();

			if (relativeMouse.x - armOffset > 0 && !isFacingRight)

				Flip();

		}

        if (animator != null) {
            animator.SetFloat("HorSpeed", Mathf.Abs(rb.velocity.x));
            animator.SetFloat("VerSpeed", rb.velocity.y);
            animator.SetBool("IsGrounded", isGrounded);
        }
	}



	#endregion MonoBehaviour methods



	#region Control methods





	void playerHorizontalMove(){

		/** 

		  * Usar rb.velocity para que o movimento do personagem
		  * sofra a influência da física. Um transform direto poderia quebrar
		  * A física em parte
		  */

		// Pega input horizontal

		movePlayerVector = Input.GetAxisRaw("Horizontal");

		rb.velocity = new Vector2 (movePlayerVector * speed, rb.velocity.y);

	}



    /**
	 * Método que faz a verificação se o jogador está fazendo contato com o chão.
	 * @return true		Está tocando o chão
	 */

    bool touchesGround(Vector2 pos){

		/*Array de todos os colliders que colidem com os pés do jogador.

		 * recebe de argumento um Vector2, raio do círculo*/

		bool isGrounded = true;

		Collider2D [] array = Physics2D.OverlapCircleAll (pos, 0.3f);

		foreach (Collider2D obj in array) {

			//Verificação manual da layer

			if (obj.gameObject.layer == LayerMask.NameToLayer ("Ground")) {

				isGrounded = true;
				currHighJumpTime = maxHighJumpTime;
				break;

			} else {

				isGrounded = false;

			}	

		}

		return isGrounded;

	}





	//Método para Salto, adiciona velocidade no eixo Y

	void Jump (){
		//print ("jump");
		rb.velocity += new Vector2  (0, jumpSpeed);

	}


	/**
	//Método para Salto curto, ocorre quando o botão de salto é solto logo após saltar

	void lowJump (){
		print ("LJ");
		rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier) * Time.deltaTime;

	}



	* Método responsável por fazer com que a queda do jogador seja mais rápida
	* Isso deixa um controle de salto mais aprimorado e próprio para platformer


	void Fall (){
		print ("fall");
		rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.deltaTime;
	}*/



	/**
	 * Mantém a velocidade em Y constante enquanto o botão estiver pressionado
	 */
	void highJump(){
		//print ("highJump");
		if (currHighJumpTime > 0) {
			rb.velocity = new Vector2  (rb.velocity.x, jumpSpeed);
			currHighJumpTime-= Time.deltaTime;
		}
	}


	#endregion Control methods

	/***
	 * Método que troca o sentido do sprite.
	 */

	void Flip()

	{

		//muda a direção que o jogador está encarando

		isFacingRight = !isFacingRight;

		if (!useArmIK)

		{

			//Multiplica a escala por -1

			Vector3 theScale = transform.localScale;

			theScale.x *= -1;

			transform.localScale = theScale;

		}



		//Inverte a orientação do esqueleto

		if (useArmIK)

		{

			Transform skeletonTransform = transform.Find("Bones");

			Vector3 theScale = skeletonTransform.localScale;

			Vector3 ikScale = leftArmIK.localScale;

			theScale.x *= -1;

			ikScale.x *= -1;

			skeletonTransform.localScale = theScale;

			leftArmIK.localScale = ikScale;

			rightArmIK.localScale = ikScale;

			SpriteRenderer sprite = GetComponent<SpriteRenderer>();

			if (sprite != null) {

				bool Flip = !sprite.flipX;

				sprite.flipX = Flip;

			}

		}

        if (paManager != null)
            paManager.Flip();


	}

}

