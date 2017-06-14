using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controlador do jogador para o protótipo do ALLPUNK
 * @author João Victor L. da S. Guimarães
 */
public class scr_PlayerController : scr_Entity {


	#region variables


	//Velocidade da caminhada do personagem
	public float speed = 4.0f;
	//Velocidade do salto do personagem
	public float jumpSpeed = 4.0f;
	//Multiplicador da gravidade p/ queda, deixando-a mais rápida
	public float fallMultiplier = 1.5f;
	//Multiplicador da gravidade p/ saltos curtos
	public float lowJumpMultiplier = 1f;
    //Define se o personagem tem ou vai usar os IKs para movimentar os braços
    public bool useArmIK = true;
    //Distancia considerada até inverter o personagem
    public float armOffset;
	//Dano do ataque melee
	public float meleeAtackDamage;
	//Raio de distância entre a posição do player e o tiro
	[Range(2,4)]
	public float rangedAttackOffset = 3.0f;
	//Referencia ao projétil para ataque ranged.
	public GameObject projectile;


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
    //Distancia de ataque melee
    public float meleeAtackDistance;
	//Variable to track how much movement is needed from input
	private float movePlayerVector;
	// For determining which way the player is currently facing.
	private bool isFacingRight;
	//Referencia ao rigidBody
	private Rigidbody2D rb;

    #endregion variables


	#region MonoBehaviour methods


    //Chamado ao carregar o script, inicializa variáveis
    protected void Awake()
	{
		base.Awake ();//Awake da classe pai
		isFacingRight = true;
		rb = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;
        if (useArmIK)
        {
            rightArmIK = this.transform.Find("IK").Find("IK_RHand").GetComponent<Transform>();
            leftArmIK = this.transform.Find("IK").Find("IK_LHand").GetComponent<Transform>();
        }
    }



	/**
	 * Chamado uma vez por frame, usada para gerenciar Rigibody
	 */
	void FixedUpdate(){

		//Verifica contato com o chão
		isGrounded = touchesGround (playerFeetPosition.position);

		playerHorizontalMove ();

		if (Input.GetButtonDown ("Jump") && isGrounded)
			Jump ();
		if (rb.velocity.y < 0 && !isGrounded)
			Fall ();
		if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !isGrounded)
			lowJump();


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

    }

    private void Update()
    {
		/*
		 * OBSERVAÇÃO SOBRE O ATAQUE:
		 * Futuramente seria interessante os botoes de ataque invocarem metodos dos scripts das
		 * P.A.s que ativam seus efeitos. Para principios de prototipacao, os efeitos ficarao
		 * no controlador do player
		 */
		if (Input.GetButtonDown("Fire1"))
			meleeAttack ();
		if (Input.GetButtonDown("Fire2")) 
			rangedAttack();
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


	//Método para Salto, adiciona velocidade no eixo Y
	void Jump (){
		print ("jump");
		rb.velocity += new Vector2  (0, jumpSpeed);
	}

	//Método para Salto curto, ocorre quando o botão de salto é solto logo após saltar
	void lowJump (){
		print ("LJ");
		rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier) * Time.deltaTime;
	}
	/**
	* Método responsável por fazer com que a queda do jogador seja mais rápida
	* Isso deixa um controle de salto mais aprimorado e próprio para platformer
	*/
	void Fall (){
		print ("fall");
		rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.deltaTime;
	}


	void meleeAttack(){
		//Posiçao do centro do collider melee a ser usado
		Vector2 pos = new Vector2(transform.position.x + meleeAtackDistance / 2, transform.position.y);

		Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(meleeAtackDistance / 2,2), 0);

		foreach (Collider2D hit in hits) {
			if (hit.gameObject.tag == "Enemy") {
				scr_Enemy enemy = hit.GetComponent<scr_Enemy>();
				enemy.takeDamage(meleeAtackDamage, new Vector2(10, 0));
			}
		}
	}

	void rangedAttack(){
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPosition.z = transform.position.z;

		Vector3 gunDirection = mouseWorldPosition - transform.position;

		Vector3 gunPosition = gunDirection.normalized * rangedAttackOffset + transform.position;

		GameObject projectileClone = Instantiate (projectile, gunPosition, transform.rotation) as GameObject;
		scr_Projectile projScr = projectileClone.GetComponent<scr_Projectile>();
		projScr.Fire (gunDirection);
	}



	#endregion Control methods


	protected override void die(){
		print ("Morreu");
		Destroy(this.gameObject);
	}

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
			Transform skeletonTransform = transform.Find("Skeleton");
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
	}


}
