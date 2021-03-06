﻿using System.Collections;

using System.Collections.Generic;

using UnityEngine;



/**

 * Controlador do jogador para o protótipo do ALLPUNK

 * @author João Victor L. da S. Guimarães

 */

public class scr_PlayerController : MonoBehaviour {


    #region variables

	//Velocidade da caminhada do personagem
	public float speed = 4.0f;

    //Velocidade da caminhada do personagem de costas

    public float backwardSpeed = 2.0f;

    //Velocidade do salto do personagem

    public float jumpSpeed = 6.0f;

	//Multiplicador da gravidade p/ queda, deixando-a mais rápida

	public float fallMultiplier = 1.5f;

	//Multiplicador da gravidade p/ saltos curtos

	public float lowJumpMultiplier = 1f;

    //Distancia considerada até inverter o personagem

    public float armOffset;

    //Tempo que o jogador fica sem poder se mexer depois de receber um ataque;

    [Range(0,2)]
    public float knockbackTime = 0.5f;

    //Facilidade de movimento no ar
    [Range(0,1)]
    public float airControl = 1f;

	// Raio para detecção de chão

	public float checkGroundRadius = 0.01f;

	// Coyote time: Tempo que o usuário ainda está "no chão" após sair de plataforma
	public float coyoteTime = 0.1f;

	private float coyoteCounter = 0;

	//booleano se determina se o jogador esta no chão

	public bool isGrounded
	{
		get {return coyoteCounter > 0;}
		set {coyoteCounter = (value) ? coyoteTime : 0;}
	}

	//booleano se determina se o jogador esta pulando

	private bool isJumping;

	//Transform que armazena a posição dos pés

	private Transform playerFeetPosition;


	//Variable to track how much movement is needed from input

	private float movePlayerVector;

	// For determining which way the player is currently facing.

	private bool isFacingRight;

	//Referencia ao rigidBody

	private Rigidbody2D rb;

	public bool IsJumping
	{
		get {return isJumping;}
	}

    //Verifica se o jogador está recebendo ataque
    private bool underKnockback = false;

    //Referência para o gerenciador de PA's
    private scr_EPManager epManager;

    //Referência para o animator
    private Animator animator;

	[Header("Audio Client")]
	public scr_AudioClient audioclient;

    #endregion variables

    #region getters
    public Vector2 getVelocity()
    {
        return rb.velocity;
    }
    #endregion



    #region MonoBehaviour methods




    //Chamado ao carregar o script, inicializa variáveis

    protected void Awake()

	{
		isFacingRight = true;

		rb = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));

		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;
        
        epManager = GetComponent<scr_EPManager>();

        animator = GetComponent<Animator>();

    }

    private void Start()
    {
        //Adiciona callback de knockback da vida
        scr_HealthController health = GetComponent<scr_HealthController>();
        if (health != null)
        {
            health.addKnockbackCallback(this.onKnockBack);
			health.addDeathCallback(onDeath);
        }
    }
	
    private void Update()

    {
		//Update input
		pressedJump = Input.GetButtonDown("Jump") && !underKnockback;
		isPressingJump = Input.GetButton("Jump") && !underKnockback;

		//Verifica contato com o chão
		if(touchesGround (playerFeetPosition.position))
		{
			isGrounded = true;
			isJumping = false;
		}

		playerHorizontalMove ();
		Jump();

		coyoteCounter -= Time.deltaTime;

		//Change arm orientation

		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		mouseWorldPosition.z = transform.position.z;

		Vector3 relativeMouse = mouseWorldPosition - transform.position;

		if (relativeMouse.x + armOffset < 0 && isFacingRight)

			Flip();

		if (relativeMouse.x - armOffset > 0 && !isFacingRight)

			Flip();

		

        UpdateAnimation();
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

        //Decide se usa a velocidade pra frente ou pra traz
        float localSpeed = backwardSpeed;

        if ((movePlayerVector > 0 && isFacingRight) || movePlayerVector < 0 && !isFacingRight)
            localSpeed = speed;
        if (!isGrounded)
            localSpeed = speed;
        
        //Verifica se o jogador nao esta sobre acao de knockback
        if (!underKnockback)
        {
            //Interpola a velocidade no ar, o que controla o fator de movimento aerio do personagem
			if (isGrounded) {
				rb.velocity = new Vector2 (movePlayerVector * localSpeed, rb.velocity.y);
			}
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(movePlayerVector * localSpeed, rb.velocity.y), airControl);
            }
        }
    }


	/// <summary>
	/// Verifies if the character is in contact with the ground
	/// </summary>
	/// <returns><c>true</c>, if ground was touched, <c>false</c> otherwise.</returns>
	/// <param name="pos">Position.</param>
    bool touchesGround(Vector2 pos){

		/*Array de todos os colliders que colidem com os pés do jogador.
		 * recebe de argumento um Vector2, raio do círculo*/
		bool checkGrounded = false;

		Collider2D [] array = Physics2D.OverlapCircleAll (pos, checkGroundRadius);

		foreach (Collider2D obj in array) {
			// Debug.Log("Estou tocando: "+ obj.gameObject.name);
			//Verificação manual da layer
			if (obj.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
				checkGrounded = true;
				break;
			}
		}
		return checkGrounded;
	}
		
	//Método para Salto, adiciona velocidade no eixo Y

	private bool pressedJump;
	private bool isPressingJump;

	void Jump (){
		// rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
		if(isGrounded && pressedJump)
		{
			rb.velocity = new Vector2  (rb.velocity.x, jumpSpeed);
			isGrounded = false;
			isJumping = true;
			pressedJump = false;
		}
		
		if(!isGrounded && !isPressingJump && rb.velocity.y > 0)
		{
			isJumping = false;
			rb.velocity += Vector2.up *  Physics2D.gravity.y * (lowJumpMultiplier) * Time.deltaTime;
		}
		else if(rb.velocity.y < 0)
		{
			isJumping = false;
			rb.velocity += Vector2.up *  Physics2D.gravity.y * (fallMultiplier) * Time.deltaTime;
		}

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


	#endregion Control methods

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


        if (epManager != null)
            epManager.Flip();
	}

    void UpdateAnimation()
    {
        if (animator != null)
        {
            int inversion = (isFacingRight) ? 1 : -1;
            animator.SetFloat("HorSpeed", rb.velocity.x * inversion);
            animator.SetFloat("VerSpeed", rb.velocity.y);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    private void onKnockBack()
    {
        StartCoroutine(waitKnockback());
    }

	private void onDeath(){
		StopAllCoroutines();
		underKnockback = true;
		scr_GameManager.instance.startGameOver();
		gameObject.layer = 14;
		// gameObject.layer = LayerMask.NameToLayer("Corpse");
		animator.SetTrigger("Dead");
	}

    private IEnumerator waitKnockback()
    {
        underKnockback = true;
        float counter = 0;
        while (counter < knockbackTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        underKnockback = false;
    }

    private void OnDestroy()
    {
        scr_HealthController health = GetComponent<scr_HealthController>();
        if (health != null)
        {
            health.removeKnockbackCallback(this.onKnockBack);
        }
    }

	private void OnDrawGizmos()
	{
		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;
		Gizmos.DrawWireSphere(playerFeetPosition.position, checkGroundRadius);
	}

}

